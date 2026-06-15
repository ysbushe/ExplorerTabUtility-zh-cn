# ExplorerTabUtility zh-CN 测试报告

> 分支：`feature/zh-cn-localization`
> 日期：2026-06-15
> 仓库：https://github.com/ysbushe/ExplorerTabUtility-zh-cn

---

## 一、GitHub Actions 构建结果

| 项目 | 状态 |
|------|------|
| 构建结果 | ✅ 成功 |
| 构建时间 | 2026-06-15 01:47 UTC |
| 工作流运行 ID | 27519204346 |
| 构建目标 | net9.0-windows (Release) |
| Runner | windows-latest |
| 警告 | 1（Node.js 20 已弃用，不影响构建） |
| 错误 | 0 |

### 构建步骤详情

| 步骤 | 状态 |
|------|------|
| Checkout code | ✅ |
| Setup MSBuild | ✅ |
| Setup .NET 9 SDK | ✅ |
| Restore NuGet packages | ✅ |
| Build net9.0-windows | ✅ |
| List build output | ✅ |
| Upload build artifact | ✅ |
| Post Setup .NET 9 SDK | ✅ |
| Post Checkout code | ✅ |

---

## 二、Artifact 信息

| 项目 | 值 |
|------|-----|
| 名称 | `ExplorerTabUtility-zh-CN-net9-windows` |
| 下载位置 | https://github.com/ysbushe/ExplorerTabUtility-zh-cn/actions |
| 路径 | Actions → Build zh-CN Localization → Artifacts → ExplorerTabUtility-zh-CN-net9-windows |
| 保留时间 | 30 天 |
| 包含文件 | ExplorerTabUtility.exe、ExplorerTabUtility.dll、zh-CN 卫星程序集、依赖 DLL |

### Artifact 内容

```
ExplorerTabUtility/bin/Any CPU/Release/net9.0-windows/
├── ExplorerTabUtility.exe          # 主程序
├── ExplorerTabUtility.dll          # 主程序库
├── ExplorerTabUtility.pdb          # 调试符号
├── zh-CN/
│   └── ExplorerTabUtility.resources.dll  # 中文资源卫星程序集
├── AutoUpdater.NET.dll             # 自动更新
├── H.Hooks.dll                     # 键盘/鼠标钩子
├── Hardcodet.NotifyIcon.Wpf.dll    # 系统托盘
├── Markdig.dll                     # Markdown 解析
└── ... (其他依赖)
```

---

## 三、本机运行测试

### 3.1 运行环境要求

| 组件 | 要求 |
|------|------|
| 操作系统 | Windows 10/11 |
| 运行时 | .NET 9 Desktop Runtime |
| 架构 | x64 |

### 3.2 运行步骤

```powershell
# 1. 安装 .NET 9 Desktop Runtime（如未安装）
winget install Microsoft.DotNet.DesktopRuntime.9

# 2. 下载 artifact ZIP 并解压

# 3. 运行程序
cd ExplorerTabUtility
.\ExplorerTabUtility.exe
```

### 3.3 语言设置

**方法一：通过设置界面**

1. 双击托盘图标打开设置窗口
2. 切换到"偏好设置"页面
3. 在"语言"下拉框中选择"简体中文"
4. 关闭程序并重新启动

**方法二：手动编辑 settings.json**

1. 打开 `%APPDATA%\ExplorerTabUtility\settings.json`
2. 修改或添加 `"Language": "zh-CN"`
3. 保存文件
4. 重启程序

### 3.4 语言切换后提示

选择语言后，程序会弹出提示框："语言设置将在重启程序后生效。"

---

## 四、中文界面验证清单

| 界面区域 | 预期中文文本 | 状态 |
|----------|--------------|------|
| 主窗口标题 | 资源管理器标签页工具 | ⏳ 待验证 |
| 导航菜单 | 快捷键 / 偏好设置 / 关于 | ⏳ 待验证 |
| 按钮 | 新建 / 导入 / 导出 / 保存 | ⏳ 待验证 |
| 状态栏 | 窗口监控 / 复用标签页 / 快捷键监控 / 鼠标监控 | ⏳ 待验证 |
| 托盘菜单 | 快捷键监控 / 鼠标监控 / 窗口监控 / 复用标签页 / 开机启动 / 检查更新 / 设置 / 退出 | ⏳ 待验证 |
| 偏好设置 | 应用设置 / 自动更新 / 我有主题兼容问题 / 保存已关闭历史 / 恢复上次窗口 / 隐藏托盘图标 | ⏳ 待验证 |
| 语言设置 | 语言 / 界面语言 / Auto / English / 简体中文 | ⏳ 待验证 |
| 快捷键配置 | 拦截 / 标签页 / 延迟 | ⏳ 待验证 |
| 关于页面 | 增强您的 Windows 资源管理器体验 / 支持本项目 / 开发者 / 当前支持者 | ⏳ 待验证 |
| 消息框按钮 | 确定 / 取消 / 是 / 否 | ⏳ 待验证 |
| ToolTip | 所有鼠标悬停提示为中文 | ⏳ 待验证 |
| HotKeyAction 下拉框 | 打开 / 复制标签页 / 重开已关闭 / 等 | ⏳ 待验证 |

