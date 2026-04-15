import http from './http'

export const jobApi = {
    // 获取职位分页列表
    getList(params) {
        return http.get('/api/jobs', { params })
    },
    // 获取职位详情
    getDetail(id) {
        return http.get(`/api/jobs/${id}`)
    },
    // 创建职位
    create(data) {
        return http.post('/api/jobs', data)
    },
    // 更新职位
    update(id, data) {
        return http.put(`/api/jobs/${id}`, data)
    }
}
