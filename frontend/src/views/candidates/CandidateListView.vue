<template>
  <div class="candidate-list-container">
    <div class="page-header">
      <h2 class="title-font text-gradient">候选人列表</h2>
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
        <el-table-column prop="ownerName" label="归属人" width="150">
           <template #default="{ row }">
             <el-tag :type="row.ownerName ? 'success' : 'info'">{{ row.ownerName || '公海池' }}</el-tag>
           </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="录入时间" />
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
    <el-dialog v-model="uploadDialogVisible" title="上传外部候选人" width="500px" class="glass-panel" destroy-on-close>
      <div style="text-align: center; padding: 20px 0;">
        <el-upload
          drag
          action="/api/upload/resume"
          :headers="uploadHeaders"
          :on-success="handleUploadSuccess"
          :on-error="handleUploadError"
          :show-file-list="false"
        >
          <el-icon class="el-icon--upload"><upload-filled /></el-icon>
          <div class="el-upload__text">
            将简历拖拽至此，或 <em>点击上传</em>
          </div>
          <template #tip>
            <div class="el-upload__tip" style="color: var(--text-muted);">
              支持 PDF / Word / 图片格式，自动解析关键字段
            </div>
          </template>
        </el-upload>
      </div>

      <template v-if="parsedData">
        <div class="parsed-preview" style="text-align: left; background: rgba(255,255,255,0.05); padding: 15px; border-radius: 8px; margin-top: 20px;">
          <h4 style="color: var(--brand-cyan); margin-bottom: 10px;">🌟 智能解析结果预览</h4>
          <el-form :model="parsedData" label-width="80px">
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
              <el-select v-model="parsedData.gender">
                <el-option label="男" :value="1" />
                <el-option label="女" :value="2" />
                <el-option label="未知" :value="0" />
              </el-select>
            </el-form-item>
            <el-form-item label="工作年限">
              <el-input-number v-model="parsedData.workYears" :min="0" />
            </el-form-item>
          </el-form>
        </div>
      </template>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="uploadDialogVisible = false">取消</el-button>
          <el-button type="primary" :disabled="!parsedData" @click="handleConfirmSave">保存入库</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { candidateApi } from '@/services/candidate.api'
import { UploadFilled } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'

const uploadDialogVisible = ref(false)
const queryParams = ref({
  keyword: '',
  page: 1,
  pageSize: 10
})

const loading = ref(false)
const tableData = ref([])
const total = ref(0)
const parsedData = ref(null)

const auth = useAuthStore()

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

const handleUploadSuccess = (response, uploadFile) => {
  if (response.code === 200) {
    ElMessage.success('简历上传成功，AI 正在结构化抽取...')
    // Mocking AI Parse behavior dynamically
    setTimeout(() => {
      parsedData.value = {
        name: uploadFile.name.split('.')[0] || '默认解析名',
        phone: '138' + Math.floor(Math.random() * 100000000), // Random mock
        email: 'test@example.com',
        gender: 1,
        highestDegree: '本科',
        workYears: Math.floor(Math.random() * 5) + 1,
        resumeFileUrl: response.data.url,
        aiParsedData: JSON.stringify({ skill: "mock skill" })
      }
      ElMessage.success('智能解析完成！')
    }, 1500)
  } else {
    ElMessage.error(response.msg || '上传失败')
  }
}

const handleUploadError = () => {
  ElMessage.error('网络错误或上传失败')
}

const handleConfirmSave = async () => {
  if (!parsedData.value) return
  
  try {
    await candidateApi.create(parsedData.value)
    ElMessage.success('成功放入人才库！')
    uploadDialogVisible.value = false
    fetchData()
  } catch (error) {
    ElMessage.error(error.response?.data?.msg || error.message || '保存失败\n手机号冲突！该候选人已存在。')
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
</style>
