<template>
  <el-dialog
    :title="isEdit ? '编辑职位' : '发布新职位'"
    v-model="visible"
    width="650px"
    @close="handleClose"
  >
    <el-form ref="formRef" :model="formData" :rules="rules" label-width="100px">
      <el-form-item label="职位名称" prop="title">
        <el-input v-model="formData.title" placeholder="如：高级Java开发工程师" />
      </el-form-item>
      
      <el-row>
        <el-col :span="12">
          <el-form-item label="用人部门" prop="departmentName">
            <el-input v-model="formData.departmentName" placeholder="如：研发中心" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="工作地点" prop="city">
            <el-input v-model="formData.city" placeholder="如：北京" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="12">
          <el-form-item label="薪资范围" prop="minSalary">
            <div style="display: flex; align-items: center;">
              <el-input-number v-model="formData.minSalary" :min="0" :controls="false" style="width: 80px" />
              <span style="margin: 0 10px;">k -</span>
              <el-input-number v-model="formData.maxSalary" :min="0" :controls="false" style="width: 80px" />
              <span style="margin-left: 10px;">k</span>
            </div>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="HC预算" prop="headcountTarget">
            <el-input-number v-model="formData.headcountTarget" :min="1" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="状态" prop="status">
        <el-radio-group v-model="formData.status" >
          <el-radio label="PUBLISHED" value="PUBLISHED">招聘中</el-radio>
          <el-radio label="DRAFT" value="DRAFT">存为草稿</el-radio>
          <el-radio label="PAUSED" value="PAUSED">暂停</el-radio>
        </el-radio-group>
      </el-form-item>

      <el-form-item label="JD详情" prop="description">
        <el-input type="textarea" v-model="formData.description" :rows="5" placeholder="职位描述和要求..." />
      </el-form-item>
    </el-form>
    
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="visible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="submitForm">确定</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { ElMessage } from 'element-plus'
import { jobApi } from '@/services/job.api'

const emit = defineEmits(['success'])

const visible = ref(false)
const isEdit = ref(false)
const submitting = ref(false)
const formRef = ref(null)

const defaultForm = {
  id: undefined,
  title: '',
  departmentName: '',
  city: '',
  headcountTarget: 1,
  minSalary: 10,
  maxSalary: 20,
  status: 'PUBLISHED',
  description: '',
  hrOwnerId: 0
}

const formData = reactive({ ...defaultForm })

const rules = {
  title: [{ required: true, message: '请输入职位名称', trigger: 'blur' }],
  departmentName: [{ required: true, message: '请输入部门名称', trigger: 'blur' }],
  city: [{ required: true, message: '请输入工作地点', trigger: 'blur' }],
  description: [{ required: true, message: '请输入JD详情', trigger: 'blur' }]
}

const open = (row) => {
  if (row) {
    isEdit.value = true
    Object.assign(formData, row)
  } else {
    isEdit.value = false
    Object.assign(formData, defaultForm)
  }
  visible.value = true
}

const handleClose = () => {
  formRef.value?.resetFields()
}

const submitForm = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      submitting.value = true
      try {
        if (isEdit.value) {
          await jobApi.update(formData.id, formData)
          ElMessage.success('更新成功')
        } else {
          await jobApi.create(formData)
          ElMessage.success('发布成功')
        }
        visible.value = false
        emit('success')
      } catch (error) {
        console.error(error)
      } finally {
        submitting.value = false
      }
    }
  })
}

defineExpose({ open })
</script>
