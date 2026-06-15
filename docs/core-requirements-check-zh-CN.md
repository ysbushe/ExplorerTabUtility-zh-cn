# ExplorerTabUtility 核心需求核查报告

> 分析日期：2026-06-15
> 系统环境：Windows 11 专业版 26H1 / Build 28000.1836 / x64

---

## 需求 A：新开资源管理器窗口自动归集到标签页

### 检查结果：✅ 已满足

### 逐项检查

| # | 检查项 | 结果 | 源码位置 | 说明 |
|---|--------|------|----------|------|
| 1 | 是否有窗口监听机制 | ✅ 有 | `ExplorerWatcher.cs:908` | `SetWinEventHook(EVENT_OBJECT_SHOW)` 监听窗口显示事件 |
| 2 | 是否能检测新开窗口 | ✅ 有 | `ExplorerWatcher.cs:369` | `OnShellWindowRegistered` 通过 COM `ShellWindows.WindowRegistered` 事件 |
| 3 | 是否能获取新窗口路径 | ✅ 有 | `ExplorerWatcher.cs:774-782` | 通过 `InternetExplorer.LocationURL` COM 属性 |
| 4 | 是否能找到目标窗口 | ✅ 有 | `ExplorerWatcher.cs:689-706` | `GetMainWindowHWnd` 查找主窗口（标签数最多的 CabinetWClass） |
| 5 | 是否能转换为标签页 | ✅ 有 | `ExplorerWatcher.cs:590-661` | `OpenTabNavigateWithSelection` 发送 Ctrl+T 创建标签并导航 |
| 6 | 是否支持重复路径复用 | ✅ 有 | `ExplorerWatcher.cs:595-604` | `SearchForTab` 使用 `ShellPathComparer` 比较 PIDL |
| 7 | 是否支持设为目标窗口 | ✅ 有 | `ExplorerWatcher.cs:295-299` | `SetTargetWindow` 方法 |
| 8 | 是否支持 Ctrl+Shift 强制新窗口 | ✅ 有 | `Helper.cs:425-436` | `IsCtrlShiftDown()` 检测按键状态 |
| 9 | Windows 11 26H1 兼容性 | ⚠️ 需测试 | - | COM 接口和魔法命令可能受 Windows 更新影响 |

### 功能实现原理

```
用户双击文件夹 / 打开新路径
  ↓
Windows 创建新 Explorer 窗口 (CabinetWClass)
  ↓
COM ShellWindows 触发 WindowRegistered 事件
  ↓
ExplorerWatcher.OnShellWindowRegistered() 接收
  ↓
检测到 Ctrl+Shift？ ──是──→ 放行（保持新窗口）
  │
  否
  ↓
获取新窗口的 InternetExplorer COM 对象
  ↓
获取窗口路径（LocationURL）
  ↓
窗口数 > 1 且是单标签窗口？
  │
  是
  ↓
隐藏新窗口（透明/移出屏幕）
  ↓
在主窗口中创建新标签页（Ctrl+T）
  ↓
导航到目标路径
  ↓
关闭原始新窗口（window.Quit()）
  ↓
完成：用户看到的是标签页而非新窗口
```

### 配置步骤

1. **启动程序** → 系统托盘出现图标
2. **右键托盘图标** → 确认以下选项已勾选：
   - ✅ **Window Hook**（窗口监控）— 核心开关
   - ✅ **Reuse Tabs**（复用已有标签页）— 避免重复标签
3. **验证**：打开新的资源管理器窗口，应自动变为当前窗口的标签页
4. **强制新窗口**：按住 **Ctrl+Shift** 再打开路径

### 默认行为

| 场景 | 默认行为 | 可否配置 |
|------|----------|----------|
| 普通打开新路径 | 转为标签页 | ✅ Window Hook 开关 |
| 已打开路径再次打开 | 切换到已有标签 | ✅ Reuse Tabs 开关 |
| Ctrl+Shift 打开 | 保持新窗口 | ❌ 固定行为 |
| 资源管理器崩溃恢复 | 自动恢复 | ✅ Restore Previous Windows |
| 控制面板特殊处理 | 保持新窗口 | ❌ 固定行为 |

### 风险提示

| 风险项 | 等级 | 说明 |
|--------|------|------|
| COM 接口兼容性 | ⚠️ 中 | `InternetExplorer` COM 接口是 Windows Shell 内部实现，未公开保证 |
| 魔法命令 | ⚠️ 中 | `0xA221`（切换标签）、`0xA21B`（新建标签）、`0xA021`（关闭标签）为未公开命令 |
| Windows 更新 | ⚠️ 中 | 大版本更新可能改变 Explorer 内部结构 |
| 杀毒软件 | ⚠️ 低 | 低级钩子可能触发误报 |

---

## 需求 B：资源管理器空白处双击向上一级

### 检查结果：✅ 已满足（需手动配置）

### 逐项检查

