# Explorer Tab Utility（资源管理器标签页工具）

> [!TIP]
> 强制将 Windows 11 中新打开的文件资源管理器窗口转换为标签页，让您的工作流更整洁、更有序！

<div align="center">
  <img src="https://cdn.jsdelivr.net/gh/w4po/ExplorerTabUtility@master/Assets/ExplorerTabUtilityLogo.gif" alt="Explorer Tab Utility Logo">
  
  [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  [![Windows 11](https://img.shields.io/badge/Windows%2011-22H2+-blue.svg)](https://www.microsoft.com/windows/windows-11)
  [![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
  [![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8.1-purple.svg)](https://dotnet.microsoft.com/download/dotnet-framework)
</div>

> [!IMPORTANT]
> 本程序需要 Windows 11（版本 22H2，Build 22621 或更高版本）并启用文件资源管理器标签页功能。

## 为什么需要 Explorer Tab Utility？

<div align="center">
  <img src="https://cdn.jsdelivr.net/gh/w4po/ExplorerTabUtility@master/Assets/TheWhy.png" alt="为什么使用 Explorer Tab Utility">
</div>

告别杂乱的多窗口桌面！Explorer Tab Utility 自动将新窗口转换为标签页，提供更整洁、更有序的文件管理体验。

## 功能特性

### 自动窗口转标签页

- 无缝将新打开的资源管理器窗口转换为标签页
- 如果路径已打开，自动切换到已有标签页
- 支持虚拟桌面切换（通过快捷键）
- 支持分离/附加标签页
- 优雅处理"在文件夹中显示"的文件选择
- 支持同时打开多个标签页

### 复制当前标签页

- 快速复制当前标签页/窗口
- 可选择作为标签页或新窗口复制（切换"标签页"选项）
- 保留当前位置和选中的文件

### 重新打开已关闭标签页

- 重新打开之前关闭的标签页/窗口
- 可选择作为标签页或新窗口打开
- 恢复确切的位置和选中的文件
- 历史记录可跨程序重启保存（在设置中启用"保存已关闭历史"）

### 恢复上次窗口

- 在文件资源管理器重启/崩溃或系统重启后自动恢复之前打开的窗口
- 可通过"恢复上次窗口"设置配置
- 首次启动时确认对话框可选择是否恢复窗口

### 分离与贴靠窗口

- 将当前标签页分离为新窗口
- 将窗口贴靠到屏幕边缘（上/下/左/右）
- 可用单个快捷键串联多个操作
- 示例设置（Ctrl + Q）：
  1. 第一个配置：分离当前标签页
  2. 第二个配置：将原窗口贴靠到左侧
  3. 第三个配置：将新窗口贴靠到右侧（可自定义延迟）

### 后退、前进、向上导航

- 使用以下方式在资源管理器中导航：
  - 键盘快捷键（可自定义）
  - 在文件夹空白区域点击鼠标
- 在目录间快速导航

### 自定义路径导航

- 分配快捷键快速打开常用位置
- 支持多种格式：
  - 标准路径：`C:\Users\Documents`
  - 环境变量：`%USERPROFILE%\Downloads`
  - Windows CLSID 路径：特殊文件夹
  - 程序和文件：`C:\file.txt`
  - 网址：`https://github.com`（在默认浏览器中打开）

### 标签页搜索/切换器

- 快速查找和切换已打开的标签页/窗口
- 输入文件夹名称或路径的一部分进行搜索
- 键盘导航：上下箭头选择，回车确认
- 特殊修饰键：
  - 默认：切换到已有标签页或在新标签页中打开
  - Shift：在新窗口中打开
  - Ctrl：复制标签页

### 性能与可靠性

- 轻量级，资源占用低
- 快速响应的标签页创建
- 基于 COM 的稳定实现
- 可靠的窗口状态管理

## 快速开始

1. 从 [Releases](https://github.com/w4po/ExplorerTabUtility/releases) 页面下载最新版本，或通过 `winget` 或 `choco` 安装：
    ```powershell
    winget install w4po.ExplorerTabUtility --interactive
    ```
    ```powershell
    choco install explorertabutility --params "/interactive"
    ```
2. 运行程序
3. 查看系统托盘图标，即可开始使用！

## 配置说明

> [!NOTE]
> 程序默认以最小化方式运行在系统托盘中。
> 要进行配置，双击或右键托盘图标。

### 通用设置

- **窗口监控**：启用/禁用自动窗口转标签页
- **复用标签页**：切换到已有标签页而非打开重复标签
- **快捷键监控**：启用/禁用键盘快捷键
- **鼠标监控**：启用/禁用鼠标导航功能
- **开机启动**：配置随 Windows 自动启动
- **设置持久化**：
  * 设置保存在 AppData 文件夹中的 JSON 文件：
  ```
  %APPDATA%\ExplorerTabUtility\settings.json
  ```
  如需重置为默认设置，只需删除 settings.json 文件。

### 偏好设置

- **自动更新**：启动时自动检查更新
- **我有主题兼容问题**：使用替代的窗口隐藏方法，保留自定义资源管理器主题
- **保存已关闭历史**：保存已关闭窗口的记录
- **恢复上次窗口**：在重启或崩溃后恢复之前打开的窗口
- **隐藏托盘图标**：隐藏系统托盘图标

### 快捷键配置管理

每个配置包含：
1. 基本配置：快捷键、作用范围、动作类型、路径
2. 高级设置：执行延迟、按键拦截、配置删除

> [!TIP]
> 使用"拦截"开关可以防止或允许快捷键传递给其他应用。

> [!NOTE]
> "设为目标窗口"动作可以让您选择哪个资源管理器窗口接收新标签页。这在有多个资源管理器窗口或在不同虚拟桌面上工作时很有用。

## 卸载

- **如果使用安装程序安装**：使用标准 Windows 卸载程序
- **如果通过 winget 安装**：
  ```powershell
  winget uninstall w4po.ExplorerTabUtility
  ```
- **如果通过 Chocolatey 安装**：
  ```powershell
  choco uninstall explorertabutility
  ```
- **如果使用便携版**：直接删除应用程序文件夹

## 杀毒软件检测

> [!WARNING]
> 本工具可能被杀毒软件标记为可疑。这是**误报**，原因是：
> - COM 交互（用于文件资源管理器标签页管理）
> - 低级键盘和鼠标钩子（用于快捷键支持）
>
> 该工具完全开源，您可以：
> - 在本仓库中查看源代码
> - 使用 Visual Studio 自行构建
> - 验证其安全性和功能

## 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件。

## 致谢

本项目使用了以下优秀的开源包：

- **[H.Hooks](https://github.com/HavenDV/H.Hooks)** - 高效可靠的键盘钩子实现
- **[Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon)** - 基于 WPF 的现代系统托盘图标实现

## 贡献

欢迎贡献！请随时提交 issue 和 pull request。

---

**其他语言版本：** [English](README.md)
