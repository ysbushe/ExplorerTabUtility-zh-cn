# ExplorerTabUtility zh-CN 英文残留修复报告

> 日期：2026-06-15
> 分支：feature/zh-cn-localization
> Commit：07a95a2

---

## 一、修复内容

### 1. Action 下拉框汉化

**修改文件：** `UI/Converters/EnumDescriptionConverter.cs`

**修复方案：**
- 修改 `EnumDescriptionConverter` 支持 `HotKeyAction` 和 `HotkeyScope` 两种枚举类型
- 根据枚举类型名自动选择资源键前缀：`Action_` 或 `Scope_`
- 使用 `CultureInfo.CurrentUICulture` 确保正确加载本地化资源

**资源键（已添加到 .resx）：**

| Key | 英文 | 中文 |
|-----|------|------|
| Action_Open | Open | 打开 |
| Action_Duplicate | Duplicate | 复制标签页 |
| Action_ReopenClosed | Reopen Closed | 重新打开已关闭标签页 |
| Action_TabSearch | Tab Search | 标签页搜索 |
| Action_NavigateBack | Navigate Back | 后退 |
| Action_NavigateForward | Navigate Forward | 前进 |
| Action_NavigateUp | Navigate Up | 向上一级 |
| Action_SetTargetWindow | Set Target Window | 设为目标窗口 |
| Action_ToggleWinHook | Toggle Window Hook | 切换窗口监控 |
| Action_ToggleReuseTabs | Toggle Reuse Tabs | 切换复用标签页 |
| Action_ToggleVisibility | Toggle Visibility | 显示/隐藏主窗口 |
| Action_DetachTab | Detach Tab | 分离标签页 |
| Action_SnapRight | Snap Right | 贴靠到右侧 |
| Action_SnapLeft | Snap Left | 贴靠到左侧 |
| Action_SnapUp | Snap Up | 贴靠到顶部 |
| Action_SnapDown | Snap Down | 贴靠到底部 |

### 2. Scope 下拉框汉化

**资源键（已添加到 .resx）：**

| Key | 英文 | 中文 |
|-----|------|------|
| ScopeGlobal | Global | 全局 |
| ScopeFileExplorer | File Explorer | 资源管理器内 |

### 3. "空白处双击向上一级"开关

**修改文件：**
- `Managers/SettingsManager.cs` — 新增 `DoubleClickEmptySpace` 设置项
- `UI/Views/MainWindow.xaml` — 偏好设置页面新增开关
- `UI/Views/MainWindow.xaml.cs` — 开关逻辑实现

**功能说明：**
- 勾选时：自动创建专用鼠标 Profile（双击左键 + NavigateUp + FileExplorer）
- 自动启用 MouseHook
- 取消勾选时：禁用专用 Profile，保留其他自定义配置
- 内部使用固定 GUID 标识专用 Profile，避免重复创建

### 4. 资源文件更新

**Strings.resx（英文）新增：**
- `GrpMouseFeatures`: Mouse Features
- `CbDoubleClickEmptySpace`: Double-click empty space to navigate up
- `TipDoubleClickEmptySpace`: When enabled, double-clicking empty space in File Explorer navigates up one directory level.

**Strings.zh-CN.resx（中文）新增：**
- `GrpMouseFeatures`: 鼠标功能
- `CbDoubleClickEmptySpace`: 启用资源管理器空白处双击向上一级
- `TipDoubleClickEmptySpace`: 启用后，在资源管理器文件列表空白处双击鼠标左键可返回上一级目录。

---

## 二、修改文件清单

| 文件 | 改动类型 |
|------|----------|
| `UI/Converters/EnumDescriptionConverter.cs` | 重构：支持 Action_ 和 Scope_ 资源键 |
| `Strings/Strings.resx` | 新增 Scope 和鼠标功能资源键 |
| `Strings/Strings.zh-CN.resx` | 新增中文翻译 |
| `Strings/Strings.Designer.cs` | 新增属性 |
| `Managers/SettingsManager.cs` | 新增 DoubleClickEmptySpace 属性 |
| `UI/Views/MainWindow.xaml` | 新增鼠标功能 GroupBox |
| `UI/Views/MainWindow.xaml.cs` | 新增开关逻辑和 Profile 管理 |

---

## 三、是否修改核心功能逻辑

**否。** 本轮修改仅涉及：
- UI 显示文本的本地化
- 新增偏好设置选项
- 新增内置 Profile 管理逻辑

未修改以下核心逻辑：
- `ExplorerWatcher.cs`（窗口转标签页）
- `Mouse.cs`（鼠标 Hook 底层）
- `Keyboard.cs`（键盘 Hook 底层）
- `Helper.IsExplorerEmptySpace()`（空白处检测）
- `HookManager.cs`（Hook 编排）

---

## 四、GitHub Actions 构建状态

| 项目 | 值 |
|------|-----|
| Run ID | 27523437172 |
| 状态 | ✅ 成功 |
| 耗时 | 1分28秒 |
| Commit | 07a95a2 |

---

## 五、需要用户验证的项目

1. Action 下拉框是否显示中文
2. Scope 下拉框是否显示中文
3. "启用资源管理器空白处双击向上一级"开关是否出现在偏好设置页面
4. 勾选开关后是否自动创建专用鼠标 Profile
5. 空白处双击是否能触发向上一级
6. 是否存在误触发

---

## 六、需要重新下载 artifact

**是。** Artifact 名称：`ExplorerTabUtility-zh-CN-net9-windows`
下载地址：https://github.com/ysbushe/ExplorerTabUtility-zh-cn/actions
