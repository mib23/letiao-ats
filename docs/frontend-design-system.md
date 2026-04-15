# 乐跳 ATS (Letiao) 前端全局设计规范预案

为了让招聘系统摆脱传统的“干瘪后台”感，我们制定了这套**高级、现代、动态**的设计规范。该规范采用 **深色美学 (Sleek Dark Mode)、微量玻璃拟物化 (Glassmorphism)** 和 **平滑微动效 (Micro-Animations)**。

## 一、 设计语言与原则

1. **极其高端的深空暗色调 (Premium Dark)**
   抛弃非黑即白的扁平风格。背景采用带有极微妙冷蓝色调的深色（`#0B0F19`），面板层利用白色的极低透明度配合背景虚化（Blur），产生“多层厚度”的空间感。

2. **现代几何字体 (Modern Typography)**
   全局主要正文使用 **Inter**（极大提升屏显易读性），大标题与数字混合采用现代字重。

3. **动态与生命力 (Dynamic & Alive)**
   所有交互元素（如列表卡片、按钮）在鼠标悬停时，均应有平滑的 `transform: translateY(-2px)` 及带有光晕色彩的 `box-shadow` 变化，让系统具有呼吸感。

---

## 二、 核心 CSS Tokens (设计变量)

我们统一定义了以下关键变量，请严格在 Vue 组件中搭配 `var(--xxx)` 使用：

### 1. 颜色体系 (Colors)
*   **背景色**: 
    *   `--bg-body`: `#0B0F19` (主屏幕底色)
    *   `--bg-panel`: `rgba(22, 27, 45, 0.6)` (毛玻璃卡片背景)
*   **品牌主色调**:
    *   `--brand-purple`: `#6366f1` (靛蓝紫，主按钮与主链接)
    *   `--brand-cyan`: `#06b6d4` (青色，用于高亮渐变)
*   **文本色**:
    *   `--text-main`: `#ffffff` (正文)
    *   `--text-muted`: `rgba(255, 255, 255, 0.65)` (副标题、说明文字)

### 2. 边框与光影 (Shadows & Borders)
*   `--border-glass`: `1px solid rgba(255, 255, 255, 0.08)`
*   `--shadow-hover`: `0 10px 25px -5px rgba(99, 102, 241, 0.3)`

---

## 三、 全局公用 CSS 类 (Utilities)

为了避免在多个 `.vue` 中书写冗余代码，我们提炼了以下标准全局 Class：

### `.glass-panel` (毛玻璃面板)
**用法**：替代平凡的实色 `el-card` 或底板组合。
**特征**：半透明背景 + 亚克力模糊 (`backdrop-filter: blur(16px)`) + 环境光内阴影与极细发光边框。

### `.hover-lift` (交互微动效悬浮)
**用法**：所有“可点击/引关注”的区块容器（候选人卡片、汇总图表）。
**特征**：Hover 瞬间平滑上浮 `2px` 到 `4px`，并溢出主色调漫反射光晕。

### `.text-gradient` (炫彩文字)
**用法**：大标题、着重强调数据项（比如 Dashboard 数据看板里的指标）。
**特征**：利用 `--brand-purple` -> `--brand-cyan` 的线性渐变包裹字体本身 (`-webkit-text-fill-color: transparent`)。

---

## 四、 Element Plus 深度定制覆盖

在业务实现上，我们会于公共文件级别劫持 Element Plus 默认样式：
1. **交互组件 (`el-input`, `el-select`)**：完全消除呆板边框，背景使用通透的 `rgba(255,255,255,0.04)`。
2. **表单框与卡片 (`el-card`, `el-dialog`)**：注入 `glass-panel` 的圆角和光泽质感。
3. **按钮 (`el-button--primary`)**：重新设计渐变背景，摆脱普通的单色块填充。
