# 安全说明

ExplorerTabUtility zh-CN 是基于开源项目 ExplorerTabUtility 的非官方简体中文维护版，用于增强 Windows 11 文件资源管理器标签页体验。

## 程序会做什么

- 监控新打开的文件资源管理器窗口，并按用户设置将其合并为标签页。
- 提供键盘快捷键和鼠标快捷键，用于打开、复制、搜索、重开或导航标签页。
- 通过 Windows Shell/Explorer COM 接口读取当前资源管理器窗口、路径、标签页和选中文件。
- 在用户启用开机启动后，仅写入当前用户的 `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` 项。
- 在用户启用自动更新后，检查本汉化仓库的公开 GitHub Releases。

## 为什么会使用敏感系统接口

本工具的核心功能依赖资源管理器窗口和标签页状态，因此需要使用 `Shell32`、`SHDocVw`、`ShellWindows` 等 Windows Shell 接口。

快捷键和鼠标导航功能需要监听用户配置的快捷键组合，因此会使用低级键盘/鼠标 Hook。程序只判断按键或鼠标动作是否匹配用户配置，不记录键盘输入内容，不保存输入文本。

## 程序不会做什么

- 不创建 VBS 启动脚本。
- 不创建计划任务。
- 不创建 Startup 文件夹快捷方式作为默认启动方式。
- 主程序不隐藏运行。
- 不远程控制计算机。
- 不收集、不读取、不上传用户文件内容。
- 不上传键盘输入、文件列表、路径历史或个人数据。
- 不使用混淆、加壳、隐藏脚本或绕过安全软件的手段。

## 自动更新说明

自动更新只检查公开的 GitHub Releases API：

`https://api.github.com/repos/ysbushe/ExplorerTabUtility-zh-cn/releases/latest`

更新包应来自本汉化维护仓库。建议用户通过仓库源码、GitHub Actions 构建记录、Release 页面和文件哈希确认程序来源。

## 安全软件误报

由于本工具使用全局键鼠 Hook、Explorer/Shell COM 控制和可选开机启动，部分安全软件可能将其误报为可疑程序。

如果遇到误报，建议：

- 不要从未知来源下载可执行文件。
- 优先使用本仓库 GitHub Actions 或 Release 产物。
- 记录安全软件报毒名称、文件 SHA256、下载来源和构建记录。
- 向安全软件厂商提交误报申诉，并附上本仓库地址和源码说明。

本项目不会建议用户关闭、绕过或欺骗安全软件。
