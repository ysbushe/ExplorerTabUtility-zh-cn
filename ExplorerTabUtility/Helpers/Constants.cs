using System.Text.Json;
using ExplorerTabUtility.Models;
using ExplorerTabUtility.Strings;
using H.Hooks;

namespace ExplorerTabUtility.Helpers;

internal static class Constants
{
    internal const string AppName = "ExplorerTabUtility";
    internal const string MutexId = $"__{AppName}Hook__Mutex";
    internal static string NotifyIconText => Res.NotifyIconText;
    internal const string SettingsFileName = "settings.json";
    internal const string HotKeyProfilesFileName = "HotKeyProfiles.json";
    internal const string PortableModeFileName = "portable.mode";
    internal const string JsonFileFilter = "JSON files (*.json)|*.json|All Files|*.*";
    internal const string UpdateUrl = "https://api.github.com/repos/ysbushe/ExplorerTabUtility-zh-cn/releases/latest";
    internal static string DefaultHotKeyProfiles => JsonSerializer.Serialize(new[]
    {
        new HotKeyProfile
        {
            Name = Res.DefaultProfileHomeName ?? "Home",
            HotKeys = new[] { Key.LWin, Key.E },
            Scope = HotkeyScope.Global,
            Action = HotKeyAction.Open,
            Path = string.Empty,
            IsHandled = true,
            IsEnabled = true
        },
        new HotKeyProfile
        {
            Name = Res.DefaultProfileDuplicateName ?? "Duplicate",
            HotKeys = new[] { Key.Ctrl, Key.D },
            Scope = HotkeyScope.FileExplorer,
            Action = HotKeyAction.Duplicate,
            IsHandled = true,
            IsEnabled = true
        },
        new HotKeyProfile
        {
            Name = Res.DefaultProfileReopenClosedName ?? "Reopen Closed",
            HotKeys = new[] { Key.Shift, Key.Ctrl, Key.T },
            Scope = HotkeyScope.FileExplorer,
            Action = HotKeyAction.ReopenClosed,
            IsHandled = true,
            IsEnabled = true
        }
    });

}
