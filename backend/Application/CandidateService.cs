using Letiao.ATS.Api.Common;
using Letiao.ATS.Api.Domain;
using Letiao.ATS.Api.Repository;

namespace Letiao.ATS.Api.Application;

public class CandidateService(CandidateRepository candidateRepository)
{
    public async Task<int> CreateCandidateAsync(CandidateSaveRequest request, int? userId = null)
    {
        // 规则前置 (1.4 需求)：在创建候选人方法中直接写入防撞单逻辑
        var existing = await candidateRepository.GetByPhoneAsync(request.Phone);
        if (existing != null)
        {
            throw new BusinessException(409, "该候选人已存在（手机号冲突），请进行合并操作");
        }

        var candidate = new Candidate
        {
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            Gender = request.Gender,
            HighestDegree = request.HighestDegree,
            WorkYears = request.WorkYears,
            ResumeFileUrl = request.ResumeFileUrl,
            AiParsedData = request.AiParsedData,
            OwnerId = userId
        };
        return await candidateRepository.CreateAsync(candidate);
    }

    public Task<PagedResult<CandidateResponse>> GetFilteredCandidatesAsync(CandidateQueryRequest query) => candidateRepository.GetPagedListAsync(query);
}
