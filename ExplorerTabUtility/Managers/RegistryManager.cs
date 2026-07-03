using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using ExplorerTabUtility.Helpers;

namespace ExplorerTabUtility.Managers;

public static class RegistryManager
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string StartupApprovedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
    private const string ExplorerAdvancedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
    private const string StartupTaskName = Constants.AppName;
    private static readonly string? ExecutablePath = Helper.GetExecutablePath();
    public static bool IsStartupEnabled => IsInStartup() && IsStartupApprovedEnabled();

    public static void ToggleStartup()
    {
        if (IsStartupEnabled)
            RemoveFromStartup();
        else
            AddToStartup();
    }

    private static bool IsInStartup()
    {
        if (string.IsNullOrWhiteSpace(ExecutablePath)) return false;

        using var key = OpenCurrentUserKey(RunKeyPath, false);
        var value = key?.GetValue(Constants.AppName) as string;
        return string.Equals(NormalizeRunValue(value), ExecutablePath, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsStartupApprovedEnabled()
    {
        using var key = OpenCurrentUserKey(StartupApprovedKeyPath, false);
        var value = key?.GetValue(Constants.AppName) as byte[];
        return value == null || value.Length == 0 || value[0] % 2 == 0;
    }

    private static void AddToStartup()
    {
        if (string.IsNullOrWhiteSpace(ExecutablePath)) return;

        CleanupLegacyStartupEntries(removeCurrentRunEntry: true);

        using var runKey = OpenCurrentUserKey(RunKeyPath, true);
        runKey?.SetValue(Constants.AppName, QuotePath(ExecutablePath));

        var enabledData = new byte[12];
        enabledData[0] = 0x02;

        using var approvedKey = OpenCurrentUserKey(StartupApprovedKeyPath, true);
        approvedKey?.SetValue(Constants.AppName, enabledData, RegistryValueKind.Binary);
    }

    private static void RemoveFromStartup()
    {
        CleanupLegacyStartupEntries(removeCurrentRunEntry: true);
    }

    public static void CleanupLegacyStartupEntries(bool removeCurrentRunEntry = false)
    {
        if (removeCurrentRunEntry)
            RemoveRegistryStartupEntries();

        DeleteStartupFile($"{Constants.AppName}.vbs");
        DeleteStartupFile($"{Constants.AppName}.lnk");
        DeleteScheduledTask(StartupTaskName);
    }

    private static void RemoveRegistryStartupEntries()
    {
        using var runKey = OpenCurrentUserKey(RunKeyPath, true);
        var runValue = runKey?.GetValue(Constants.AppName) as string;
        if (!IsCurrentExecutablePath(runValue)) return;

        runKey?.DeleteValue(Constants.AppName, false);

        using var approvedKey = OpenCurrentUserKey(StartupApprovedKeyPath, true);
        approvedKey?.DeleteValue(Constants.AppName, false);
    }

    private static string? NormalizeRunValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;

        var trimmed = value.Trim();
        if (!trimmed.StartsWith('"')) return trimmed;

        var closingQuoteIndex = trimmed.IndexOf('"', 1);
        return closingQuoteIndex > 1
            ? trimmed.Substring(1, closingQuoteIndex - 1)
            : trimmed.Trim('"');
    }

    private static string QuotePath(string path) => $"\"{path}\"";

    private static void DeleteStartupFile(string fileName)
    {
        try
        {
            var startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            if (string.IsNullOrWhiteSpace(startupPath)) return;

            var targetPath = Path.Combine(startupPath, fileName);
            if (File.Exists(targetPath) && IsAppStartupFile(targetPath))
                File.Delete(targetPath);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to delete startup file {fileName}: {ex.Message}");
        }
    }

    private static void DeleteScheduledTask(string taskName)
    {
        try
        {
            if (!IsAppScheduledTask(taskName)) return;

            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = $"/Delete /TN \"{taskName}\" /F",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            if (process != null && !process.WaitForExit(3000))
                TryKill(process);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to delete startup task {taskName}: {ex.Message}");
        }
    }

    private static bool IsAppStartupFile(string path)
    {
        var extension = Path.GetExtension(path);
        if (string.Equals(extension, ".lnk", StringComparison.OrdinalIgnoreCase))
            return IsCurrentExecutablePath(GetShortcutTargetPath(path));

        if (string.Equals(extension, ".vbs", StringComparison.OrdinalIgnoreCase))
            return ContainsCurrentExecutableReference(File.ReadAllText(path));

        return false;
    }

    private static string? GetShortcutTargetPath(string shortcutPath)
    {
        object? shell = null;
        object? shortcut = null;
        try
        {
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType == null) return null;

            shell = Activator.CreateInstance(shellType);
            shortcut = shellType.InvokeMember(
                "CreateShortcut",
                BindingFlags.InvokeMethod,
                null,
                shell,
                new object[] { shortcutPath });

            return shortcut?.GetType().InvokeMember(
                "TargetPath",
                BindingFlags.GetProperty,
                null,
                shortcut,
                null) as string;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to inspect startup shortcut {shortcutPath}: {ex.Message}");
            return null;
        }
        finally
        {
            if (shortcut != null && Marshal.IsComObject(shortcut))
                Marshal.FinalReleaseComObject(shortcut);
            if (shell != null && Marshal.IsComObject(shell))
                Marshal.FinalReleaseComObject(shell);
        }
    }

    private static bool IsAppScheduledTask(string taskName)
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = $"/Query /TN \"{taskName}\" /XML",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            if (process == null) return false;

            if (!process.WaitForExit(3000))
            {
                TryKill(process);
                return false;
            }

            var output = process.StandardOutput.ReadToEnd();
            return process.ExitCode == 0 && ContainsCurrentExecutableReference(output);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to inspect startup task {taskName}: {ex.Message}");
            return false;
        }
    }

    private static bool IsCurrentExecutablePath(string? path)
    {
        var normalizedPath = NormalizeRunValue(path);
        return !string.IsNullOrWhiteSpace(ExecutablePath) &&
               string.Equals(normalizedPath, ExecutablePath, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainsCurrentExecutableReference(string? text)
    {
        return !string.IsNullOrWhiteSpace(text) &&
               !string.IsNullOrWhiteSpace(ExecutablePath) &&
               text.IndexOf(ExecutablePath, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static void TryKill(Process process)
    {
        try
        {
            process.Kill();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to stop startup cleanup helper process: {ex.Message}");
        }
    }

    public static int GetDefaultExplorerLaunchId()
    {
        using var key = OpenCurrentUserKey(ExplorerAdvancedKeyPath, false);
        if (key == null) return 1;
        return key.GetValue("LaunchTo") as int? ?? 1;
    }

    private static RegistryKey? OpenCurrentUserKey(string name, bool writable) =>
        Registry.CurrentUser.OpenSubKey(name, writable);
}