> 注：标记为"待验证"的项目需要在本机运行程序后手动验证。

---

## 五、功能回归测试

### 5.1 核心功能 A：新窗口自动转标签页

| 测试项 | 步骤 | 预期结果 | 状态 |
|--------|------|----------|------|
| 窗口监控开关 | 右键托盘 → 勾选/取消窗口监控 | 状态栏绿灯亮/灭 | ⏳ 待验证 |
| 复用标签页开关 | 右键托盘 → 勾选/取消复用标签页 | 状态栏绿灯亮/灭 | ⏳ 待验证 |
| 新窗口转标签 | 打开新的资源管理器窗口 | 自动变为当前窗口的标签页 | ⏳ 待验证 |
| 已打开路径复用 | 再次打开已打开的路径 | 切换到已有标签页 | ⏳ 待验证 |
| Ctrl+Shift 强制新窗口 | 按住 Ctrl+Shift 打开路径 | 保持新窗口 | ⏳ 待验证 |
| 设为目标窗口 | 配置 SetTargetWindow 快捷键后触发 | 指定窗口接收新标签 | ⏳ 待验证 |

### 5.2 核心功能 B：空白处双击向上一级

| 测试项 | 步骤 | 预期结果 | 状态 |
|--------|------|----------|------|
| 鼠标双击向上一级 | 配置鼠标 Profile（双击左键 + NavigateUp + FileExplorer）后双击空白处 | 导航到上级目录 | ⏳ 待验证 |
| 双击文件夹 | 双击文件夹 | 打开文件夹 | ⏳ 待验证 |
| 双击文件 | 双击文件 | 打开文件 | ⏳ 待验证 |
| 右键菜单 | 右键空白处 | 弹出右键菜单 | ⏳ 待验证 |
| 地址栏双击 | 双击地址栏 | 编辑路径 | ⏳ 待验证 |
| 侧边栏双击 | 双击侧边栏 | 无误触发 | ⏳ 待验证 |

### 5.3 语言切换测试

| 测试项 | 步骤 | 预期结果 | 状态 |
|--------|------|----------|------|
| Auto 模式 | 删除 settings.json，启动程序 | 根据系统语言显示对应界面 | ⏳ 待验证 |
| 强制中文 | 设置 Language 为 "zh-CN"，重启 | 界面显示中文 | ⏳ 待验证 |
| 强制英文 | 设置 Language 为 "en-US"，重启 | 界面显示英文 | ⏳ 待验证 |
| 语言持久化 | 切换语言后重启 | 语言设置保留 | ⏳ 待验证 |

---

## 六、兼容性测试

| 测试项 | 环境 | 预期结果 | 状态 |
|--------|------|----------|------|
| Windows 11 26H1 | Build 28000.1836 | 功能正常 | ⏳ 待验证 |
| .NET 9 运行时 | 已安装 | 程序启动正常 | ⏳ 待验证 |
| 高 DPI 显示 | 150% 缩放 | 界面正常显示 | ⏳ 待验证 |

---

## 七、已知问题与限制

| 问题 | 影响 | 解决方案 |
|------|------|----------|
| HotKeyAction 枚举描述未汉化 | 快捷键配置页面的 Action 下拉框仍显示英文 | 需修改 EnumDescriptionConverter 支持资源绑定 |
| 安装脚本未汉化 | 安装程序界面仍为英文 | 需添加 Inno Setup 中文语言支持 |
| 语言切换需重启 | 切换语言后需重启程序 | 当前设计如此，如需实时切换需重构 |

---

## 八、构建修复历史

| 提交 | 问题 | 修复方案 |
|------|------|----------|
| 27518360210 | `dotnet restore -f` 参数错误 | 改为 `/p:TargetFramework=net9.0-windows` |
| 27518410945 | `Strings` 类找不到 | 重命名类为 `Res`，避免命名空间冲突 |
| 27518822428 | 重复 Compile 项 | 改回 `Compile Update`（非 `Compile Include`） |
| 27519025771 | `Application.Language` 不存在 | 移除该行，仅保留线程文化设置 |
| 27519107093 | Artifact 路径错误 | 修正为 `bin/Any CPU/Release/net9.0-windows/` |

---

## 九、后续待办

| 优先级 | 任务 | 说明 |
|--------|------|------|
| 高 | 本机运行测试 | 下载 artifact 并在 Windows 11 上运行 |
| 高 | 中文界面验证 | 检查所有界面元素是否正确显示中文 |
| 高 | 核心功能 A 测试 | 验证新窗口自动转标签页功能 |
| 高 | 核心功能 B 测试 | 验证空白处双击向上一级功能 |
| 中 | HotKeyAction 汉化 | 修改 EnumDescriptionConverter 支持资源绑定 |
| 低 | 安装脚本汉化 | 添加 Inno Setup 中文语言支持 |
| 低 | 语言切换提示优化 | 可选：实现重启确认对话框 |

---

## 十、下载链接

- **GitHub 仓库**: https://github.com/ysbushe/ExplorerTabUtility-zh-cn
- **构建产物**: https://github.com/ysbushe/ExplorerTabUtility-zh-cn/actions
- **分支**: `feature/zh-cn-localization`