| # | 检查项 | 结果 | 源码位置 | 说明 |
|---|--------|------|----------|------|
| 1 | 是否有 Mouse Hook | ✅ 有 | `Hooks/Mouse.cs` | `LowLevelMouseHook` 全局鼠标钩子 |
| 2 | 是否支持鼠标点击空白处触发 | ✅ 有 | `Helper.cs:266-273` | `IsExplorerEmptySpace` 通过 IAccessible 检测 |
| 3 | 是否支持 NavigateUp 动作 | ✅ 有 | `HookManager.cs:134-142` | 模拟 `Alt+Up` 键盘操作 |
| 4 | 是否支持双击触发 | ✅ 有 | `Mouse.cs:78-89` | `IsDoubleClick` 检测（500ms 内同键两次） |
| 5 | 是否区分空白处和文件 | ✅ 有 | `Helper.cs:267-272` | `AccessibleObjectFromPoint` + `ROLE_SYSTEM_LIST` |
| 6 | 是否不影响双击打开文件夹 | ✅ 不影响 | - | 双击文件夹时 childId≠0，不匹配空白处检测 |
| 7 | 是否不影响文件选择 | ✅ 不影响 | - | 空白处点击不会选中文件 |
| 8 | 是否不影响右键菜单 | ✅ 不影响 | - | 仅响应鼠标按键事件 |

### 功能实现原理

```
用户在资源管理器文件列表空白处双击左键
  ↓
Mouse.LowLevelMouseHook_Down() 接收鼠标事件
  ↓
IsDoubleClick() 检测：500ms 内同一键两次点击？
  │
  否 → 忽略
  是 ↓
遍历所有 HotKeyProfile
  ↓
找到匹配的 Profile（IsMouse=true, IsDoubleClick=true, HotKeys 匹配）
  ↓
检查 Scope：FileExplorer？
  │
  是 → 检查前台窗口是否为资源管理器
  │
  是 ↓
Helper.IsExplorerEmptySpace(mousePosition)
  ↓
WinApi.AccessibleObjectFromPoint(point)
  ↓
检查 childId == 0 且 accRole == ROLE_SYSTEM_LIST (0x21)
  │
  是（确实是空白处）→ 触发 NavigateUp
  │
  否（点击在文件/文件夹上）→ 不触发
  ↓
KeyboardSimulator.ModifiedKeyStroke(Alt, Up)
  ↓
资源管理器执行"向上一级"操作
```

### 空白处检测细节

```csharp
// Helper.cs:266-273
public static bool IsExplorerEmptySpace(Point point)
{
    // 获取点击位置的无障碍对象
    var hr = WinApi.AccessibleObjectFromPoint(point, out var accObj, out var childId);
    
    // 如果获取失败，或 childId 不为 0（说明点在子元素上），返回 false
    if (hr != 0 || childId is not 0) return false;
    
    // 检查角色是否为列表（ROLE_SYSTEM_LIST = 0x21）
    var role = accObj.get_accRole(0);
    return role is 0x21;
}
```

**判断逻辑：**
- `childId == 0`：点击在容器本身（列表控件），而非子元素（文件/文件夹）
- `role == 0x21`：容器是列表类型（文件列表区域）

### 配置步骤

1. **双击托盘图标** → 打开主设置窗口
2. **点击 "NEW"** → 添加新配置项
3. **设置快捷键**：
   - 点击快捷键输入框
   - **快速双击鼠标左键**（两次点击间隔 < 500ms）
   - 输入框应显示 `LMB_DBL`
4. **设置作用范围**：选择 **FileExplorer**（资源管理器内）
5. **设置动作**：选择 **NavigateUp**（向上一级）
6. **点击 "SAVE"** → 保存配置
7. **启用鼠标 Hook**：右键托盘图标 → 勾选 **Mouse Hook**

### 验证清单

| 测试场景 | 预期行为 | 是否影响 |
|----------|----------|----------|
| 双击空白处 | 执行"向上一级" | ✅ 正常 |
| 双击文件夹 | 打开文件夹 | ❌ 不影响 |
| 双击文件 | 打开文件 | ❌ 不影响 |
| 单击文件 | 选中文件 | ❌ 不影响 |
| 单击空白处 | 取消选中 | ❌ 不影响 |
| 右键空白处 | 弹出右键菜单 | ❌ 不影响 |
| 右键文件 | 弹出文件右键菜单 | ❌ 不影响 |
| 地址栏双击 | 编辑路径 | ❌ 不影响（非列表区域） |
| 侧边栏双击 | 无操作 | ❌ 不影响（非列表区域） |
| 标题栏双击 | 最大化/还原 | ❌ 不影响（非列表区域） |

### 注意事项

| 事项 | 说明 |
|------|------|
| 需手动配置 | 默认配置中没有此功能，需用户手动添加 |
| 需启用 Mouse Hook | 配置后需在托盘菜单中启用鼠标 Hook |
| 双击间隔 | 默认 500ms，过快或过慢可能无法识别 |
| 系统无障碍 | 依赖 IAccessible 接口，某些无障碍工具可能干扰 |

---

## 综合评估

| 需求 | 满足度 | 配置复杂度 | 风险等级 |
|------|--------|------------|----------|
| A：新窗口自动转标签页 | ✅ 完全满足 | 低（默认已启用） | ⚠️ 中（COM 依赖） |
| B：空白处双击向上一级 | ✅ 完全满足 | 中（需手动配置） | 低 |

### 建议

1. **需求 A**：开箱即用，无需额外配置。建议先测试基本功能是否正常。
2. **需求 B**：需要手动添加鼠标 Profile。建议按照上述步骤配置并测试。
3. **兼容性**：Windows 11 26H1 是较新版本，建议先运行程序测试基本功能，如发现问题再排查。
