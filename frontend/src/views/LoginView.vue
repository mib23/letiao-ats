<template>
  <div class="login-page">
    <!-- 背景装饰 -->
    <div class="bg-decoration">
      <div class="blob blob-1"></div>
      <div class="blob blob-2"></div>
      <div class="blob blob-3"></div>
    </div>

    <div class="login-card">
      <!-- Logo & 标题 -->
      <div class="login-header">
        <div class="logo-icon">
          <svg viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
            <rect width="48" height="48" rx="14" fill="url(#grad)"/>
            <path d="M14 28L22 20L28 26L34 18" stroke="white" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"/>
            <circle cx="34" cy="18" r="3" fill="#a5f3fc"/>
            <defs>
              <linearGradient id="grad" x1="0" y1="0" x2="48" y2="48" gradientUnits="userSpaceOnUse">
                <stop stop-color="#6366f1"/>
                <stop offset="1" stop-color="#8b5cf6"/>
              </linearGradient>
            </defs>
          </svg>
        </div>
        <h1 class="system-name">乐跳 ATS</h1>
        <p class="system-desc">企业招聘管理系统</p>
      </div>

      <!-- 登录表单 -->
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        class="login-form"
        @keyup.enter="handleLogin"
      >
        <el-form-item prop="username">
          <el-input
            v-model="form.username"
            placeholder="请输入账号（手机号/工号）"
            size="large"
            :prefix-icon="User"
            clearable
          />
        </el-form-item>

        <el-form-item prop="password">
          <el-input
            v-model="form.password"
            type="password"
            placeholder="请输入密码"
            size="large"
            :prefix-icon="Lock"
            show-password
          />
        </el-form-item>

        <el-button
          type="primary"
          size="large"
          class="login-btn"
          :loading="loading"
          @click="handleLogin"
        >
          {{ loading ? '登录中...' : '登 录' }}
        </el-button>
      </el-form>

      <p class="login-tip">初始账号 admin / Admin@123</p>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { User, Lock } from '@element-plus/icons-vue'
import { authApi } from '@/services/auth.api'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const route  = useRoute()
const auth   = useAuthStore()

const formRef = ref(null)
const loading = ref(false)

const form = reactive({ username: '', password: '' })
const rules = {
  username: [{ required: true, message: '请填写账号', trigger: 'blur' }],
  password: [{ required: true, message: '请填写密码', trigger: 'blur' }]
}

async function handleLogin() {
  await formRef.value.validate(async valid => {
    if (!valid) return
    loading.value = true
    try {
      const res = await authApi.login(form.username, form.password)
      auth.setAuth(res.data)
      ElMessage.success(`欢迎回来，${res.data.realName}！`)
      const redirect = route.query.redirect || '/'
      router.push(redirect)
    } catch {
      // 错误已由 http 拦截器统一处理
    } finally {
      loading.value = false
    }
  })
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #0f0f1a;
  position: relative;
  overflow: hidden;
}

/* ── 背景光晕 ── */
.bg-decoration { position: absolute; inset: 0; pointer-events: none; }
.blob {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  opacity: 0.25;
  animation: float 8s ease-in-out infinite;
}
.blob-1 { width: 500px; height: 500px; background: #6366f1; top: -150px; left: -100px; animation-delay: 0s; }
.blob-2 { width: 400px; height: 400px; background: #8b5cf6; bottom: -100px; right: -80px; animation-delay: -3s; }
.blob-3 { width: 300px; height: 300px; background: #06b6d4; top: 40%; left: 55%; animation-delay: -6s; }

@keyframes float {
  0%, 100% { transform: translateY(0) scale(1); }
  50%       { transform: translateY(-30px) scale(1.05); }
}

/* ── 卡片 ── */
.login-card {
  position: relative;
  z-index: 1;
  width: 420px;
  padding: 48px 40px;
  background: rgba(255, 255, 255, 0.04);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 24px;
  box-shadow: 0 32px 80px rgba(0, 0, 0, 0.5);
}

/* ── 头部 ── */
.login-header { text-align: center; margin-bottom: 36px; }
.logo-icon svg { width: 56px; height: 56px; }
.system-name {
  margin: 16px 0 6px;
  font-size: 26px;
  font-weight: 700;
  color: #fff;
  letter-spacing: 2px;
}
.system-desc { color: rgba(255,255,255,0.45); font-size: 14px; }

/* ── 表单 ── */
.login-form { display: flex; flex-direction: column; gap: 4px; }

:deep(.el-input__wrapper) {
  background: rgba(255,255,255,0.06) !important;
  border-color: rgba(255,255,255,0.1) !important;
  border-radius: 10px !important;
  box-shadow: none !important;
  transition: border-color 0.25s;
}
:deep(.el-input__wrapper:hover),
:deep(.el-input__wrapper.is-focus) {
  border-color: #6366f1 !important;
}
:deep(.el-input__inner) { color: #fff !important; }
:deep(.el-input__inner::placeholder) { color: rgba(255,255,255,0.3); }
:deep(.el-input__prefix) { color: rgba(255,255,255,0.45); }

.login-btn {
  width: 100%;
  margin-top: 12px;
  height: 48px;
  font-size: 16px;
  font-weight: 600;
  letter-spacing: 4px;
  border-radius: 10px;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  border: none;
  transition: transform 0.2s, box-shadow 0.2s;
}
.login-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(99, 102, 241, 0.5);
}

.login-tip {
  margin-top: 20px;
  text-align: center;
  color: rgba(255,255,255,0.25);
  font-size: 12px;
}
</style>
