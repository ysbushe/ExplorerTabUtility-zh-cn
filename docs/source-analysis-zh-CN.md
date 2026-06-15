# ExplorerTabUtility 源码结构分析报告

> 分析日期：2026-06-15
> 项目来源：https://github.com/w4po/ExplorerTabUtility
> 版本：2.5.0

---

## 1. 项目基本信息

| 项目 | 值 |
|------|-----|
| 项目类型 | WPF 桌面应用（系统托盘工具） |
| 解决方案文件 | `ExplorerTabUtility.sln` |
| 主项目文件 | `ExplorerTabUtility/ExplorerTabUtility.csproj` |
| 构建目标框架 | `net9.0-windows` + `net481`（双目标） |
| 输出类型 | `WinExe` |
| 版本 | 2.5.0 |
| 作者 | w4po (Abd-Alrahman Ghanem) |
| 许可证 | MIT |
| 是否已有 .resx 资源文件 | **无** |
| 是否已有语言机制 | **无** |
| NuGet 依赖 | H.Hooks、Hardcodet.NotifyIcon.Wpf、AutoUpdater.NET、Fody |
| COM 依赖 | Shell32、SHDocVw（Windows Shell 自动化） |

---

## 2. 文件树概览

```
ExplorerTabUtility-master/
├── ExplorerTabUtility.sln              # 解决方案文件
├── README.md                           # 英文项目说明
├── LICENSE                             # MIT 许可证
├── .gitattributes / .gitignore / .github/
├── Assets/                             # README 引用的图片资源
├── installers/
│   └── installer.iss                   # Inno Setup 安装脚本（446行）
├── packages/                           # NuGet 包缓存
└── ExplorerTabUtility/                 # 主项目目录
    ├── ExplorerTabUtility.csproj       # 项目文件（双目标框架）
    ├── App.xaml / App.xaml.cs          # 程序入口（Mutex 单实例）
    ├── Icon.ico                        # 应用图标
    ├── FodyWeavers.xml                 # Fody IL 编织配置
    │
    ├── Helpers/                        # 工具类目录
    │   ├── Constants.cs                # 常量定义（App名、默认Profile JSON）
    │   ├── Helper.cs                   # 核心工具方法（531行）
    │   ├── KeyboardSimulator.cs        # 键盘输入模拟
    │   ├── StaTaskScheduler.cs         # STA 线程调度器
    │   ├── ProcessWatcher.cs           # 进程监控（Explorer 崩溃恢复）
    │   ├── IntPtrConverter.cs          # nint JSON 转换器
    │   └── HotKeyActionJsonConverter.cs # HotKeyAction JSON 转换器
    │
    ├── Hooks/                          # Hook 核心目录
    │   ├── IHook.cs                    # Hook 接口定义
    │   ├── ExplorerWatcher.cs          # ★ 窗口监控核心（1002行）
    │   ├── Keyboard.cs                 # 键盘低级 Hook（85行）
    │   └── Mouse.cs                    # 鼠标低级 Hook + 双击检测（104行）
    │
    ├── Managers/                       # 管理器目录
    │   ├── HookManager.cs              # Hook 编排器（206行）
    │   ├── ProfileManager.cs           # 快捷键配置管理（146行）
    │   ├── SettingsManager.cs          # 设置持久化 - JSON（224行）
    │   ├── RegistryManager.cs          # 开机启动注册表管理（76行）
    │   ├── UpdateManager.cs            # 自动更新（73行）
    │   └── ClipboardManager.cs         # 剪贴板管理
    │
    ├── Models/                         # 数据模型目录
    │   ├── HotKeyProfile.cs            # 快捷键配置模型（51行）
    │   ├── HotKeyAction.cs             # 动作枚举（16种，42行）
    │   ├── HotkeyScope.cs              # 作用范围枚举（Global/FileExplorer）
    │   ├── HotKeyEventArgs.cs          # 事件参数
    │   ├── WindowRecord.cs             # 窗口记录模型（23行）
    │   ├── WindowInfo.cs               # 窗口信息
    │   ├── SupporterInfo.cs            # 支持者信息
    │   └── DualKeyDictionary.cs        # 双键字典数据结构
    │
    ├── Interop/                        # COM/Shell 互操作目录
    │   ├── ShellPathComparer.cs        # Shell 路径比较（PIDL）
    │   ├── IShellFolder.cs             # IShellFolder COM 接口
    │   ├── IShellBrowser.cs            # IShellBrowser COM 接口
    │   ├── IServiceProvider.cs         # IServiceProvider COM 接口
    │   └── IAccessible.cs              # IAccessible 无障碍接口
    │
    ├── WinAPI/                         # Windows API 目录
    │   ├── WinApi.cs                   # P/Invoke 声明（187行）
    │   ├── INPUT.cs                    # 键鼠输入结构体
    │   ├── RECT.cs                     # 矩形结构体
    │   ├── VirtualKey.cs               # 虚拟键码枚举
    │   └── WinEventDelegate.cs         # WinEvent 回调委托
    │
    └── UI/                             # 界面目录
        ├── Views/
        │   ├── MainWindow.xaml/.cs     # ★ 主设置窗口（343行 XAML / 284行 CS）
        │   ├── TabSearchPopup.xaml/.cs # 标签页搜索弹窗（184行 / 209行）
        │   ├── HotKeyProfileControl.xaml/.cs  # 快捷键配置控件（119行 / 355行）
        │   ├── AboutView.xaml/.cs      # 关于页面（685行 / 86行）
        │   ├── CustomMessageBox.xaml/.cs # 自定义消息框（136行 / 150行）
        │   └── Controls/
        │       ├── SystemTrayIcon.xaml/.cs    # ★ 托盘图标（131行 / 250行）
        │       └── NumericInputControl.xaml/.cs # 数字输入控件
        ├── Themes/                     # 样式主题文件（10个 XAML）
        │   ├── Colors.xaml、ThemeResources.xaml、DefaultStyles.xaml
        │   ├── ButtonStyles.xaml、CheckBoxStyles.xaml、ComboBoxStyles.xaml
        │   ├── TextBoxStyles.xaml、TabControlStyles.xaml、ListBoxStyles.xaml
        │   ├── ScrollViewerStyles.xaml、GroupBoxStyles.xaml、BorderStyles.xaml
        ├── Converters/                 # 值转换器
        │   ├── EnumDescriptionConverter.cs
        │   └── Base64ImageConverter.cs
        ├── Commands/
        │   └── RelayCommand.cs         # ICommand 实现
        └── Behaviors/                  # WPF 行为
            ├── HoverBrightness.cs
            ├── HoverBorder.cs
            └── HoverBackground.cs
```

