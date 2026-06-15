using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using H.Hooks;
using ExplorerTabUtility.Models;
using ExplorerTabUtility.Helpers;
using ExplorerTabUtility.WinAPI;

namespace ExplorerTabUtility.Hooks;

public sealed class Mouse : IHook
{
    private int _lastClickTime;
    private Key _lastClickKey;
    private Point _lastClickPosition;
    private bool _lastClickWasExplorerEmptySpace;
    private readonly LowLevelMouseHook _lowLevelMouseHook;
    private readonly IReadOnlyCollection<HotKeyProfile> _hotkeyProfiles;
    public bool IsHookActive => _lowLevelMouseHook.IsStarted;
    public event Action<HotKeyEventArgs>? OnHotKeyProfileTriggered;

    public Mouse(IReadOnlyCollection<HotKeyProfile> hotkeyProfiles)
    {
        _hotkeyProfiles = hotkeyProfiles;
        _lowLevelMouseHook = new LowLevelMouseHook { AddKeyboardKeys = true };
        _lowLevelMouseHook.Down += LowLevelMouseHook_Down;
    }

    public void StartHook() => _lowLevelMouseHook.Start();
    public void StopHook() => _lowLevelMouseHook.Stop();

    private void LowLevelMouseHook_Down(object? sender, MouseEventArgs e)
    {
        var handler = OnHotKeyProfileTriggered;
        if (handler == null) return;

        var tracksEmptySpaceDoubleClick = _hotkeyProfiles.Any(profile =>
            profile is
            {
                IsMouse: true,
                IsEnabled: true,
                IsDoubleClick: true,
                Action: HotKeyAction.NavigateUp,
                Scope: HotkeyScope.FileExplorer
            } &&
            profile.HotKeys is { Length: > 0 } hotKeys &&
            e.Keys.Are(hotKeys));

        var currentClickIsExplorerEmptySpace =
            tracksEmptySpaceDoubleClick &&
            Helper.IsFileExplorerForeground(out _) &&
            Helper.IsExplorerEmptySpace(e.Position);

        var click = RegisterClick(e.CurrentKey, e.Position, currentClickIsExplorerEmptySpace);

        bool? isFileExplorerForeground = null;
        nint handle = 0;
        foreach (var profile in _hotkeyProfiles)
        {
            // Skip disabled, empty or non mouse
            if (!profile.IsMouse || !profile.IsEnabled || profile.HotKeys is null || profile.HotKeys.Length == 0)
                continue;
            
            // Skip if it requires double-click and it is not
            if (profile.IsDoubleClick && !click.IsDoubleClick) continue;

            // NavigateUp from the mouse is intentionally conservative: both clicks
            // must land on the Explorer file list's empty space.
            if (profile is { IsDoubleClick: true, Action: HotKeyAction.NavigateUp } &&
                !click.IsExplorerEmptySpaceDoubleClick)
                continue;
            
            // Skip if keys do not match
            if (!e.Keys.Are(profile.HotKeys)) continue;

            // Let's see if we need to check File Explorer
            if (profile.Scope == HotkeyScope.FileExplorer)
            {
                // Check if File Explorer is foreground (only once)
                isFileExplorerForeground ??= Helper.IsFileExplorerForeground(out handle);

                if (isFileExplorerForeground == false)
                {
                    handle = 0; // Reset handle if not File Explorer
                    continue;
                }
            }
            
            // Queue the hotkey trigger in a separate thread.
#if NET7_0_OR_GREATER
            ThreadPool.QueueUserWorkItem(static s => s.Handler.Invoke(new HotKeyEventArgs(s.Profile, s.Handle, s.Position)),
                new State(handler, profile, handle, e.Position), false);
#else
            ThreadPool.QueueUserWorkItem(static state =>
            {
                var s = (State)state!;
                s.Handler.Invoke(new HotKeyEventArgs(s.Profile, s.Handle, s.Position));
            }, new State(handler, profile, handle, e.Position));
#endif
        }
    }

    private ClickState RegisterClick(Key currentKey, Point position, bool isExplorerEmptySpace)
    {
        var now = Environment.TickCount;
        var elapsed = unchecked((uint)(now - _lastClickTime));
        var maxX = Math.Max(1, WinApi.GetSystemMetrics(WinApi.SM_CXDOUBLECLK) / 2);
        var maxY = Math.Max(1, WinApi.GetSystemMetrics(WinApi.SM_CYDOUBLECLK) / 2);

        var isDoubleClick =
            _lastClickKey == currentKey &&
            elapsed <= WinApi.GetDoubleClickTime() &&
            Math.Abs(position.X - _lastClickPosition.X) <= maxX &&
            Math.Abs(position.Y - _lastClickPosition.Y) <= maxY;

        var isExplorerEmptySpaceDoubleClick =
            isDoubleClick &&
            _lastClickWasExplorerEmptySpace &&
            isExplorerEmptySpace;

        if (isDoubleClick)
        {
            _lastClickTime = 0;
            _lastClickKey = default;
            _lastClickPosition = default;
            _lastClickWasExplorerEmptySpace = false;
        }
        else
        {
            _lastClickTime = now;
            _lastClickKey = currentKey;
            _lastClickPosition = position;
            _lastClickWasExplorerEmptySpace = isExplorerEmptySpace;
        }

        return new ClickState(isDoubleClick, isExplorerEmptySpaceDoubleClick);
    }

    public void Dispose()
    {
        StopHook();
        _lowLevelMouseHook.Dispose();
    }

    private readonly struct State(Action<HotKeyEventArgs> handler, HotKeyProfile profile, nint handle, Point position)
    {
        public readonly Action<HotKeyEventArgs> Handler = handler;
        public readonly HotKeyProfile Profile = profile;
        public readonly nint Handle = handle;
        public readonly Point Position = position;
    }

    private readonly struct ClickState(bool isDoubleClick, bool isExplorerEmptySpaceDoubleClick)
    {
        public bool IsDoubleClick { get; } = isDoubleClick;
        public bool IsExplorerEmptySpaceDoubleClick { get; } = isExplorerEmptySpaceDoubleClick;
    }
}
