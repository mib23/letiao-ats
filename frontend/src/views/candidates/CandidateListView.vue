<template>
  <div class="candidate-list-container">
    <div class="page-header">
      <h2 class="title-font text-gradient">候选人库</h2>
      <el-button type="primary" class="hover-lift" @click="handleUpload">上传候选人简历</el-button>
    </div>

    <!-- 筛选区 -->
    <el-card class="filter-card glass-panel hover-lift">
      <el-form inline :model="queryParams">
        <el-form-item label="模糊搜索">
          <el-input v-model="queryParams.keyword" placeholder="搜索姓名/手机号..." clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="fetchData">查询</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 列表区 -->
    <el-card class="table-card glass-panel hover-lift">
      <el-table :data="tableData" v-loading="loading" border style="width: 100%">
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="name" label="姓名" width="150" />
        <el-table-column prop="phone" label="联系电话" width="150" />
        <el-table-column prop="highestDegree" label="最高学历" width="120" />
        <el-table-column prop="workYears" label="工作经验(年)" width="120" />
        <el-table-column prop="ownerName" label="归属人" width="120">
           <template #default="{ row }">
             <el-tag :type="row.ownerName ? 'success' : 'info'">{{ row.ownerName || '公海池' }}</el-tag>
           </template>
        </el-table-column>
        <el-table-column label="流转" width="120" fixed="right">
           <template #default="{ row }">
             <el-button type="success" link @click="openAssignDialog(row)">投递岗位</el-button>
           </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="录入时间" width="160" />
      </el-table>

      <div class="pagination-container">
        <el-pagination
          v-model:current-page="queryParams.page"
          v-model:page-size="queryParams.pageSize"
          :total="total"
          :page-sizes="[10, 20, 50]"
          layout="total, sizes, prev, pager, next"
          @size-change="fetchData"
          @current-change="fetchData"
        />
      </div>
    </el-card>

    <!-- 简历上传弹窗 -->
    <el-dialog v-model="uploadDialogVisible" title="候选人扫描仪 - 结构化入库" width="900px" class="glass-panel" destroy-on-close>
      
      <!-- 未解析时展示大上传框 -->
      <div v-show="!parsedData" style="text-align: center; padding: 40px 20px;">
        <el-upload
          drag
          action="/api/upload/resume"
          :headers="uploadHeaders"
          :on-success="handleUploadSuccess"
          :on-progress="handleUploadProgress"
          :on-error="handleUploadError"
          :show-file-list="false"
        >
          <el-icon class="el-icon--upload" style="color: var(--brand-purple);"><upload-filled /></el-icon>
          <div class="el-upload__text" style="font-size: 16px;">
            拖拽简历文件 (PDF) 放至此处，或 <em class="text-gradient">点击上传</em>
          </div>
          <template #tip>
            <div class="el-upload__tip" style="color: var(--text-muted); margin-top: 15px;">
              系统将自动调用大语言模型进行精准结构化提取
            </div>
          </template>
        </el-upload>
      </div>

      <!-- 解析完成后展示【左预览 + 右表单】 -->
      <template v-if="parsedData">
        <el-row :gutter="20">
          <el-col :span="12">
            <div class="preview-panel glass-panel">
              <h4 class="panel-head text-gradient">📄 原件预览</h4>
              <iframe :src="parsedData.resumeFileUrl" class="resume-iframe"></iframe>
            </div>
          </el-col>
          <el-col :span="12">
            <div class="parsed-form-panel glass-panel">
              <h4 class="panel-head text-gradient">🌟 智能解析结果确认</h4>
              <el-form :model="parsedData" label-width="80px" label-position="left">
                <el-form-item label="姓名">
                  <el-input v-model="parsedData.name" />
                </el-form-item>
                <el-form-item label="手机号">
                  <el-input v-model="parsedData.phone" />
                </el-form-item>
                <el-form-item label="邮箱">
                  <el-input v-model="parsedData.email" />
                </el-form-item>
                <el-form-item label="性别">
                  <el-select v-model="parsedData.gender" style="width: 100%;">
                    <el-option label="未知" :value="0" />
                    <el-option label="男" :value="1" />
                    <el-option label="女" :value="2" />
                  </el-select>
                </el-form-item>
                <el-form-item label="最高学历">
                  <el-input v-model="parsedData.highestDegree" />
                </el-form-item>
                <el-form-item label="工作年限">
                  <el-input-number v-model="parsedData.workYears" :min="0" style="width: 100%;" />
                </el-form-item>
              </el-form>
            </div>
          </el-col>
        </el-row>
      </template>

      <template #footer>
        <span class="dialog-footer">
          <el-button @click="uploadDialogVisible = false" :disabled="uploading">取消</el-button>
          <el-button type="primary" :disabled="!parsedData" @click="handleConfirmSave" :loading="saving">
            保存归档入库
          </el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 投递岗位弹窗 -->
    <el-dialog v-model="assignDialogVisible" title="将候选人调入岗位流程" width="400px" class="glass-panel">
      <el-form label-position="top">
        <el-form-item label="选择目标岗位 (处于开放中)">
          <el-select v-model="assignJobId" filterable placeholder="请选择职位..." style="width: 100%">
            <el-option v-for="job in openJobs" :key="job.id" :label="job.title" :value="job.id" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="assignDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="confirmAssign">确认推进</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { ElMessage, ElLoading } from 'element-plus'
