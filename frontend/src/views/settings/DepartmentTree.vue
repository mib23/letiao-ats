<template>
  <div class="dept-container">
    <div class="page-header">
      <h2 class="title-font text-gradient">组织架构配置</h2>
    </div>

    <el-row :gutter="24">
      <!-- 树形控件区 -->
      <el-col :span="8">
        <el-card class="glass-panel hover-lift" style="height: 600px; overflow-y: auto;">
          <template #header>
            <div class="card-header" style="display: flex; justify-content: space-between;">
              <span>部门层级台账</span>
              <el-button type="primary" link @click="handleAddRoot">添加顶级部门</el-button>
            </div>
          </template>
          
          <el-tree
            :data="treeData"
            :props="defaultProps"
            node-key="id"
            default-expand-all
            :expand-on-click-node="false"
            highlight-current
            @node-click="handleNodeClick"
          >
            <template #default="{ node, data }">
              <span class="custom-tree-node" style="flex: 1; display: flex; align-items: center; justify-content: space-between; font-size: 14px; padding-right: 8px;">
                <span>{{ node.label }}</span>
                <span>
                  <el-button type="primary" link icon="Plus" size="small" @click.stop="append(data)"></el-button>
                  <el-button type="warning" link icon="Edit" size="small" @click.stop="edit(data)"></el-button>
                  <el-button type="danger" link icon="Delete" size="small" @click.stop="remove(node, data)"></el-button>
                </span>
              </span>
            </template>
          </el-tree>
        </el-card>
      </el-col>

      <!-- 员工映射区 -->
      <el-col :span="16">
        <el-card class="glass-panel hover-lift" style="height: 600px;">
          <template #header>
            <div class="card-header" style="display: flex; justify-content: space-between;">
              <span v-if="currentDept">【{{ currentDept.name }}】 名下人员名册</span>
              <span v-else>请在左侧选择一个部门以管理人员</span>
            </div>
          </template>
          
          <div v-if="currentDept">
            <el-form inline>
               <el-form-item label="分配现有员工">
                 <el-select v-model="selectedUserId" placeholder="选择要调入的员工账号" filterable style="width: 250px;">
                   <el-option v-for="u in allUsers" :key="u.id" :label="u.realName + ' (' + u.username + ')'" :value="u.id" />
                 </el-select>
               </el-form-item>
               <el-form-item>
                 <el-button type="primary" @click="handleAssignUser">确认划入该部门</el-button>
               </el-form-item>
            </el-form>

            <el-table :data="deptUsers" border style="margin-top: 15px;">
               <el-table-column prop="id" label="ID" width="80" />
               <el-table-column prop="realName" label="真实姓名" />
               <el-table-column prop="username" label="关联登录账号" />
               <el-table-column label="操作" width="100">
                 <template #default="{ row }">
                   <el-button type="danger" link @click="handleRemoveUser(row)">移除架构</el-button>
                 </template>
               </el-table-column>
            </el-table>
          </div>
          <el-empty v-else description="占位中，等待选择左侧数据树" />
        </el-card>
      </el-col>
    </el-row>

    <!-- 部门编辑弹窗 -->
    <el-dialog v-model="deptDialogVisible" :title="deptForm.id ? '编辑部门' : '新增部门'" width="400px" class="glass-panel">
      <el-form :model="deptForm" label-width="80px">
        <el-form-item label="部门名称" required>
          <el-input v-model="deptForm.name" />
        </el-form-item>
        <el-form-item label="同级排序">
          <el-input-number v-model="deptForm.orderNum" :min="0" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="deptDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="saveDept">确 定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { departmentApi } from '@/services/department.api'
import { Plus, Edit, Delete } from '@element-plus/icons-vue'

const treeData = ref([])
const defaultProps = { children: 'children', label: 'name' }
const currentDept = ref(null)
const deptUsers = ref([])
const allUsers = ref([])
const selectedUserId = ref(null)

const deptDialogVisible = ref(false)
const deptForm = ref({ id: null, name: '', parentId: 0, orderNum: 0 })

const loadTree = async () => {
  const res = await departmentApi.getTree()
  treeData.value = res.data
}

const loadAllUsers = async () => {
  const res = await departmentApi.getUsers(null)
  allUsers.value = res.data
}

const loadDeptUsers = async (deptId) => {
  const res = await departmentApi.getUsers(deptId)
  deptUsers.value = res.data
}

const handleNodeClick = (data) => {
  currentDept.value = data
  loadDeptUsers(data.id)
}

const handleAddRoot = () => {
  deptForm.value = { id: null, name: '', parentId: 0, orderNum: 0 }
  deptDialogVisible.value = true
}

const append = (data) => {
  deptForm.value = { id: null, name: '', parentId: data.id, orderNum: 0 }
  deptDialogVisible.value = true
}

const edit = (data) => {
  deptForm.value = { id: data.id, name: data.name, parentId: data.parentId, orderNum: data.orderNum }
  deptDialogVisible.value = true
}

const remove = (node, data) => {
  ElMessageBox.confirm('确定要删除该组织节点吗?', '警告', { type: 'warning' }).then(async () => {
    try {
      await departmentApi.delete(data.id)
      ElMessage.success('节点拔除成功')
      if (currentDept.value?.id === data.id) currentDept.value = null
      loadTree()
    } catch(e) {
      ElMessage.error('删除失败')
    }
  }).catch(() => {})
}

const saveDept = async () => {
  if (!deptForm.value.name) return ElMessage.warning('组织名称禁止为空')
  
  if (deptForm.value.id) {
    await departmentApi.update(deptForm.value.id, deptForm.value)
  } else {
    await departmentApi.create(deptForm.value)
  }
  
  ElMessage.success('组织节点保存落库成功')
  deptDialogVisible.value = false
  loadTree()
}

const handleAssignUser = async () => {
  if (!selectedUserId.value) return ElMessage.warning('请选择需要纳入管辖的人员字典')
  if (!currentDept.value) return
  
  await departmentApi.assignUser(selectedUserId.value, { 
    departmentId: currentDept.value.id, 
    departmentName: currentDept.value.name 
  })
  ElMessage.success('HR 账号归属部划分成功！')
  selectedUserId.value = null
  loadDeptUsers(currentDept.value.id)
}

const handleRemoveUser = (row) => {
  ElMessageBox.confirm('是否将该员工解除该部门管辖圈?', '提示').then(async () => {
    await departmentApi.assignUser(row.id, { departmentId: 0, departmentName: '' })
    ElMessage.success('已清空其组织归属状态')
    loadDeptUsers(currentDept.value.id)
  }).catch(() => {})
}

onMounted(() => {
  loadTree()
  loadAllUsers()
})
</script>

<style scoped>
.dept-container {
  padding: 20px;
}
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

:deep(.el-tree) {
  background: transparent !important;
  color: var(--text-main);
}
:deep(.el-tree-node__content:hover) {
  background: rgba(99, 102, 241, 0.15) !important;
}
:deep(.el-tree-node:focus > .el-tree-node__content) {
  background: rgba(255, 255, 255, 0.05) !important;
}
</style>
