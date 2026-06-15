# ExplorerTabUtility 汉化实施报告

> 分支：`feature/zh-cn-localization`
> 日期：2026-06-15
> 方案：.resx 双语资源文件 + Language 设置项

---

## 一、改动清单

### 1.1 新增文件

| 文件路径 | 类型 | 行数 | 用途 |
|----------|------|------|------|
| `ExplorerTabUtility/Strings/Strings.resx` | 资源文件 | ~180 | 英文默认资源（120+ 条键值对） |
| `ExplorerTabUtility/Strings/Strings.zh-CN.resx` | 资源文件 | ~180 | 中文简体资源（120+ 条键值对） |
| `ExplorerTabUtility/Strings/Strings.Designer.cs` | C# 代码 | ~130 | 自动生成的资源访问类（static 属性） |
| `ExplorerTabUtility/Strings/LanguageManager.cs` | C# 代码 | ~35 | 语言初始化管理器 |

### 1.2 修改文件

| 文件路径 | 改动类型 | 改动内容 |
|----------|----------|----------|
| `ExplorerTabUtility/ExplorerTabUtility.csproj` | 配置 | 添加 `Strings.Designer.cs` 编译引用 |
| `ExplorerTabUtility/Managers/SettingsManager.cs` | 新增字段 | `AppSettings.Language` 属性（默认 `"Auto"`） |
| `ExplorerTabUtility/App.xaml.cs` | 初始化 | 启动时调用 `LanguageManager.Initialize()` |
| `ExplorerTabUtility/Helpers/Constants.cs` | 属性化 | `NotifyIconText` 从 const 改为 static 属性读取资源 |
| `ExplorerTabUtility/UI/Views/MainWindow.xaml` | XAML 绑定 | 20+ 处文本改为 `{x:Static strings:Strings.XXX}` |
| `ExplorerTabUtility/UI/Views/Controls/SystemTrayIcon.xaml` | XAML 绑定 | 8 个菜单项 + 8 个 ToolTip |
| `ExplorerTabUtility/UI/Views/HotKeyProfileControl.xaml` | XAML 绑定 | 14 个 ToolTip + 3 个标签文本 |
| `ExplorerTabUtility/UI/Views/TabSearchPopup.xaml` | XAML 绑定 | 1 个 ToolTip |
| `ExplorerTabUtility/UI/Views/AboutView.xaml` | XAML 绑定 | 8 处文本 |
| `ExplorerTabUtility/UI/Views/MainWindow.xaml.cs` | 代码替换 | 3 处对话框消息 |
| `ExplorerTabUtility/UI/Views/TabSearchPopup.xaml.cs` | 代码替换 | 2 处对话框消息 |
| `ExplorerTabUtility/UI/Views/CustomMessageBox.xaml.cs` | 代码替换 | 4 个按钮文本（OK/Cancel/Yes/No） |
| `ExplorerTabUtility/Hooks/ExplorerWatcher.cs` | 代码替换 | 2 处对话框消息 |

### 1.3 未修改文件（保持原样）

| 文件 | 原因 |
|------|------|
| `Models/HotKeyAction.cs` | 枚举 Description 需运行时读取资源，待后续实现 |
| `installers/installer.iss` | Inno Setup 脚本需 Inno Setup 编译器处理 |
| `UI/Themes/*.xaml` | 样式文件无用户可见文本 |
| `Hooks/ExplorerWatcher.cs` 核心逻辑 | 仅替换用户可见提示文本，不改动核心逻辑 |
| `Hooks/Mouse.cs` / `Keyboard.cs` | 不涉及用户可见文本 |
| `WinAPI/*.cs` | 不涉及用户可见文本 |

---

## 二、资源文件内容

### 2.1 Strings.resx（英文默认）键值对清单

#### MainWindow 相关
| Key | Value |
|-----|-------|
| AppTitle | Explorer Tab Utility |
| NavShortcuts | Shortcuts |
| NavPreferences | Preferences |
| NavAbout | About |
| BtnNew | NEW |
| BtnImport | IMPORT |
| BtnExport | EXPORT |
| BtnSave | SAVE |
| CbAutoSave | Auto save |
| GrpAppSettings | Application Settings |
| CbAutoUpdate | Auto update |
| CbThemeIssue | I have theme issues |
| CbSaveClosedHistory | Save closed history |
| CbRestorePrevious | Restore previous windows |
| CbHideTrayIcon | Hide tray icon |
| StatusWindowHook | Window Hook |
| StatusReuseTabs | Reuse Tabs |
| StatusKeyboardHook | Keyboard Hook |
| StatusMouseHook | Mouse Hook |

