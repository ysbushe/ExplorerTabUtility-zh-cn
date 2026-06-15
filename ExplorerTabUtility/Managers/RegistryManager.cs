using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using ExplorerTabUtility.Helpers;

namespace ExplorerTabUtility.Managers;

public static class RegistryManager
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string StartupApprovedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
    private const string ExplorerAdvancedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
    private static readonly string? ExecutablePath = Helper.GetExecutablePath();
    private static readonly string StartupShortcutPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Startup),
        $"{Constants.AppName}.lnk");
    public static bool IsStartupEnabled =>
        IsStartupShortcutCorrect() || (IsInStartup() && IsStartupApprovedEnabled());

    public static void ToggleStartup()
    {
        if (IsStartupEnabled)
        {
            RemoveFromStartup();
            DeletePortableAutoStartMarker();
        }
        else
        {
            if (IsPortableMode())
                CreateStartupShortcut();
            else
                AddToStartup();
            CreatePortableAutoStartMarker();
        }
    }

    public static void EnsurePortableStartup()
    {
        var portableAutoStartPath = Path.Combine(
            AppContext.BaseDirectory,
            Constants.PortableAutoStartFileName);

        if (File.Exists(portableAutoStartPath) && !IsStartupShortcutCorrect())
            CreateStartupShortcut();
    }

    private static bool IsInStartup()
    {
        if (string.IsNullOrWhiteSpace(ExecutablePath)) return false;

        // Check if the application exists in the Run registry key and has the correct executable location
        using var key = OpenCurrentUserKey(RunKeyPath, false);
        var value = key?.GetValue(Constants.AppName) as string;
        return string.Equals(value, ExecutablePath, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsStartupApprovedEnabled()
    {
        using var key = OpenCurrentUserKey(StartupApprovedKeyPath, false);
        var value = key?.GetValue(Constants.AppName) as byte[];
        // Check first byte parity (even = enabled, odd = disabled), null and empty also mean enabled.
        return value == null || value.Length == 0 || value[0] % 2 == 0;
    }

    private static void AddToStartup()
    {
        if (string.IsNullOrWhiteSpace(ExecutablePath)) return;

        try
        {
            // Add to Run registry key
            using var runKey = OpenCurrentUserKey(RunKeyPath, true);
            runKey?.SetValue(Constants.AppName, ExecutablePath);

            // Create enabled entry in StartupApproved
            var enabledData = new byte[12];
            enabledData[0] = 0x02; // Even value for enabled

            using var approvedKey = OpenCurrentUserKey(StartupApprovedKeyPath, true);
            approvedKey?.SetValue(Constants.AppName, enabledData, RegistryValueKind.Binary);
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.WriteLine($"Failed to add startup registry entry: {ex.Message}");
        }
    }

    private static void RemoveFromStartup()
    {
        try
        {
            // Remove from Run registry key
            using var runKey = OpenCurrentUserKey(RunKeyPath, true);
            runKey?.DeleteValue(Constants.AppName, false);

            // Remove from StartupApproved
            using var approvedKey = OpenCurrentUserKey(StartupApprovedKeyPath, true);
            approvedKey?.DeleteValue(Constants.AppName, false);
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.WriteLine($"Failed to remove startup registry entry: {ex.Message}");
        }

        if (File.Exists(StartupShortcutPath))
            File.Delete(StartupShortcutPath);
    }

    private static bool IsStartupShortcutCorrect()
    {
        if (string.IsNullOrWhiteSpace(ExecutablePath) ||
            !File.Exists(StartupShortcutPath))
        {
            return false;
        }

        dynamic? shell = null;
        dynamic? shortcut = null;
        try
        {
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType == null) return false;

            shell = Activator.CreateInstance(shellType);
            shortcut = shell?.CreateShortcut(StartupShortcutPath);
            return string.Equals(
                shortcut?.TargetPath as string,
                ExecutablePath,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to inspect startup shortcut: {ex.Message}");
            return false;
        }
        finally
        {
            ReleaseComObject(shortcut);
            ReleaseComObject(shell);
        }
    }

    private static void CreateStartupShortcut()
    {
        if (string.IsNullOrWhiteSpace(ExecutablePath)) return;

        dynamic? shell = null;
        dynamic? shortcut = null;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StartupShortcutPath)!);
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType == null) return;

            shell = Activator.CreateInstance(shellType);
            shortcut = shell?.CreateShortcut(StartupShortcutPath);
            if (shortcut == null) return;

            shortcut.TargetPath = ExecutablePath;
            shortcut.WorkingDirectory = AppContext.BaseDirectory;
            shortcut.Description = "ExplorerTabUtility portable startup";
            shortcut.Save();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to create startup shortcut: {ex.Message}");
        }
        finally
        {
            ReleaseComObject(shortcut);
            ReleaseComObject(shell);
        }
    }

    private static void CreatePortableAutoStartMarker()
    {
        if (!IsPortableMode()) return;

        File.WriteAllText(
            Path.Combine(AppContext.BaseDirectory, Constants.PortableAutoStartFileName),
            "Restore the current executable path at next launch.");
    }

    private static void DeletePortableAutoStartMarker()
    {
        var markerPath = Path.Combine(
            AppContext.BaseDirectory,
            Constants.PortableAutoStartFileName);
        if (File.Exists(markerPath))
            File.Delete(markerPath);
    }

    private static bool IsPortableMode() =>
        File.Exists(Path.Combine(AppContext.BaseDirectory, Constants.PortableModeFileName));

    private static void ReleaseComObject(object? value)
    {
        if (value != null && Marshal.IsComObject(value))
            Marshal.FinalReleaseComObject(value);
    }

    public static int GetDefaultExplorerLaunchId()
    {
        using var key = OpenCurrentUserKey(ExplorerAdvancedKeyPath, false);
        if (key == null) return 1;
        return key.GetValue("LaunchTo") as int? ?? 1;
    }

    private static RegistryKey? OpenCurrentUserKey(string name, bool writable) => Registry.CurrentUser.OpenSubKey(name, writable);
}
