<template>
  <div class="kanban-page">
    <div class="page-header">
      <el-button link icon="ArrowLeft" @click="$router.push('/jobs')" style="margin-right: 15px; font-size: 20px;"></el-button>
      <h2 class="title-font text-gradient">岗位招聘跟进看板 - Kanban</h2>
    </div>

    <!-- 看板画板主区 -->
    <div class="kanban-board">
      <div class="lane glass-panel" v-for="stage in stages" :key="stage.key">
        <div class="lane-header text-gradient">
          {{ stage.label }} 
          <span class="count-badge">{{ getStageItems(stage.key).length }}</span>
        </div>

        <!-- 拖拽主容器 -->
        <vue-draggable-next
          class="lane-body"
          :list="getStageItems(stage.key)"
          group="pipeline"
          item-key="id"
          @change="(e) => handleChange(e, stage.key)"
        >
          <!-- 避免警告，只在需要时渲染内禀元素 -->
          <div class="k-card hover-lift" v-for="element in getStageItems(stage.key)" :key="element.id">
            <div class="k-name">{{ element.candidateName }}</div>
            <div class="k-desc">
              <span>{{ element.candidateHighestDegree || '未知学历' }}</span>
              <span style="margin: 0 4px">|</span>
              <span>{{ element.candidateWorkYears }} 年经验</span>
            </div>
            <div class="k-desc">📞 {{ element.candidatePhone }}</div>
            <div class="k-time">{{ formatDate(element.statusChangedAt) }}入流</div>
          </div>
        </vue-draggable-next>
        
        <div v-if="!getStageItems(stage.key).length" class="empty-lane">该阶段暂无候选人</div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { pipelineApi } from '@/services/pipeline.api'
import { VueDraggableNext } from 'vue-draggable-next'
import { ElMessage } from 'element-plus'

const route = useRoute()
const jobId = route.params.id
const pipelines = ref([])

// 定义看板泳道
const stages = [
  { key: 'SCREENING', label: '初筛阶段' },
  { key: 'BIZ_REVIEW', label: '业务复筛' },
  { key: 'INTERVIEW', label: '面试环节' },
  { key: 'BG_CHECK', label: '背调审计' },
  { key: 'OFFER', label: 'Offer 发放' },
  { key: 'ONBOARDED', label: '成功入职' }
]

// 按阶段过滤渲染
const getStageItems = (stageKey) => {
  return pipelines.value.filter(p => p.currentStage === stageKey)
}

const loadData = async () => {
  try {
    const res = await pipelineApi.getBoard(jobId)
    pipelines.value = res.data
  } catch (error) {
    ElMessage.error('无法加载看板数据')
  }
}

// 拖拽释放回调，向后端提交同步新位置
const handleChange = async (e, toStage) => {
  if (e.added) {
    const item = e.added.element
    if (item.currentStage === toStage) return
    
    // UI 层乐观更新加速响应感
    const originalStage = item.currentStage
    item.currentStage = toStage
    
    try {
      await pipelineApi.moveStage(item.id, toStage)
      ElMessage.success(`成功流转至 ${stages.find(s => s.key === toStage).label}`)
    } catch(err) {
      item.currentStage = originalStage // 操作失败，本地闪回
      loadData()
    }
  }
}

const formatDate = (ds) => {
  if(!ds) return '';
  const d = new Date(ds)
  return `${d.getMonth() + 1}-${d.getDate()} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.kanban-page {
  padding: 20px;
  height: calc(100vh - 40px);
  display: flex;
  flex-direction: column;
}
.page-header {
  display: flex;
  align-items: center;
  margin-bottom: 24px;
}
.kanban-board {
  display: flex;
  gap: 16px;
  flex: 1;
  overflow-x: auto;
  align-items: stretch;
  padding-bottom: 10px;
}

/* 泳道系统 */
.lane {
  width: 280px;
  min-width: 280px;
  display: flex;
  flex-direction: column;
  padding: 16px 12px;
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.05);
}
.lane-header {
  font-size: 16px;
  font-weight: 700;
  margin-bottom: 16px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.count-badge {
  background: rgba(99, 102, 241, 0.2);
  color: #a5b4fc;
  font-size: 12px;
  padding: 2px 8px;
  border-radius: 10px;
}

.lane-body {
  flex: 1;
  overflow-y: auto;
  min-height: 150px; /* 保证可以作为释放区 */
}
.empty-lane {
  text-align: center;
  color: rgba(255,255,255,0.2);
  font-size: 13px;
  margin-top: 20px;
  pointer-events: none; /* 穿透 */
}

/* 卡片 */
.k-card {
  background: rgba(30, 36, 56, 0.8);
  border: 1px solid rgba(99, 102, 241, 0.15);
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  padding: 16px;
  border-radius: 12px;
  margin-bottom: 12px;
  cursor: grab;
  position: relative;
}
.k-card:active {
  cursor: grabbing;
}
.k-card:hover { border-color: rgba(99, 102, 241, 0.5); }
.k-name {
  font-weight: 700;
  color: #fff;
  font-size: 15px;
  margin-bottom: 6px;
}
.k-desc {
  font-size: 12px;
  color: rgba(255,255,255,0.6);
  margin-bottom: 6px;
}
.k-time {
  font-size: 11px;
  color: rgba(99, 102, 241, 0.7);
  text-align: right;
  margin-top: 10px;
  border-top: 1px solid rgba(255,255,255,0.05);
  padding-top: 6px;
}
</style>