#### SystemTrayIcon 相关
| Key | Value |
|-----|-------|
| MenuKeyboardHook | Keyboard Hook |
| MenuMouseHook | Mouse Hook |
| MenuWindowHook | Window Hook |
| MenuReuseTabs | Reuse Tabs |
| MenuAddToStartup | Add to startup |
| MenuCheckUpdates | Check for updates |
| MenuSettings | Settings |
| MenuExit | Exit |

#### HotKeyProfileControl 相关
| Key | Value |
|-----|-------|
| ProfileHandled | Handled |
| ProfileTab | Tab |
| ProfileDelay | Delay |

#### AboutView 相关
| Key | Value |
|-----|-------|
| AboutSubtitle | Enhance your Windows File Explorer experience |
| AboutStarGitHub | Star on GitHub |
| AboutSupportProject | Support the Project |
| AboutSupportDesc | If you find this utility helpful, consider supporting its development through one of these options: |
| AboutDeveloper | Developer |
| AboutCurrentSupporters | Current Supporters |
| AboutSupportersDesc | Thank you to all the amazing people who support this project! |
| AboutBeFirst | Be the first to support this project! |
| AboutKeepAlive | Your support helps keep this project alive |

#### CustomMessageBox 相关
| Key | Value |
|-----|-------|
| BtnOK | OK |
| BtnCancel | Cancel |
| BtnYes | Yes |
| BtnNo | No |

#### 对话框消息
| Key | Value |
|-----|-------|
| MsgAnotherInstance | Another instance is already running.\nCheck in System Tray Icons. |
| MsgRestoreWindows | Do you want to restore previously opened windows? |
| MsgToggleVisibility | You can show the app again by pressing {0} |
| MsgNoToggleVisibility | Cannot hide tray icon if no hotkey is configured to toggle visibility. |
| ConfirmClearHistory | Are you sure you want to clear the closed windows history? |
| ConfirmClearHistoryTitle | Confirm Clear History |
| NotifyIconText | Explorer Tab Utility: Force new windows to tabs. |

#### ToolTip 文本（30+ 条）
| Key | Value |
|-----|-------|
| TipOpenAppMenu | Open application menu |
| TipConfigureShortcuts | Configure keyboard and mouse shortcuts |
| TipConfigureSettings | Configure application settings |
| TipAboutApp | About the application and support options |
| TipCreateProfile | Create a new profile |
| TipImportProfiles | Import profiles from a file |
| TipExportProfiles | Export profiles to a file |
| TipSaveProfiles | Persist all profile changes |
| TipAutoSave | Automatically save profiles when closing the window |
| TipAutoUpdate | Automatically check for updates on startup |
| TipThemeIssue | Use alternative window hiding method... |
| TipSaveClosedHistory | Save closed windows history... |
| TipRestorePrevious | Restore previously opened windows after restart or crash |
| TipHideTrayIcon | Hide the system tray icon |
| TipEnableKeyboard | Enable or disable keyboard shortcuts |
| TipEnableMouse | Enable or disable mouse shortcuts |
| TipToggleWindowHook | Toggle automatic redirection of new Explorer windows to tabs |
| TipToggleReuseTabs | When enabled, navigates to existing tabs... |
| TipToggleStartup | Automatically start Explorer Tab Utility when Windows starts |
| TipCheckUpdates | Check if a newer version... |
| TipOpenSettings | Open the settings window... |
| TipExitApp | Close Explorer Tab Utility and stop all hooks |
| TipEnableProfile | Enable or disable this profile |
| TipProfileName | Name of the profile |
| TipProfileHotkeys | Keyboard or mouse keys to listen for... |
| TipProfileScope | Scope of the hotkeys... |
| TipProfileAction | Action to perform when the hotkeys are pressed |
| TipShowMore | Show more options |
| TipDeleteProfile | Delete this profile |
| TipProfilePath | Path to open when the hotkeys are pressed... |
| TipProfileDelay | Delay in milliseconds before performing the action |
| TipProfileHandled | Prevent further processing of the hotkeys in other applications |
| TipProfileAsTab | Open as tab instead of a new window |
| TipClearHistory | Clear closed windows history |

