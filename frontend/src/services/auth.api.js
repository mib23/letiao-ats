import http from '@/services/http'

export const authApi = {
  /** 账号密码登录 */
  login: (username, password) =>
    http.post('/api/auth/login', { username, password }),
}
