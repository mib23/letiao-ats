<div align="center">
  <img src="https://img.shields.io/badge/Aesthetics-Glassmorphism-8b5cf6" alt="Style" />
  <img src="https://img.shields.io/badge/AI-Doubao_Seed_Pro-06b6d4" alt="AI Model" />
  <img src="https://img.shields.io/badge/.NET-8.0-512bd4" alt="DotNet" />
  <img src="https://img.shields.io/badge/Vue.js-3.0-4FC08D" alt="Vue" />
  <img src="https://img.shields.io/badge/Dapper-MicroORM-222" alt="Dapper" />

  <h1>🚀 Letiao ATS (乐跳招聘管理系统)</h1>
  <p>
    <strong>下一代企业级 AI 招聘智能引擎，深度融合深空极简与超视觉玻璃拟物美学范式。</strong>
  </p>

[**🇺🇸 English Version**](./README_EN.md)

</div>

---

## 🇨🇳 核心介绍 

**乐跳 ATS** 是一套现代化企业人力资源管理利器。结合了现代理念领先的**“深空极简玻璃拟物化 (Dark-space Glassmorphism)”**多维前端视觉语言，同时搭载极致稳定轻量构架的 **.NET 8** 高并发后端生态；不仅如此系统更大胆并入了由**大语言模型（LLM）**驱动前沿的物理自动化解析体验理念，极大程度打散降低了 HR 在重复录入上耗损的操作成本与漏斗阻塞！

### ✨ 极致科技特性
* 🎨 **沉浸交互美学 UI**：全站代码级贯穿极致光影拟物风与特调的高阶毛玻璃（`backdrop-filter`）卡片网格。重燃“工具后台也可以是艺术”之火，搭载悬浮发光的树状组织架构展示、大容量控制面板等硬核体验。
* 🤖 **AI 重构原生操作 (Smart-OCR Auto-Fill)**：集成嵌入 `火山引擎 Doubao-Seed-PRO` 超强推理大底座！当上传任何标准简历 PDF 时，左页执行文件内联高清预览展示，右页则能在网络无感通讯下闪现精准抽出诸如学历、工龄等核心业务节点数据，让每一把录入都是“开箱验货”的快感。
* 🛡️ **反拥堵与防碰瓷管线架构**：借助 MySQL+C# `BusinessException` 的强绑定体系，实现全局候选人手机号精准物理查重打回机制，把一切风控逻辑和不合规请求极速捕获并在中间件管道中无损转化为前台标准的 UI 拦截面板。
* 🚀 **骨髓级高性能后台支持**：全境采用强构件化逻辑，彻底剥离过重的 ORM 环境，改用 `Dapper` 微引擎对接纯 SQL 查询配合 C# 最新特性的完美调度，轻松让单节点承载高并发。

---

## 🛠 军火库及技术栈

### 🖥️ 前端框架
- **引擎构建**: `Vue 3` (Composition API) + `Vite` 极速热更新加载。
- **视觉组件库**: `Element Plus` (对底层样式进行了大刀阔斧的重写，以独占其唯美的毛玻璃主题呈现)。
- **通讯协议**: 基于 `Axios` 的全局无感知拦截交互系统，彻底切分鉴权及自动处理跨域抛出物。

### ⚙️ 后端系统 (Letiao.ATS.Api)
- **底层架构**: 最新一期 `.NET 8 Web API`，在 DI 注入上玩转极致性能切片。
- **数据访问驱动**: 纯原生 `Dapper` 取代臃肿的 EF，超高速单向解构内存中的数据库模型。
- **通行门禁**: 挂载标准的 Microsoft 内置 `JwtBearer` 持久生态验证护卫盾。
- **原件粉碎机**: 内网集成 `UglyToad.PdfPig`，直接绕过渲染层以恐怖级的速度生吞 PDF 结构文字投喂大模型。

### 🗄️ 数据库台账
- **驱动中枢**: 原生版 `MySQL 8.x`，全关系化物理主外键管线铺设，打硬底护长盘。

---

## 🏗️ 架构脉络纵览

- **领域化防腐层设计**: 基于轻盈的三层边界，控制 Controller 的薄度，让复杂的防碰撞校验完全在前置仓储前剥离。
- **统一化异常抛填管理**: 通过强接管 ASP.NET 顶级 Exception Middleware，将底层的报错乃至碰撞查重信息翻译为前端亲切的 `ApiResponse` 对话框，业务永不断档报错。
- **黑盒运行机密环境**: 私有的 `DotNetEnv` 文件锁机制，即使将整个业务打包出圈也绝不外露自己心爱的 AI 火山大模型凭证钥。
- **非线性递归树状重绘**: 后端内存态仅靠 $O(n)$ 时间复杂度的 Hash 映射魔法即由原本死板的 `parent_id` 台账行构建出极富生命力的树状嵌套数据结构支撑动态组织架构展示。

---

## 🧑‍💻 极速起步

> 本地必须准备好：**Node.js 18+** 及 **.NET 8.0 SDK** 组件环境。

### 1. 本地数据库装载初始化
1. 请先在本地的 MySQL 中执行 `database/init.sql`，为您搭设整个坚不可摧的数据底座。
2. 我们内部留下的唯一通关超级密码就是账号 `admin` + 密码 `Admin@123`，起步即全开！

### 2. 接通微服务总线 (后端启动)
```bash
> cd backend
> dotnet restore
> dotnet run   # 服务将火热挂载于 localhost:5244 上
```
*(⚠️ 温馨提示：如果想要体验高阶 AI 脱水解析能力，需要先前往 `/backend` 下补上缺失的 `.env` 配置文件并将您的 `VOLCENGINE_API_KEY=` 植入其中！)*

### 3. 打开惊艳的视觉舱门 (前端构建)
```bash
> cd frontend
> npm install
> npm run dev  # 即时在 localhost:5173 以流式启动前端系统
```

---
<p align="center"><i>倾注对未来 ATS 工作流自动化憧憬的纯粹工艺品。</i></p>
