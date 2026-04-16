# 项目环境与 AI 工作流规范

## 前端开发核心纪律
本项目采用极其特殊的“深色玻璃拟物化 (Glassmorphism)”和动态 UI 设计。
🚨 **警告：当你要进行任何前端 UI、CSS、Vue 组件相关的开发或修改时，你必须：**
1. 先静默读取并严格遵循 `docs/frontend-design-system.md` 里的所有规范。
2. 绝对禁止硬编码十六进制颜色，必须使用其中的 CSS Tokens（如 `var(--brand-purple)`）。
3. 必须优先使用 `.glass-panel`, `.hover-lift` 等全局类，禁止擅自写“干瘪扁平”的普通样式。
