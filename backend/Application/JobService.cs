using Letiao.ATS.Api.Common;
using Letiao.ATS.Api.Domain;
using Letiao.ATS.Api.Repository;

namespace Letiao.ATS.Api.Application;

public class JobService(JobRepository jobRepository)
{
    public async Task<int> CreateJobAsync(JobSaveRequest request)
    {
        var job = new JobPosting
        {
            Title = request.Title,
            DepartmentName = request.DepartmentName,
            City = request.City,
            HeadcountTarget = request.HeadcountTarget,
            MinSalary = request.MinSalary,
            MaxSalary = request.MaxSalary,
            Description = request.Description,
            Status = request.Status,
            HrOwnerId = request.HrOwnerId
        };
        return await jobRepository.CreateAsync(job);
    }

    public async Task<bool> UpdateJobAsync(int id, JobSaveRequest request)
    {
        var existingJob = await jobRepository.GetByIdAsync(id);
        if (existingJob == null) return false;

        existingJob.Title = request.Title;
        existingJob.DepartmentName = request.DepartmentName;
        existingJob.City = request.City;
        existingJob.HeadcountTarget = request.HeadcountTarget;
        existingJob.MinSalary = request.MinSalary;
        existingJob.MaxSalary = request.MaxSalary;
        existingJob.Description = request.Description;
        existingJob.Status = request.Status;
        existingJob.HrOwnerId = request.HrOwnerId;

        return await jobRepository.UpdateAsync(existingJob);
    }

    public Task<JobResponse?> GetJobAsync(int id) => jobRepository.GetByIdAsync(id);

    public Task<PagedResult<JobResponse>> GetFilteredJobsAsync(JobQueryRequest query) => jobRepository.GetPagedListAsync(query);
}
