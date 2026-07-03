using System;
using System.Diagnostics;
using Microsoft.Win32;
using ExplorerTabUtility.Helpers;

namespace ExplorerTabUtility.Managers;

public static class RegistryManager
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ExplorerAdvancedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
    private static readonly string? ExecutablePath = Helper.GetExecutablePath();
    public static bool IsStartupEnabled => IsInStartup();

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

        try
        {
            using var key = OpenCurrentUserKey(RunKeyPath, false);
            var value = key?.GetValue(Constants.AppName) as string;
            return IsCurrentExecutablePath(value);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to read startup registry value: {ex.Message}");
            return false;
        }
    }

    private static void AddToStartup()
    {
        if (string.IsNullOrWhiteSpace(ExecutablePath)) return;

        try
        {
            using var runKey = OpenCurrentUserKey(RunKeyPath, true);
            runKey?.SetValue(Constants.AppName, QuotePath(ExecutablePath));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to add startup registry value: {ex.Message}");
        }
    }

    private static void RemoveFromStartup()
    {
        try
        {
            using var runKey = OpenCurrentUserKey(RunKeyPath, true);
            var runValue = runKey?.GetValue(Constants.AppName) as string;
            if (IsCurrentExecutablePath(runValue))
                runKey?.DeleteValue(Constants.AppName, false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to remove startup registry value: {ex.Message}");
        }
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

    private static bool IsCurrentExecutablePath(string? path)
    {
        var normalizedPath = NormalizeRunValue(path);
        return !string.IsNullOrWhiteSpace(ExecutablePath) &&
               string.Equals(normalizedPath, ExecutablePath, StringComparison.OrdinalIgnoreCase);
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
