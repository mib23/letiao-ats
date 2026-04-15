import axios from 'axios'
import { ElMessage } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import router from '@/router'

const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
  timeout: 15000
})

// ─── 请求拦截：自动埋入 JWT ───────────────────────────────────
http.interceptors.request.use(config => {
  const auth = useAuthStore()
  if (auth.token) {
    config.headers['Authorization'] = `Bearer ${auth.token}`
  }
  return config
})

// ─── 响应拦截：统一错误处理 ───────────────────────────────────
http.interceptors.response.use(
  res => {
    const data = res.data
    // 后端业务错误（HTTP 200 但 code != 200）
    if (data && data.code && data.code !== 200) {
      ElMessage.error(data.msg || '操作失败')
      return Promise.reject(new Error(data.msg))
    }
    return data
  },
  err => {
    const status = err.response?.status
    if (status === 401) {
      ElMessage.error('登录已过期，请重新登录')
      const auth = useAuthStore()
      auth.clearAuth()
      router.push('/login')
    } else if (status === 403) {
      ElMessage.error('您没有权限执行此操作')
    } else if (status === 503) {
      ElMessage.error('服务暂时不可用，请稍后重试')
    } else {
      ElMessage.error(err.response?.data?.msg || err.message || '网络错误')
    }
    return Promise.reject(err)
  }
)

export default http
