import http from './http'

export const pipelineApi = {
    getBoard(jobId) {
        return http.get(`/api/pipeline/jobs/${jobId}`)
    },
    addCandidate(data) {
        return http.post('/api/pipeline', data)
    },
    moveStage(id, targetStage) {
        return http.put(`/api/pipeline/${id}/move`, { targetStage })
    }
}
