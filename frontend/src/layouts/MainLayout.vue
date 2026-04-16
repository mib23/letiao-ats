<template>
  <el-container class="main-layout">
    <!-- ── 侧边栏 ── -->
    <el-aside :width="collapsed ? '64px' : '220px'" class="sidebar">
      <!-- Logo -->
      <div class="sidebar-logo" @click="router.push('/')">
        <div class="logo-mark">
          <svg viewBox="0 0 32 32" fill="none"><rect width="32" height="32" rx="8" fill="url(#g2)"/>
            <path d="M8 20L14 14L19 19L24 12" stroke="white" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"/>
            <circle cx="24" cy="12" r="2.5" fill="#a5f3fc"/>
            <defs><linearGradient id="g2" x1="0" y1="0" x2="32" y2="32" gradientUnits="userSpaceOnUse">
              <stop stop-color="#6366f1"/><stop offset="1" stop-color="#8b5cf6"/>
            </linearGradient></defs>
          </svg>
        </div>
        <span v-show="!collapsed" class="logo-text">乐跳 ATS</span>
      </div>

      <!-- 菜单 -->
      <el-menu
        :default-active="activeMenu"
        :collapse="collapsed"
        :collapse-transition="false"
        router
        class="sidebar-menu"
      >
        <el-menu-item index="/dashboard">
          <el-icon><DataAnalysis /></el-icon>
          <template #title>工作台</template>
        </el-menu-item>
        <el-menu-item index="/jobs">
          <el-icon><Suitcase /></el-icon>
          <template #title>职位管理</template>
        </el-menu-item>
        <el-menu-item index="/candidates">
          <el-icon><User /></el-icon>
          <template #title>候选人库</template>
        </el-menu-item>
      </el-menu>

      <!-- 折叠按钮 -->
      <div class="collapse-btn" @click="collapsed = !collapsed">
        <el-icon><component :is="collapsed ? ArrowRightBold : ArrowLeftBold" /></el-icon>
      </div>
    </el-aside>

    <!-- ── 主区域 ── -->
    <el-container class="main-container">
      <!-- 顶栏 -->
      <el-header class="topbar">
        <div class="topbar-left">
          <el-breadcrumb separator="/">
            <el-breadcrumb-item :to="{ path: '/' }">首页</el-breadcrumb-item>
            <el-breadcrumb-item v-if="currentTitle">{{ currentTitle }}</el-breadcrumb-item>
          </el-breadcrumb>
        </div>
        <div class="topbar-right">
          <el-dropdown trigger="click" @command="handleUserCmd">
            <div class="user-info">
              <el-avatar :size="32" class="user-avatar">
                {{ auth.realName?.charAt(0) || 'U' }}
              </el-avatar>
              <span class="user-name">{{ auth.realName }}</span>
              <el-icon class="arrow"><ArrowDown /></el-icon>
            </div>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="logout">
                  <el-icon><SwitchButton /></el-icon> 退出登录
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-header>

      <!-- 内容区 -->
      <el-main class="main-content">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessageBox, ElMessage } from 'element-plus'
import {
  DataAnalysis, Suitcase, ArrowLeftBold, ArrowRightBold, ArrowDown, SwitchButton, User
} from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'

const router   = useRouter()
const route    = useRoute()
const auth     = useAuthStore()
const collapsed = ref(false)

const activeMenu   = computed(() => route.path)
const currentTitle = computed(() => route.meta?.title || '')

function handleUserCmd(cmd) {
  if (cmd === 'logout') {
    ElMessageBox.confirm('确认退出登录？', '提示', { type: 'warning' })
      .then(() => {
        auth.clearAuth()
        ElMessage.success('已安全退出')
        router.push('/login')
      })
      .catch(() => {})
  }
}
</script>

<style scoped>
.main-layout { height: 100vh; background: #0d0d1a; }

/* ── 侧边栏 ── */
.sidebar {
  background: #111128;
  border-right: 1px solid rgba(255,255,255,0.06);
  display: flex;
  flex-direction: column;
  transition: width 0.25s;
  overflow: hidden;
}

.sidebar-logo {
  height: 64px;
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 0 16px;
  cursor: pointer;
  border-bottom: 1px solid rgba(255,255,255,0.06);
  flex-shrink: 0;
}
.logo-mark svg { width: 32px; height: 32px; flex-shrink: 0; }
.logo-text { font-size: 17px; font-weight: 700; color: #fff; white-space: nowrap; letter-spacing: 1px; }

.sidebar-menu {
  flex: 1;
  border-right: none !important;
  background: transparent;
  overflow-y: auto;
  overflow-x: hidden;
}
:deep(.el-menu-item) { color: rgba(255,255,255,0.6); border-radius: 8px; margin: 2px 8px; }
:deep(.el-menu-item:hover) { background: rgba(99,102,241,0.15); color: #fff; }
:deep(.el-menu-item.is-active) { background: rgba(99,102,241,0.25); color: #6366f1; font-weight: 600; }

.collapse-btn {
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgba(255,255,255,0.35);
  cursor: pointer;
  border-top: 1px solid rgba(255,255,255,0.06);
  transition: color 0.2s;
}
.collapse-btn:hover { color: #6366f1; }

/* ── 顶栏 ── */
.topbar {
  height: 64px;
  background: rgba(17, 17, 40, 0.9);
  border-bottom: 1px solid rgba(255,255,255,0.06);
  backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24px;
}
:deep(.el-breadcrumb__inner) { color: rgba(255,255,255,0.5) !important; }
:deep(.el-breadcrumb__item:last-child .el-breadcrumb__inner) { color: #fff !important; }

.topbar-right { display: flex; align-items: center; }
.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  padding: 4px 10px;
  border-radius: 8px;
  transition: background 0.2s;
}
.user-info:hover { background: rgba(255,255,255,0.06); }
.user-avatar { background: linear-gradient(135deg, #6366f1, #8b5cf6); font-size: 14px; font-weight: 700; }
.user-name { color: rgba(255,255,255,0.85); font-size: 14px; }
.arrow { color: rgba(255,255,255,0.4); font-size: 12px; }

/* ── 内容 ── */
.main-content {
  background: #0d0d1a;
  padding: 24px;
  overflow-y: auto;
}

/* ── 路由切换过渡 ── */
.fade-enter-active, .fade-leave-active { transition: opacity 0.2s, transform 0.2s; }
.fade-enter-from { opacity: 0; transform: translateY(8px); }
.fade-leave-to   { opacity: 0; transform: translateY(-8px); }
</style>
