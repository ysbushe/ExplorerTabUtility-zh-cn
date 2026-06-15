using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Win32;
using ExplorerTabUtility.Helpers;

namespace ExplorerTabUtility.Managers;

public static class RegistryManager
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string StartupApprovedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
    private const string ExplorerAdvancedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
    private static readonly string? ExecutablePath = Helper.GetExecutablePath();
    private static readonly string PortableStartupScriptPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Startup),
        $"{Constants.AppName}.vbs");
    public static bool IsStartupEnabled =>
        IsPortableStartupScriptCorrect() || (IsInStartup() && IsStartupApprovedEnabled());

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
                CreatePortableStartupScript();
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

        if (File.Exists(portableAutoStartPath) && !IsPortableStartupScriptCorrect())
            CreatePortableStartupScript();
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

        if (File.Exists(PortableStartupScriptPath))
            File.Delete(PortableStartupScriptPath);
    }

    private static bool IsPortableStartupScriptCorrect()
    {
        if (string.IsNullOrWhiteSpace(ExecutablePath) ||
            !File.Exists(PortableStartupScriptPath))
        {
            return false;
        }

        try
        {
            var script = File.ReadAllText(PortableStartupScriptPath);
            return script.Contains(
                EscapeVbScriptString(ExecutablePath),
                StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to inspect portable startup script: {ex.Message}");
            return false;
        }
    }

    private static void CreatePortableStartupScript()
    {
        if (string.IsNullOrWhiteSpace(ExecutablePath)) return;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(PortableStartupScriptPath)!);
            var escapedPath = EscapeVbScriptString(ExecutablePath);
            var script =
                "Set shell = CreateObject(\"WScript.Shell\")\r\n" +
                $"shell.Run Chr(34) & \"{escapedPath}\" & Chr(34), 0, False\r\n";
            File.WriteAllText(
                PortableStartupScriptPath,
                script,
                Encoding.Unicode);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to create portable startup script: {ex.Message}");
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

    private static string EscapeVbScriptString(string value) =>
        value.Replace("\"", "\"\"");

    public static int GetDefaultExplorerLaunchId()
    {
        using var key = OpenCurrentUserKey(ExplorerAdvancedKeyPath, false);
        if (key == null) return 1;
        return key.GetValue("LaunchTo") as int? ?? 1;
    }

    private static RegistryKey? OpenCurrentUserKey(string name, bool writable) => Registry.CurrentUser.OpenSubKey(name, writable);
}
