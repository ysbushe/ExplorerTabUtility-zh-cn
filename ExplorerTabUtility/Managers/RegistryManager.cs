using System;
using Microsoft.Win32;
using ExplorerTabUtility.Helpers;

namespace ExplorerTabUtility.Managers;

public static class RegistryManager
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string StartupApprovedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
    private const string ExplorerAdvancedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
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
        return string.Equals(value, ExecutablePath, StringComparison.OrdinalIgnoreCase);
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

        using var runKey = OpenCurrentUserKey(RunKeyPath, true);
        runKey?.SetValue(Constants.AppName, ExecutablePath);

        var enabledData = new byte[12];
        enabledData[0] = 0x02;

        using var approvedKey = OpenCurrentUserKey(StartupApprovedKeyPath, true);
        approvedKey?.SetValue(Constants.AppName, enabledData, RegistryValueKind.Binary);
    }

    private static void RemoveFromStartup()
    {
        using var runKey = OpenCurrentUserKey(RunKeyPath, true);
        runKey?.DeleteValue(Constants.AppName, false);

        using var approvedKey = OpenCurrentUserKey(StartupApprovedKeyPath, true);
        approvedKey?.DeleteValue(Constants.AppName, false);
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
