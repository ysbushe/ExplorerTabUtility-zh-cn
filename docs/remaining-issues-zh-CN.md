# ExplorerTabUtility zh-CN 遗留问题清单

> 日期：2026-06-15
> 版本：feature/zh-cn-localization (07a95a2)

---

## 一、已解决

| # | 问题 | 解决方案 |
|---|------|----------|
| 1 | Action 下拉框显示英文 | EnumDescriptionConverter 支持 Action_ 资源键 |
| 2 | Scope 下拉框显示英文 | EnumDescriptionConverter 支持 Scope_ 资源键 |
| 3 | 鼠标监控灰色（未启用） | 新增偏好设置开关自动启用 |
| 4 | 空白处双击无反应 | 新增专用开关自动创建 Profile |
| 5 | Res 类为 internal | 改为 public |
| 6 | XAML 无法绑定 Res | 类和属性改为 public |

---

## 二、尚未解决

| # | 问题 | 影响 | 优先级 |
|---|------|------|--------|
| 1 | 默认 Profile 名称仍为英文 | 界面显示 Home/Duplicate/ReopenClosed | 低 |
| 2 | 安装器未汉化 | 安装界面仍为英文 | 低 |

---

## 三、暂缓处理

| # | 问题 | 原因 |
|---|------|------|
| 1 | 实时语言切换 | 需大规模重构 MVVM |
| 2 | 多语言扩展 | 当前只需中文 |
| 3 | net481 构建 | 需要 .NET Framework SDK |
| 4 | Node.js 20 deprecation | 非阻塞警告 |

---

## 四、无法验证

| # | 问题 | 原因 |
|---|------|------|
| 1 | GUI 界面文本实际显示 | CLI 环境无法观察 GUI |
| 2 | ToolTip 悬停显示 | 需要鼠标操作 |
| 3 | Action/Scope 下拉框中文 | 需要实际运行观察 |
| 4 | 空白处双击功能 | 需要 GUI 交互测试 |

---

## 五、需要用户手动测试

| # | 测试项 | 步骤 |
|---|--------|------|
| 1 | 下载最新 artifact | https://github.com/ysbushe/ExplorerTabUtility-zh-cn/actions |
| 2 | 运行程序 | 双击 ExplorerTabUtility.exe |
| 3 | 验证中文界面 | 检查所有界面文本 |
| 4 | 测试 Action 下拉框 | 打开快捷键配置，查看 Action 下拉框 |
| 5 | 测试 Scope 下拉框 | 打开快捷键配置，查看 Scope 下拉框 |
| 6 | 启用空白处双击 | 偏好设置 → 勾选"启用资源管理器空白处双击向上一级" |
| 7 | 测试双击向上一级 | 资源管理器空白处双击 |
| 8 | 测试误触发 | 双击文件夹、文件、地址栏等 |
| 9 | 测试新窗口转标签页 | 打开多个资源管理器窗口 |

---

## 六、已知限制

| 限制 | 说明 |
|------|------|
| 语言切换需重启 | 不支持实时切换 |
| HotKeyAction 枚举值 | 内部值仍为英文 |
| 品牌名保留英文 | GitHub Sponsors 等 |
| 安装器未汉化 | Inno Setup 脚本 |
| 双击间隔固定 500ms | 未使用系统双击时间 |
| 空白处检测依赖 IAccessible | 可能受 Windows 版本影响 |