import { candidateApi } from '@/services/candidate.api'
import { pipelineApi } from '@/services/pipeline.api'
import { UploadFilled } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'

const uploadDialogVisible = ref(false)
const queryParams = ref({
  keyword: '',
  page: 1,
  pageSize: 10
})

const loading = ref(false)
const uploading = ref(false)
const saving = ref(false)
const tableData = ref([])
const total = ref(0)
const parsedData = ref(null)
let loadingInstance = null

const auth = useAuthStore()

// Assign Pipeline
const assignDialogVisible = ref(false)
const assignJobId = ref(null)
const assignCandidateId = ref(null)
const openJobs = ref([])

const openAssignDialog = async (row) => {
  if (openJobs.value.length === 0) {
    // dynamically fetch published jobs 
    import('@/services/http').then(m => {
       m.default.get('/api/jobs', { params: { page: 1, pageSize: 100, keyword: '' } })
        .then(res => { openJobs.value = res.data.list })
    })
  }
  assignCandidateId.value = row.id
  assignJobId.value = null
  assignDialogVisible.value = true
}

const confirmAssign = async () => {
  if (!assignJobId.value) return ElMessage.warning('请选择职位')
  try {
    await pipelineApi.addCandidate({ candidateId: assignCandidateId.value, jobId: assignJobId.value })
    ElMessage.success('成功投递！该候选人已进入职位初筛阶段')
    assignDialogVisible.value = false
  } catch(e) {
    ElMessage.error(e.response?.data?.msg || '投递失败')
  }
}

// Upload headers
const uploadHeaders = computed(() => ({
  Authorization: `Bearer ${auth.token}`
}))

const fetchData = async () => {
  loading.value = true
  try {
    const res = await candidateApi.getList(queryParams.value)
    tableData.value = res.data.list
    total.value = res.data.total
  } catch (error) {
    console.error(error)
  } finally {
    loading.value = false
  }
}

const handleUpload = () => {
  parsedData.value = null
  uploadDialogVisible.value = true
}

const handleUploadProgress = () => {
  uploading.value = true
  loadingInstance = ElLoading.service({
    target: '.el-dialog',
    text: '正在借助 AI 引擎结构化解析您的简历（耗时约稍后秒，请耐心等待）...',
    background: 'rgba(0, 0, 0, 0.7)',
  })
}

const handleUploadSuccess = (response, uploadFile) => {
  uploading.value = false
  if (loadingInstance) loadingInstance.close()

  if (response.code === 200) {
    ElMessage.success('智能解析完成！')
    
    // Parse the JSON string from AI model
    let aiJson = {}
    try {
      if (response.data.aiParsedData) {
        aiJson = JSON.parse(response.data.aiParsedData)
      }
    } catch(e) {
      console.warn('AI 无法解析 JSON', e)
    }

    parsedData.value = {
      name: aiJson.name || uploadFile.name.split('.')[0] || '',
      phone: aiJson.phone || '',
      email: aiJson.email || '',
      gender: aiJson.gender || 0,
      highestDegree: aiJson.highestDegree || '',
      workYears: aiJson.workYears || 0,
      resumeFileUrl: response.data.url,
      aiParsedData: response.data.aiParsedData || '{}'
    }
  } else {
    ElMessage.error(response.msg || '上传失败')
  }
}

const handleUploadError = () => {
  uploading.value = false
  if (loadingInstance) loadingInstance.close()
  ElMessage.error('网络错误或上传失败')
}

const handleConfirmSave = async () => {
  if (!parsedData.value) return
  
  saving.value = true
  try {
    await candidateApi.create(parsedData.value)
    ElMessage.success('简历入库成功！')
    uploadDialogVisible.value = false
    fetchData()
  } catch (error) {
    ElMessage.error(error.response?.data?.msg || error.message || '保存失败。')
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  fetchData()
})
</script>

<style scoped>
.candidate-list-container {
  padding: 20px;
}
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}
.filter-card {
  margin-bottom: 20px;
}
.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}

/* AI 解析左侧预览 / 右侧校验 布局面板样式 */
.preview-panel, .parsed-form-panel {
  padding: 16px;
  background: rgba(255, 255, 255, 0.02);
  border-radius: 12px;
  height: 500px;
  display: flex;
  flex-direction: column;
}

.panel-head {
  margin-bottom: 12px;
  font-size: 15px;
  letter-spacing: 0.5px;
}

.resume-iframe {
  flex: 1;
  width: 100%;
  border: none;
  border-radius: 6px;
  background-color: #fff; /* PDF原件多为白底更护眼清楚 */
}

.parsed-form-panel .el-form {
  flex: 1;
  overflow-y: auto;
  padding-right: 10px;
}
</style>
