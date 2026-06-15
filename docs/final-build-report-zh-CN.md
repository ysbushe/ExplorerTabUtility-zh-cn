# ExplorerTabUtility zh-CN 最终构建报告

> 日期：2026-06-15
> 仓库：https://github.com/ysbushe/ExplorerTabUtility-zh-cn
> 分支：feature/zh-cn-localization

---

## 一、GitHub Actions 构建状态

| 项目 | 值 |
|------|-----|
| Workflow 名称 | Build zh-CN Localization |
| Run 编号 | 27519555873 |
| 状态 | ✅ 成功 (success) |
| 分支 | feature/zh-cn-localization |
| Commit | e50d101 |
| 触发方式 | push |
| 创建时间 | 2026-06-15T01:58:12Z |
| 完成时间 | 2026-06-15T01:59:48Z |
| 构建耗时 | 1分36秒 |

---

## 二、Artifact 信息

| 项目 | 值 |
|------|-----|
| 名称 | ExplorerTabUtility-zh-CN-net9-windows |
| 大小 | 911,252 字节 (~890 KB) |
| 下载地址 | https://github.com/ysbushe/ExplorerTabUtility-zh-cn/actions |
| 保留时间 | 30 天（至 2026-07-15） |
| 是否可下载 | ✅ 是 |

---

## 三、Artifact 解压文件清单

| 文件 | 大小 | 说明 |
|------|------|------|
| ExplorerTabUtility.exe | 296,960 B | 主程序可执行文件 |
| ExplorerTabUtility.dll | 560,128 B | 主程序库 |
| ExplorerTabUtility.pdb | 101,632 B | 调试符号 |
| ExplorerTabUtility.deps.json | 3,637 B | 依赖配置 |
| ExplorerTabUtility.runtimeconfig.json | 516 B | 运行时配置 |
| zh-CN/ExplorerTabUtility.resources.dll | 11,776 B | **中文卫星程序集** |
| AutoUpdater.NET.dll | 255,488 B | 自动更新库 |
| AutoUpdater.NET.Markdown.dll | 17,408 B | Markdown 支持 |
| H.Hooks.dll | 79,360 B | 键盘/鼠标钩子 |
| Hardcodet.NotifyIcon.Wpf.dll | 123,904 B | 系统托盘 |
| Markdig.dll | 474,624 B | Markdown 解析 |

---

## 四、zh-CN 卫星程序集检查

| 检查项 | 结果 |
|--------|------|
| zh-CN 文件夹是否存在 | ✅ 是 |
| ExplorerTabUtility.resources.dll 是否存在 | ✅ 是 |
| 文件大小 | 11,776 字节 |
| 编译时间 | 2026-06-15 10:00 |

---

## 五、本机运行测试

### 5.1 运行环境

| 项目 | 值 |
|------|-----|
| 操作系统 | Windows 11 专业版 26H1 |
| .NET 运行时 | Microsoft.WindowsDesktop.App 9.0.3 |
| 测试目录 | D:\Test |

### 5.2 运行结果

| 测试项 | 结果 |
|--------|------|
| 程序能否启动 | ✅ 是（PID 1004） |
| 托盘图标是否出现 | ✅ 是（任务栏可见） |
| 设置窗口能否打开 | ✅ 是（双击托盘图标） |
| 程序退出是否正常 | ✅ 是（正常关闭） |
| 退出后资源管理器是否正常 | ✅ 是（无异常） |

### 5.3 settings.json 检查

| 字段 | 值 |
|------|-----|
| Language | zh-CN |
| WindowHook | true |
| ReuseTabs | true |
| KeyboardHook | true |
| MouseHook | false |
| IsFirstRun | false |

---

## 六、运行时依赖

| 组件 | 要求 | 本机状态 |
|------|------|----------|
| .NET 9 Desktop Runtime | 必需 | ✅ 已安装 9.0.3 |
| .NET SDK | 不需要 | - |
| Visual Studio | 不需要 | - |

### 安装 .NET 9 Desktop Runtime

```powershell
winget install Microsoft.DotNet.DesktopRuntime.9
```

或从 https://dotnet.microsoft.com/download/dotnet/9.0 手动下载。

---

## 七、语言设置方法

### 方法一：通过设置界面（推荐）

1. 双击托盘图标打开设置窗口
2. 切换到"偏好设置"页面
3. 在"界面语言"下拉框中选择"简体中文"
4. 关闭程序并重新启动

### 方法二：手动编辑

1. 打开 `%APPDATA%\ExplorerTabUtility\settings.json`
2. 修改 `"Language": "zh-CN"`
3. 重启程序

---

## 八、构建修复历史

| 提交 | 问题 | 修复 |
|------|------|------|
| 59f681e | dotnet restore 参数错误 | 改为 /p:TargetFramework |
| f2cf064 | Strings 类与命名空间同名冲突 | 重命名类为 Res |
| e7f7975 | Application.Language 不存在 | 移除该行 |
| 8ce3e93 | Artifact 路径错误 | 修正为 Any CPU 路径 |
| e50d101 | Res 类为 internal，XAML 无法绑定 | 改为 public |
