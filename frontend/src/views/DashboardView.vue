<template>
  <div class="dashboard">
    <h2 class="page-title">工作台</h2>

    <!-- 统计卡片 -->
    <div class="stat-grid">
      <div v-for="stat in stats" :key="stat.label" class="stat-card">
        <div class="stat-icon" :style="{ background: stat.bg }">
          <el-icon :size="22"><component :is="stat.icon" /></el-icon>
        </div>
        <div class="stat-info">
          <div class="stat-value">{{ stat.value }}</div>
          <div class="stat-label">{{ stat.label }}</div>
        </div>
      </div>
    </div>

    <!-- 系统状态 -->
    <div class="section-card">
      <div class="section-header">
        <span class="section-title">系统状态</span>
        <el-button size="small" :loading="healthLoading" @click="fetchHealth">刷新</el-button>
      </div>
      <div v-if="health" class="health-grid">
        <div class="health-item">
          <span class="h-label">API 服务</span>
          <el-tag :type="health.api === 'ok' ? 'success' : 'danger'" size="small">
            {{ health.api === 'ok' ? '正常' : '异常' }}
          </el-tag>
        </div>
        <div class="health-item">
          <span class="h-label">数据库</span>
          <el-tag :type="health.database === 'ok' ? 'success' : 'danger'" size="small">
            {{ health.database === 'ok' ? '已连通' : '不可达' }}
          </el-tag>
        </div>
        <div class="health-item">
          <span class="h-label">运行环境</span>
          <el-tag type="info" size="small">{{ health.environment }}</el-tag>
        </div>
        <div class="health-item">
          <span class="h-label">检查时间</span>
          <span class="h-value">{{ health.timestamp }}</span>
        </div>
      </div>
      <el-empty v-else-if="!healthLoading" description="点击刷新获取系统状态" />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { Briefcase, User, Document, TrendCharts } from '@element-plus/icons-vue'
import http from '@/services/http'

const health        = ref(null)
const healthLoading = ref(false)

const stats = [
  { label: '在招职位', value: '—', icon: Briefcase,    bg: 'linear-gradient(135deg,#6366f1,#8b5cf6)' },
  { label: '人才库',   value: '—', icon: User,         bg: 'linear-gradient(135deg,#06b6d4,#0891b2)' },
  { label: '本周简历', value: '—', icon: Document,     bg: 'linear-gradient(135deg,#10b981,#059669)' },
  { label: '面试中',   value: '—', icon: TrendCharts,  bg: 'linear-gradient(135deg,#f59e0b,#d97706)' },
]

async function fetchHealth() {
  healthLoading.value = true
  try {
    const res = await http.get('/api/health')
    health.value = res.data
  } catch {
    health.value = null
  } finally {
    healthLoading.value = false
  }
}

onMounted(fetchHealth)
</script>

<style scoped>
.dashboard { color: #fff; }
.page-title { font-size: 22px; font-weight: 700; margin: 0 0 24px; }

/* ── 统计卡片 ── */
.stat-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; margin-bottom: 24px; }
.stat-card {
  background: rgba(255,255,255,0.04);
  border: 1px solid rgba(255,255,255,0.07);
  border-radius: 16px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  transition: transform 0.2s, border-color 0.2s;
}
.stat-card:hover { transform: translateY(-3px); border-color: rgba(99,102,241,0.3); }
.stat-icon {
  width: 48px; height: 48px;
  border-radius: 12px;
  display: flex; align-items: center; justify-content: center;
  color: #fff; flex-shrink: 0;
}
.stat-value { font-size: 26px; font-weight: 700; color: #fff; line-height: 1; }
.stat-label { font-size: 13px; color: rgba(255,255,255,0.45); margin-top: 4px; }

/* ── 区块卡片 ── */
.section-card {
  background: rgba(255,255,255,0.04);
  border: 1px solid rgba(255,255,255,0.07);
  border-radius: 16px;
  padding: 20px 24px;
}
.section-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 20px; }
.section-title { font-size: 15px; font-weight: 600; color: #fff; }

.health-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; }
.health-item { display: flex; flex-direction: column; gap: 8px; }
.h-label { font-size: 12px; color: rgba(255,255,255,0.4); }
.h-value { font-size: 13px; color: rgba(255,255,255,0.7); }
</style>