---

## 3. 各主要目录作用

| 目录 | 作用 | 文件数 |
|------|------|--------|
| `Helpers/` | 工具类：常量、通用方法、键盘模拟、线程调度、进程监控 | 7 |
| `Hooks/` | 三层 Hook 实现：窗口事件、键盘低级钩子、鼠标低级钩子 | 4 |
| `Managers/` | 业务管理器：Hook 编排、配置管理、设置持久化、注册表、更新 | 6 |
| `Models/` | 数据模型：配置、动作、范围、窗口记录等 | 8 |
| `Interop/` | COM 互操作接口定义（Shell32、SHDocVw、IAccessible） | 5 |
| `WinAPI/` | Windows API P/Invoke 声明和结构体 | 5 |
| `UI/Views/` | WPF 窗口和用户控件 | 7 |
| `UI/Themes/` | WPF 样式和主题资源 | 10 |
| `installers/` | Inno Setup 安装脚本 | 1 |

---

## 4. 程序入口与启动流程

```
App.xaml.cs OnStartup()
  ├── Mutex 单实例检查（__ExplorerTabUtilityHook__Mutex）
  ├── SetupTooltipBehavior() - 配置 ToolTip 行为
  └── new MainWindow()
        ├── InitializeComponent()
        ├── 从 SettingsManager 读取设置
        ├── new ProfileManager(ProfilesPanel) - 加载快捷键配置
        ├── new HookManager(_profileManager) - 初始化 Hook 管理器
        │     ├── new ExplorerWatcher() - 窗口监控（COM ShellWindows）
        │     ├── new Mouse(profiles) - 鼠标 Hook
        │     └── new Keyboard(profiles) - 键盘 Hook
        ├── new SystemTrayIcon(...) - 托盘图标
        ├── StartHooks() - 根据设置启动 Hook
        ├── 首次运行检测 → 显示主窗口
        └── 自动更新检测（如果启用）
```