### 2.2 Strings.zh-CN.resx（中文简体）对应翻译

所有 Key 与英文版一一对应，Value 为中文翻译。详见 `ExplorerTabUtility/Strings/Strings.zh-CN.resx`。

---

## 三、构建结果

### 3.1 当前环境限制

| 项目 | 状态 |
|------|------|
| .NET SDK | 8.0.422（需要 9.0） |
| .NET Framework MSBuild | ❌ 未安装 |
| Visual Studio 2022 | ❌ 未检测到 |
| COM 引用编译 | 需要 .NET Framework MSBuild |

**结论：** 当前环境无法编译项目，需要在安装了 .NET 9 SDK + Visual Studio 2022 的开发机上构建。

### 3.2 构建步骤（在开发机上执行）

```bash
# 1. 安装 .NET 9 SDK
# 下载地址：https://dotnet.microsoft.com/download/dotnet/9.0

# 2. 克隆仓库并切换分支
git clone https://github.com/w4po/ExplorerTabUtility.git
cd ExplorerTabUtility
git checkout feature/zh-cn-localization

# 3. 还原依赖
dotnet restore

# 4. 构建（Release）
dotnet build -c Release

# 5. 或使用 Visual Studio
# 打开 ExplorerTabUtility.sln → 生成 → 生成解决方案
```

### 3.3 预期构建结果

| 目标框架 | 输出路径 | 说明 |
|----------|----------|------|
| net9.0-windows | `bin/Release/net9.0-windows/` | 需要 .NET 9 运行时 |
| net481 | `bin/Release/net481/` | 需要 .NET Framework 4.8.1 |

### 3.4 构建验证清单

- [ ] `Strings.resx` 编译为嵌入资源
- [ ] `Strings.zh-CN.resx` 编译为卫星程序集
- [ ] `Strings.Designer.cs` 生成正确的静态属性
- [ ] XAML 中的 `{x:Static}` 绑定无编译错误
- [ ] C# 代码中的 `Strings.XXX` 引用无编译错误
- [ ] `LanguageManager` 在启动时正确初始化
- [ ] `SettingsManager.Language` 字段正确读写

---

## 四、测试计划

### 4.1 语言切换测试

| 测试项 | 步骤 | 预期结果 |
|--------|------|----------|
| Auto 模式（中文系统） | 删除 settings.json，启动程序 | 界面显示中文 |
| Auto 模式（英文系统） | 删除 settings.json，启动程序 | 界面显示英文 |
| 强制英文 | 设置 `"Language": "en-US"`，重启 | 界面显示英文 |
| 强制中文 | 设置 `"Language": "zh-CN"`，重启 | 界面显示中文 |
| 语言持久化 | 切换语言后重启程序 | 语言设置保留 |
| 首次运行 | 删除 settings.json，启动 | 默认 Auto，显示引导窗口 |

### 4.2 功能回归测试

| 测试项 | 步骤 | 预期结果 |
|--------|------|----------|
| 窗口监控 | 右键托盘 → 勾选窗口监控 | 状态栏绿灯亮 |
| 复用标签页 | 右键托盘 → 勾选复用标签页 | 状态栏绿灯亮 |
| 新窗口转标签 | 打开新资源管理器窗口 | 自动变为标签页 |
| Ctrl+Shift 强制新窗口 | 按住 Ctrl+Shift 打开路径 | 保持新窗口 |
| 复制标签页 | Ctrl+D | 当前标签页被复制 |
| 重开已关闭 | Shift+Ctrl+T | 最近关闭的标签页恢复 |
| 标签页搜索 | 配置快捷键后触发 | 搜索弹窗出现 |
| 鼠标双击向上一级 | 配置鼠标 Profile 后双击空白处 | 导航到上级目录 |
| 开机启动 | 右键托盘 → 勾选开机启动 | 注册表 Run 键添加 |
| 设置保存 | 修改设置后关闭重启 | 设置保留 |

### 4.3 界面完整性测试

