import http from './http'

export const candidateApi = {
    // 获取候选人分页列表
    getList(params) {
        return http.get('/api/candidates', { params })
    },
    // 创建候选人
    create(data) {
        return http.post('/api/candidates', data)
    }
}
