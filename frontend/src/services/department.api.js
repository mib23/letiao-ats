import http from './http'

export const departmentApi = {
    getTree() {
        return http.get('/api/departments/tree')
    },
    create(data) {
        return http.post('/api/departments', data)
    },
    update(id, data) {
        return http.put(`/api/departments/${id}`, data)
    },
    delete(id) {
        return http.delete(`/api/departments/${id}`)
    },
    getUsers(departmentId) {
        return http.get('/api/users', { params: { departmentId } })
    },
    assignUser(userId, data) {
        return http.put(`/api/users/${userId}/department`, data)
    }
}
