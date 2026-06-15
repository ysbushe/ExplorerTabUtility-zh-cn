# 汉化版全自动更新说明

本仓库通过 GitHub Actions 自动维护简体中文版。

## 自动流程

1. 每天检查一次上游 `w4po/ExplorerTabUtility` 的最新正式 Release。
2. 发现新版本后，将上游版本自动合并到 `feature/zh-cn-localization`。
3. 对发生文本冲突的文件优先保留汉化仓库版本。
4. 使用完整 Windows MSBuild 环境构建 `net9.0-windows` x64 版本。
5. 检查主程序和 `zh-CN` 语言资源是否齐全。
6. 打包为免安装 ZIP，并发布到本仓库 Releases。
7. 已安装的汉化版程序从本仓库检查和下载更新。

## 版本规则

汉化版在上游三段版本号后增加一段修订号：

```text
上游 v2.6.0
汉化版 v2.6.0.1
```

同一个上游版本需要重新发布修复时，修订号递增为 `.2`、`.3`。

## 安全策略

- 构建失败时不会覆盖已有 Release。
- 无法自动合并或缺少中文资源时不会发布程序包。
- 失败后会自动创建 GitHub Issue，并附上工作流日志地址。
- 发布包不修改系统文件，也不包含后台服务。

## 手动触发

进入仓库的 `Actions` 页面，选择：

```text
Sync upstream and release zh-CN
```

点击 `Run workflow`。勾选 `force_release` 可以针对当前上游版本生成新的汉化修订版。

