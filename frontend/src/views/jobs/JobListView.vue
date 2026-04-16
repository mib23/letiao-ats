<template>
  <div class="job-list-container">
    <div class="page-header">
      <h2 class="title-font text-gradient">职位管理</h2>
      <el-button type="primary" class="hover-lift" @click="handleCreate">发布新职位</el-button>
    </div>

    <!-- 筛选区 -->
    <el-card class="filter-card glass-panel hover-lift">
      <el-form inline :model="queryParams">
        <el-form-item label="职位名称">
          <el-input v-model="queryParams.keyword" placeholder="搜索关键词..." clearable />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryParams.status" placeholder="全部" clearable>
            <el-option label="招聘中" value="PUBLISHED" />
            <el-option label="暂停" value="PAUSED" />
            <el-option label="已关停" value="CLOSED" />
            <el-option label="草稿" value="DRAFT" />
          </el-select>
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
        <el-table-column prop="title" label="职位名称" />
        <el-table-column prop="departmentName" label="用人部门" width="150" />
        <el-table-column prop="city" label="地点" width="120" />
        <el-table-column label="薪资(千)" width="120">
          <template #default="{ row }">
            {{ row.minSalary }}k - {{ row.maxSalary }}k
          </template>
        </el-table-column>
        <el-table-column prop="headcountTarget" label="HC预算" width="100" />
        <el-table-column prop="hrOwnerName" label="负责HR" width="120" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">{{ getStatusLabel(row.status) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="$router.push(`/jobs/${row.id}/kanban`)">看板</el-button>
            <el-button link type="primary" @click="handleEdit(row)">编辑</el-button>
            <el-button link :type="row.status === 'PUBLISHED' ? 'warning' : 'success'" @click="handleToggleStatus(row)">
               {{ row.status === 'PUBLISHED' ? '暂停' : '发布' }}
            </el-button>
          </template>
        </el-table-column>
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

    <!-- 职位表单弹窗 -->
    <JobFormDialog ref="formDialogRef" @success="fetchData" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { jobApi } from '@/services/job.api'
import JobFormDialog from './components/JobFormDialog.vue'

const queryParams = ref({
  keyword: '',
  status: '',
  page: 1,
  pageSize: 10
})

const loading = ref(false)
const tableData = ref([])
const total = ref(0)
const formDialogRef = ref(null)

const getStatusType = (status) => {
  const map = { PUBLISHED: 'success', PAUSED: 'warning', CLOSED: 'info', DRAFT: '' }
  return map[status] || ''
}

const getStatusLabel = (status) => {
  const map = { PUBLISHED: '招聘中', PAUSED: '已暂停', CLOSED: '已关停', DRAFT: '草稿' }
  return map[status] || status
}

const fetchData = async () => {
  loading.value = true
  try {
    const res = await jobApi.getList(queryParams.value)
    tableData.value = res.data.list
    total.value = res.data.total
  } catch (error) {
    console.error(error)
  } finally {
    loading.value = false
  }
}

const handleCreate = () => {
  formDialogRef.value?.open()
}

const handleEdit = (row) => {
  formDialogRef.value?.open(row)
}

const handleToggleStatus = async (row) => {
  const newStatus = row.status === 'PUBLISHED' ? 'PAUSED' : 'PUBLISHED'
  try {
    await jobApi.update(row.id, { ...row, status: newStatus })
    ElMessage.success('操作成功')
    fetchData()
  } catch (error) {
    console.error(error)
  }
}

onMounted(() => {
  fetchData()
})
</script>

<style scoped>
.job-list-container {
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