---

## 5. 核心功能实现位置

### 5.1 新窗口转标签页（核心功能）

**文件：** `Hooks/ExplorerWatcher.cs`

**关键方法调用链：**
```
OnShellWindowRegistered()                    // COM WindowRegistered 事件
  → GetRecentlyCreatedWindow()               // 获取新窗口的 InternetExplorer COM 对象
  → GetLocation(window)                      // 获取窗口路径
  → Helper.HideWindow(hWnd)                  // 隐藏新窗口（透明或移出屏幕）
  → OpenTabNavigateWithSelection()           // 在主窗口中打开新标签页
    → RequestToOpenNewTab()                  // 发送 Ctrl+T 命令创建新标签
    → ListenForNewExplorerTabAsync()         // 等待新标签出现
    → Navigate(window, path)                 // 导航到目标路径
    → SelectItems(window, selectedItems)     // 选中之前选中的文件
  → window.Quit()                            // 关闭原始新窗口
```

**窗口隐藏机制（两种模式）：**
1. **默认模式：** 设置窗口透明度为 0（WS_EX_LAYERED + LWA_ALPHA）
2. **主题兼容模式：** 将窗口移出屏幕（-32000, -32000），保留主题

### 5.2 鼠标空白处双击向上一级

**文件：** `Hooks/Mouse.cs` + `Helpers/Helper.cs`

**关键方法：**
```
Mouse.LowLevelMouseHook_Down()               // 鼠标低级钩子回调
  → IsDoubleClick(e.CurrentKey)              // 双击检测（500ms 内同键两次）
  → 匹配 HotKeyProfile（IsMouse=true, IsDoubleClick=true）
  → Helper.IsExplorerEmptySpace(position)    // 检查是否点击在空白处
    → WinApi.AccessibleObjectFromPoint()     // IAccessible 接口
    → accObj.get_accRole(0) == 0x21          // ROLE_SYSTEM_LIST = 文件列表空白处
  → 触发 NavigateUp 动作
    → KeyboardSimulator.ModifiedKeyStroke(Alt, Up)  // 模拟 Alt+Up
```

### 5.3 快捷键系统

**文件：** `Hooks/Keyboard.cs` + `Hooks/Mouse.cs`

**支持的动作类型（HotKeyAction 枚举）：**

| 枚举值 | 动作 | 描述 |
|--------|------|------|
| Open | 打开路径 | 打开指定路径（文件夹/文件/URL） |
| Duplicate | 复制标签页 | 复制当前标签页 |
| ReopenClosed | 重开已关闭 | 重新打开最近关闭的标签页 |
| TabSearch | 标签页搜索 | 打开标签页搜索弹窗 |
| NavigateBack | 后退 | 向后导航 |
| NavigateForward | 前进 | 向前导航 |
| NavigateUp | 向上一级 | 向上一级目录 |
| SetTargetWindow | 设为目标窗口 | 设置接收新标签的窗口 |
| ToggleWinHook | 切换窗口监控 | 开关窗口监控 |
| ToggleReuseTabs | 切换复用标签 | 开关标签复用 |
| ToggleVisibility | 显示/隐藏 | 切换主窗口可见性 |
| DetachTab | 分离标签页 | 将标签页分离为新窗口 |
| SnapRight/Left/Up/Down | 贴靠窗口 | 将窗口贴靠到屏幕边缘 |

**作用范围（HotkeyScope）：**
- `Global`：全局生效（任何应用中）
- `FileExplorer`：仅在资源管理器中生效

---

## 6. 设置保存位置和字段结构

### 6.1 设置文件路径

```
%APPDATA%\ExplorerTabUtility\settings.json
```

等效于：
```
C:\Users\<用户名>\AppData\Roaming\ExplorerTabUtility\settings.json
```

### 6.2 设置字段结构

