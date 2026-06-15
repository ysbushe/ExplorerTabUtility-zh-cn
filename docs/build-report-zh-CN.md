# ExplorerTabUtility zh-CN 构建报告

> 分支：`feature/zh-cn-localization`
> 工作流：`.github/workflows/build.yml`
> 日期：2026-06-15

---

## 一、构建配置

| 项目 | 值 |
|------|-----|
| 工作流文件 | `.github/workflows/build.yml` |
| 触发条件 | push 到 `feature/zh-cn-localization` 或手动触发 |
| Runner | `windows-latest` |
| .NET SDK | 9.0.x |
| MSBuild | Visual Studio 2022 (17.0+) |
| 构建配置 | Release |
| 目标框架 | net9.0-windows |
| 输出目录 | `ExplorerTabUtility/bin/Release/net9.0-windows/` |

---

## 二、构建步骤

```
1. Checkout 代码（feature/zh-cn-localization 分支）
2. 安装 MSBuild（Visual Studio 2022）
3. 安装 .NET 9 SDK
4. 还原 NuGet 包（dotnet restore -f net9.0-windows）
5. 构建项目（msbuild /p:TargetFramework=net9.0-windows）
6. 列出构建输出
7. 上传 artifact
```

---

## 三、构建结果

### 3.1 状态

| 项目 | 状态 |
|------|------|
| 构建结果 | 待首次运行后更新 |
| 构建目标框架 | net9.0-windows |
| 是否构建 net481 | ❌ 否（见原因说明） |

### 3.2 Artifact

| 项目 | 值 |
|------|-----|
| 名称 | `ExplorerTabUtility-zh-CN-net9-windows` |
| 下载位置 | GitHub Actions → Build workflow → Artifacts |
| 保留时间 | 30 天 |

### 3.3 下载步骤

1. 打开 GitHub 仓库页面
2. 点击 **Actions** 标签
3. 点击最新的 **Build zh-CN Localization** 工作流运行
4. 在 **Artifacts** 区域点击 **ExplorerTabUtility-zh-CN-net9-windows** 下载
5. 解压 ZIP 文件

---

## 四、未构建 net481 的原因

| 原因 | 说明 |
|------|------|
| COM 引用 | net481 目标依赖 Shell32、SHDocVw COM 引用，需要完整的 MSBuild 环境 |
| 构建复杂度 | net481 需要额外的 .NET Framework SDK 和 Windows SDK |
| 优先级 | 用户明确要求优先构建 net9.0-windows，net481 可暂缓 |
| 运行时依赖 | net9.0-windows 版本只需安装 .NET 9 Desktop Runtime 即可运行 |

**后续添加 net481 的方法：**

在 `build.yml` 中添加矩阵构建：

```yaml
strategy:
  matrix:
    framework: ['net9.0-windows', 'net481']
```

并修改构建命令：

```yaml
- name: Build
  run: |
    msbuild ExplorerTabUtility/ExplorerTabUtility.csproj `
      /p:Configuration=Release `
      /p:TargetFramework=${{ matrix.framework }} `
      /restore
```

---

## 五、本机运行构建产物

### 5.1 系统要求

| 组件 | 要求 |
|------|------|
| 操作系统 | Windows 10/11 |
| 运行时 | .NET 9 Desktop Runtime |
| 架构 | x64 / x86 / arm64 |

### 5.2 安装 .NET 9 Desktop Runtime

```powershell
# 方法一：使用 winget
winget install Microsoft.DotNet.DesktopRuntime.9

# 方法二：手动下载
# 访问 https://dotnet.microsoft.com/download/dotnet/9.0
# 下载 "Desktop Runtime" for Windows x64
```

### 5.3 运行程序

```powershell
# 1. 解压 artifact ZIP 文件
# 2. 进入解压目录
cd ExplorerTabUtility

# 3. 运行程序
.\ExplorerTabUtility.exe
```

### 5.4 设置中文语言

**方法一：手动编辑 settings.json**

1. 打开 `%APPDATA%\ExplorerTabUtility\settings.json`
2. 添加或修改 `"Language": "zh-CN"`
3. 保存文件
4. 重启程序

**方法二：代码中设置（需修改源码）**

在 `App.xaml.cs` 中修改默认语言：

```csharp
LanguageManager.Initialize("zh-CN"); // 替换为 "zh-CN"
```

---

## 六、中文界面验证清单

构建成功后，请验证以下界面元素：

| 界面区域 | 预期中文文本 |
|----------|--------------|
| 主窗口标题 | 资源管理器标签页工具 |
| 导航菜单 | 快捷键 / 偏好设置 / 关于 |
| 按钮 | 新建 / 导入 / 导出 / 保存 |
| 状态栏 | 窗口监控 / 复用标签页 / 快捷键监控 / 鼠标监控 |
| 托盘菜单 | 快捷键监控 / 鼠标监控 / 窗口监控 / 复用标签页 / 开机启动 / 检查更新 / 设置 / 退出 |
| 偏好设置 | 应用设置 / 自动更新 / 我有主题兼容问题 / 保存已关闭历史 / 恢复上次窗口 / 隐藏托盘图标 |
| 快捷键配置 | 拦截 / 标签页 / 延迟 |
| 关于页面 | 增强您的 Windows 资源管理器体验 / 支持本项目 / 开发者 / 当前支持者 |
| 消息框按钮 | 确定 / 取消 / 是 / 否 |

---

## 七、常见问题

### Q: 构建失败怎么办？

A: 检查以下几点：
1. GitHub Actions 日志中查看具体错误
2. 确认 .NET 9 SDK 版本正确
3. 确认 MSBuild 可用
4. 检查 COM 引用是否正确解析

### Q: 运行时提示缺少 .NET Runtime？

A: 需要安装 .NET 9 Desktop Runtime，不是 SDK。下载地址：https://dotnet.microsoft.com/download/dotnet/9.0

### Q: 界面仍显示英文？

A: 检查：
1. settings.json 中 `"Language"` 是否为 `"zh-CN"`
2. 是否重启了程序
3. 程序是否从正确的目录运行

### Q: 如何切换回英文？

A: 将 settings.json 中 `"Language"` 改为 `"en-US"` 或 `"Auto"`，然后重启程序。

---

## 八、后续优化

| 优化项 | 说明 |
|--------|------|
| 添加 net481 构建 | 在矩阵中添加 net481 目标 |
| 添加安装程序构建 | 集成 Inno Setup 构建步骤 |
| 添加自动发布 | 构建成功后自动创建 GitHub Release |
| 多语言支持 | 添加 ja-KR、ko-KR 等语言资源 |
| 实时语言切换 | 重构为 MVVM 模式支持运行时切换 |
