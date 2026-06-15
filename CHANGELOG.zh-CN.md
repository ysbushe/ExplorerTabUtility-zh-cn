# 更新日志（中文）

## [2.5.0-zhcn] - 2026-06-15

### 新增

- 完整中文界面（简体中文 zh-CN）
- 语言设置项：支持 Auto / English / 简体中文 切换
- 中文使用说明文档
- 中文项目说明（README.zh-CN.md）

### 汉化范围

- 主窗口标题和导航菜单
- 托盘图标右键菜单
- 设置页面所有选项
- 快捷键配置界面
- 标签页搜索弹窗
- 关于页面
- 自定义消息框按钮
- 状态栏指示器
- 所有 ToolTip 提示文本
- HotKeyAction 枚举描述
- 首次运行提示消息
- 确认对话框消息

### 翻译规范

- File Explorer → 资源管理器
- Tab → 标签页
- Window Hook → 窗口监控
- Mouse Hook → 鼠标监控
- Keyboard Hook → 快捷键监控
- Reuse Tabs → 复用标签页
- Duplicate Tab → 复制当前标签页
- Reopen Closed Tabs → 重新打开已关闭标签页
- Restore Previous Windows → 恢复上次窗口
- Detach Tab → 分离标签页
- Snap → 贴靠窗口
- Navigate Back → 后退
- Navigate Forward → 前进
- Navigate Up → 向上一级
- Set Target Window → 设为目标窗口
- Toggle Visibility → 显示/隐藏主窗口
- Hotkey Profile → 快捷键配置
- Scope → 作用范围
- Global → 全局
- FileExplorer → 资源管理器内

### 技术实现

- 采用直接替换 + 语言设置项方案
- 语言资源在启动时加载，之后使用内存资源
- 不新增常驻线程、后台服务、数据库或网络请求
- 额外资源开销：内存 ~50KB，磁盘 ~20KB
- 保留原有英文版本（通过语言设置切换）

### 兼容性

- 保留英文界面（设置 Language 为 "en-US" 即可恢复）
- 不修改系统文件
- 不注入 explorer.exe
- 不写危险注册表项
- 不删除原项目功能