```json
{
  "MouseHook": false,              // 鼠标 Hook 是否启用
  "KeyboardHook": true,            // 键盘 Hook 是否启用
  "WindowHook": true,              // 窗口 Hook 是否启用（核心功能开关）
  "ReuseTabs": true,               // 是否复用已有标签页
  "FormSize": {                    // 主窗口尺寸
    "Width": 852,
    "Height": 402
  },
  "SaveProfilesOnExit": true,      // 关闭时自动保存配置
  "IsFirstRun": true,              // 是否首次运行
  "IsTrayIconHidden": false,       // 托盘图标是否隐藏
  "HaveThemeIssue": false,         // 是否有主题兼容问题
  "AutoUpdate": false,             // 是否自动更新
  "HotKeyProfiles": "[...]",       // 快捷键配置（JSON 字符串）
  "SaveClosedWindows": false,      // 是否保存已关闭窗口历史
  "RestorePreviousWindows": false,  // 是否恢复上次窗口
  "ClosedWindows": null            // 已关闭窗口记录数组
}
```

### 6.3 快捷键配置结构（HotKeyProfiles 内部）

```json
{
  "Id": " GUID ",
  "Name": "配置名称",
  "HotKeys": [键码数组],
  "Scope": 0,           // 0=Global, 1=FileExplorer
  "Action": 0,          // HotKeyAction 枚举值
  "Path": "路径",       // 仅 Open 动作使用
  "IsHandled": true,    // 是否拦截按键传递给其他应用
  "IsEnabled": true,    // 是否启用
  "IsAsTab": true,      // 是否作为标签页打开
  "IsMouse": false,     // 是否为鼠标触发
  "IsDoubleClick": false, // 是否需要双击
  "Delay": 0            // 执行延迟（毫秒）
}
```

### 6.4 默认快捷键配置

| 名称 | 快捷键 | 范围 | 动作 | 路径 |
|------|--------|------|------|------|
| Home | Win+E | Global | Open | ""（默认主页） |
| Duplicate | Ctrl+D | FileExplorer | Duplicate | - |
| ReopenClosed | Shift+Ctrl+T | FileExplorer | ReopenClosed | - |

---

## 7. 依赖关系图

```
ExplorerTabUtility.exe
├── .NET 9.0 Desktop Runtime / .NET Framework 4.8.1
├── H.Hooks (v1.7.0)           # 低级键盘/鼠标钩子
├── Hardcodet.NotifyIcon.Wpf (v2.0.1)  # WPF 系统托盘图标
├── AutoUpdater.NET.Extended.Markdown (v1.9.5.2)  # 自动更新
├── Fody (v6.9.2) + ConfigureAwait.Fody (v3.3.2)  # IL 编织
├── Shell32 (COM)               # Windows Shell 自动化
├── SHDocVw (COM)               # Explorer 窗口/标签管理
├── user32.dll (P/Invoke)       # 窗口管理、输入模拟
├── kernel32.dll (P/Invoke)     # 进程管理
├── shell32.dll (P/Invoke)      # Shell 操作
└── oleacc.dll (P/Invoke)       # 无障碍接口（空白处检测）
```

---

## 8. 线程模型

| 线程类型 | 用途 | 位置 |
|----------|------|------|
| UI 线程 | WPF 界面渲染、事件处理 | MainWindow、SystemTrayIcon |
| STA 线程 | COM 对象操作（ShellWindows、InternetExplorer） | StaTaskScheduler |
| 线程池 | 键盘/鼠标 Hook 回调、异步操作 | ThreadPool.QueueUserWorkItem |
| 定时器 | Explorer 进程监控 | Timer (ExplorerWatcher) |

---

## 9. 风险点

| 风险 | 描述 | 影响 |
|------|------|------|
| COM 接口依赖 | 依赖 Shell32/SHDocVw 的 InternetExplorer 接口 | Windows 更新可能改变内部行为 |
| 魔法命令 | 使用 0xA221、0xA21B、0xA021 等未公开的 WM_COMMAND | 可能在新版本 Windows 中失效 |
| 隐藏窗口 | 使用 WS_EX_LAYERED 透明或移出屏幕 | 可能与某些工具冲突 |
| 低级钩子 | 全局键盘/鼠标钩子 | 可能被杀毒软件误报 |
| 单实例 | Mutex 限制单实例 | 多用户场景需注意 |
| 进程监控 | 每秒检查 Explorer 进程 | 极低开销但非零 |