| 测试项 | 检查内容 |
|--------|----------|
| 主窗口标题 | 显示"资源管理器标签页工具"（中文）或"Explorer Tab Utility"（英文） |
| 导航菜单 | "快捷键"/"偏好设置"/"关于"（中文） |
| 按钮文本 | "新建"/"导入"/"导出"/"保存"（中文） |
| 状态栏 | "窗口监控"/"复用标签页"/"快捷键监控"/"鼠标监控"（中文） |
| 托盘菜单 | "快捷键监控"/"鼠标监控"/"窗口监控"/"复用标签页"/"开机启动"/"检查更新"/"设置"/"退出"（中文） |
| 偏好设置 | "应用设置"/"自动更新"/"我有主题兼容问题"/"保存已关闭历史"/"恢复上次窗口"/"隐藏托盘图标"（中文） |
| 快捷键配置 | "拦截"/"标签页"/"延迟"（中文） |
| 关于页面 | "增强您的 Windows 资源管理器体验"/"支持本项目"/"开发者"/"当前支持者"（中文） |
| 消息框按钮 | "确定"/"取消"/"是"/"否"（中文） |
| ToolTip | 所有鼠标悬停提示为中文 |

### 4.4 兼容性测试

| 测试项 | 环境 | 预期结果 |
|--------|------|----------|
| Windows 11 26H1 | Build 28000.1836 | 功能正常 |
| .NET 9 运行时 | 已安装 | 程序启动正常 |
| .NET Framework 4.8.1 | 已安装 | 程序启动正常 |
| 高 DPI 显示 | 150% 缩放 | 界面正常显示 |
| 深色模式 | Windows 深色主题 | 界面正常显示 |

---

## 五、已知问题与限制

### 5.1 HotKeyAction 枚举未汉化

**原因：** `DescriptionAttribute` 在 XAML ComboBox 中通过 `EnumDescriptionConverter` 显示，需要运行时读取资源。当前实现中 `Description` 属性是编译时常量，无法直接绑定到 .resx。

**解决方案：** 修改 `EnumDescriptionConverter` 或在 `HotKeyProfileControl` 中手动映射枚举值到资源字符串。

**影响：** 快捷键配置页面的 Action 下拉框仍显示英文。

### 5.2 安装脚本未汉化

**原因：** `installer.iss` 使用 Inno Setup 编译，需要 Inno Setup 编译器处理。

**解决方案：** 在 `[Languages]` 段添加中文语言支持，或手动替换安装界面文本。

**影响：** 安装程序界面仍为英文。

### 5.3 语言切换需重启

**原因：** WPF 的 `XmlLanguage` 和 `ResourceManager` 在启动时初始化，运行时切换需要刷新所有 UI 元素。

**解决方案：** 当前实现为重启生效。如需实时切换，需重构为 MVVM 模式并使用数据绑定。

**影响：** 用户切换语言后需重启程序。

---

## 六、回滚方法

```bash
# 回滚到原始版本
git checkout master

# 或保留分支但回滚单个文件
git checkout master -- ExplorerTabUtility/UI/Views/MainWindow.xaml
```

所有改动在 `feature/zh-cn-localization` 分支上，master 分支保持原样。

---

## 七、后续可选优化

| 优化项 | 优先级 | 说明 |
|--------|--------|------|
| HotKeyAction 枚举汉化 | 中 | 修改 EnumDescriptionConverter 支持资源绑定 |
| 安装脚本汉化 | 低 | 添加 Inno Setup 中文语言支持 |
| 实时语言切换 | 低 | 重构为 MVVM 模式，使用数据绑定 |
| 更多语言 | 低 | 添加 ja-KR、ko-KR 等 .resx 文件 |
| About 页面支持按钮文本 | 低 | GitHub Sponsors/Patreon 等品牌名保持英文 |

---

## 八、文件变更统计

| 类型 | 数量 |
|------|------|
| 新增文件 | 4 |
| 修改文件 | 13 |
| 新增代码行 | ~350 |
| 修改代码行 | ~100 |
| 资源键值对 | 120+ |
| Git 提交 | 1 |

---

## 九、构建环境要求

### 最低要求
- Windows 10/11
- .NET 9 SDK 或 .NET Framework 4.8.1 SDK
- Visual Studio 2022（推荐）或 `dotnet` CLI

### 推荐配置
- Visual Studio 2022 17.8+
- .NET 9 SDK
- Windows 11 SDK
- Inno Setup 6+（如需编译安装程序）

### 下载链接
- .NET 9 SDK: https://dotnet.microsoft.com/download/dotnet/9.0
- Visual Studio 2022: https://visualstudio.microsoft.com/
- Inno Setup: https://jrsoftware.org/isinfo.php
