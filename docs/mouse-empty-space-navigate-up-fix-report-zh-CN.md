# ExplorerTabUtility zh-CN 鼠标空白处双击向上一级修复报告

> 日期：2026-06-15
> 分支：feature/zh-cn-localization

---

## 一、问题分析

### 1.1 鼠标监控灰色状态的真实含义

底部状态栏的"鼠标监控"灰色表示 `SettingsManager.IsMouseHookActive` 为 `false`，即 Mouse Hook 未启动。

### 1.2 未启用的原因

1. **默认配置中无鼠标 Profile**：默认快捷键配置仅包含 Home（Win+E）、Duplicate（Ctrl+D）、ReopenClosed（Shift+Ctrl+T），均为键盘 Profile。
2. **MouseHook 默认为 false**：`AppSettings.MouseHook` 默认值为 `false`。
3. **用户未手动创建鼠标 Profile**：需要在快捷键页面手动添加鼠标 Profile 并启用鼠标监控。

### 1.3 空白处双击无反应的根因

1. **Mouse Hook 未启动**：因为没有鼠标 Profile 且 MouseHook 为 false。
2. **无匹配的鼠标 Profile**：即使 Mouse Hook 启动，没有配置双击左键 + NavigateUp 的 Profile 也不会触发。
3. **用户配置成本高**：需要手动创建 Profile、设置双击、选择 FileExplorer 范围、选择 NavigateUp 动作。

---

## 二、修复方案

### 2.1 新增"空白处双击向上一级"专用开关

在偏好设置页面新增一个便捷开关：

- **勾选时**：
  1. 自动创建专用鼠标 Profile（固定 GUID）
  2. Profile 配置：双击左键 + NavigateUp + FileExplorer
  3. 自动启用 MouseHook
  4. 自动保存配置

- **取消勾选时**：
  1. 禁用专用 Profile（不删除）
  2. 保留其他自定义 Profile
  3. 如果没有其他启用的鼠标 Profile，不强制关闭 MouseHook

### 2.2 专用 Profile 标识

```
ID: BuiltInDoubleClickEmptySpaceNavigateUp (固定 GUID)
名称: Double-click empty space up
触发方式: 鼠标左键双击
作用范围: FileExplorer
动作: NavigateUp
```

---

## 三、修改文件清单

| 文件 | 改动 |
|------|------|
| `Managers/SettingsManager.cs` | 新增 `DoubleClickEmptySpace` 属性 |
| `UI/Views/MainWindow.xaml` | 新增鼠标功能 GroupBox 和 CheckBox |
| `UI/Views/MainWindow.xaml.cs` | 新增 `InitializeDoubleClickEmptySpace()` 和事件处理 |
| `Strings/Strings.resx` | 新增 GrpMouseFeatures、CbDoubleClickEmptySpace、TipDoubleClickEmptySpace |
| `Strings/Strings.zh-CN.resx` | 对应中文翻译 |
| `Strings/Strings.Designer.cs` | 新增属性 |

---

## 四、IsExplorerEmptySpace 检查

当前实现（`Helper.cs:266-273`）：

```csharp
public static bool IsExplorerEmptySpace(Point point)
{
    var hr = WinApi.AccessibleObjectFromPoint(point, out var accObj, out var childId);
    if (hr != 0 || childId is not 0) return false;
    var role = accObj.get_accRole(0);
    return role is 0x21; // ROLE_SYSTEM_LIST
}
```

**分析：**
- `childId is not 0`：如果点击在子元素（文件/文件夹）上，childId 不为 0，返回 false
- `role is 0x21`：检查角色是否为 ROLE_SYSTEM_LIST（列表控件）
- Windows 11 26H1 中，资源管理器文件列表空白处应返回 ROLE_SYSTEM_LIST

**结论：** 检查逻辑正确，问题在于 Mouse Hook 未启动且无匹配 Profile。

---

## 五、双击检测检查

当前实现（`Mouse.cs:78-89`）：

```csharp
private bool IsDoubleClick(Key currentKey)
{
    var isDoubleClick = false;
    var now = Environment.TickCount;
    if (now - _lastClickTime < 500 && _lastClickKey == currentKey)
        isDoubleClick = true;
    _lastClickTime = now;
    _lastClickKey = currentKey;
    return isDoubleClick;
}
```

**分析：**
- 使用固定 500ms 双击间隔
- 检查两次点击的键是否相同（都是 MouseLeft）
- 未检查两次点击的位置距离

**结论：** 双击检测逻辑基本正确，500ms 间隔合理。

---

## 六、NavigateUp 执行检查

当前实现（`HookManager.cs:134-142`）：

```csharp
private void NavigateUp(nint foregroundWindow, Point? mousePosition)
{
    if (foregroundWindow == 0) return;
    if (mousePosition is not { } position)
        KeyboardSimulator.ModifiedKeyStroke(VirtualKey.Alt, VirtualKey.Up);
    else if (Helper.IsExplorerEmptySpace(position))
        KeyboardSimulator.ModifiedKeyStroke(VirtualKey.Alt, VirtualKey.Up);
}
```

**分析：**
- 如果有鼠标位置，先检查是否在空白处
- 如果在空白处，模拟 Alt+Up 按键
- Alt+Up 是资源管理器的"向上一级"快捷键

**结论：** 执行逻辑正确。

---

## 七、测试结果

### 7.1 配置测试

| 测试项 | 结果 |
|--------|------|
| 启用开关后 MouseHook 自动启用 | ⏳ 待用户验证 |
| 专用 Profile 自动创建 | ⏳ 待用户验证 |
| 设置保存到 settings.json | ✅ 已验证 |
| 重启后设置保持 | ⏳ 待用户验证 |

### 7.2 功能测试

| 测试项 | 结果 |
|--------|------|
| 空白处双击向上一级 | ⏳ 待用户验证 |
| 双击文件夹正常打开 | ⏳ 待用户验证 |
| 双击文件正常打开 | ⏳ 待用户验证 |
| 双击地址栏不触发 | ⏳ 待用户验证 |
| 双击侧边栏不触发 | ⏳ 待用户验证 |
| 右键空白处正常菜单 | ⏳ 待用户验证 |

---

## 八、用户操作步骤

1. 下载最新 artifact
2. 解压到 D:\Test
3. 运行 ExplorerTabUtility.exe
4. 双击托盘图标打开设置窗口
5. 切换到"偏好设置"页面
6. 勾选"启用资源管理器空白处双击向上一级"
7. 确认底部"鼠标监控"变为绿色
8. 打开资源管理器，进入多级目录
9. 在文件列表空白处双击鼠标左键
10. 检查是否返回上一级目录
