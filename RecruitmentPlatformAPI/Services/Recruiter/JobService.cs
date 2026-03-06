using Microsoft.EntityFrameworkCore;
using RecruitmentPlatformAPI.Data;
using RecruitmentPlatformAPI.DTOs.Recruiter;
using RecruitmentPlatformAPI.Enums;
using RecruitmentPlatformAPI.Models.Jobs;

namespace RecruitmentPlatformAPI.Services.Recruiter
{
    public class JobService : IJobService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<JobService> _logger;

        public JobService(AppDbContext context, ILogger<JobService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════════
        //  JOB CRUD
        // ═══════════════════════════════════════════════════════════

        public async Task<JobResponseDto?> CreateJobAsync(int userId, JobRequestDto dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.AccountType != AccountType.Recruiter)
                {
                    _logger.LogWarning("CreateJob failed: User {UserId} is not a recruiter", userId);
                    return null;
                }

                var recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);
                if (recruiter == null)
                {
                    _logger.LogWarning("CreateJob failed: No recruiter profile for user {UserId}", userId);
                    return null;
                }

                // Validate skill IDs if provided
                if (dto.SkillIds != null && dto.SkillIds.Count > 0)
                {
                    var validSkillIds = await _context.Skills
                        .Where(s => dto.SkillIds.Contains(s.Id))
                        .Select(s => s.Id)
                        .ToListAsync();

                    if (validSkillIds.Count != dto.SkillIds.Distinct().Count())
                    {
                        _logger.LogWarning("CreateJob failed: Invalid skill IDs provided by user {UserId}", userId);
                        return null;
                    }
                }

                var job = new Job
                {
                    RecruiterId = recruiter.Id,
                    Title = dto.Title.Trim(),
                    Description = dto.Description.Trim(),
                    Requirements = dto.Requirements.Trim(),
                    EmploymentType = dto.EmploymentType,
                    MinYearsOfExperience = dto.MinYearsOfExperience,
                    Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim(),
                    PostedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();

                // Add skills if provided
                if (dto.SkillIds != null && dto.SkillIds.Count > 0)
                {
                    var jobSkills = dto.SkillIds.Distinct().Select(skillId => new JobSkill
                    {
                        JobId = job.Id,
                        SkillId = skillId
                    }).ToList();

                    _context.JobSkills.AddRange(jobSkills);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Job {JobId} created by user {UserId}", job.Id, userId);
                return await BuildJobResponseDto(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job for user {UserId}", userId);
                return null;
            }
        }

        public async Task<JobResponseDto?> UpdateJobAsync(int userId, int jobId, JobRequestDto dto)
        {
            try
            {
                var job = await GetOwnedJobAsync(userId, jobId);
                if (job == null) return null;

                // Validate skill IDs if provided
                if (dto.SkillIds != null && dto.SkillIds.Count > 0)
                {
                    var validSkillIds = await _context.Skills
                        .Where(s => dto.SkillIds.Contains(s.Id))
                        .Select(s => s.Id)
                        .ToListAsync();

                    if (validSkillIds.Count != dto.SkillIds.Distinct().Count())
                    {
                        _logger.LogWarning("UpdateJob failed: Invalid skill IDs for job {JobId} by user {UserId}", jobId, userId);
                        return null;
                    }
                }

                // Update job fields
                job.Title = dto.Title.Trim();
                job.Description = dto.Description.Trim();
                job.Requirements = dto.Requirements.Trim();
                job.EmploymentType = dto.EmploymentType;
                job.MinYearsOfExperience = dto.MinYearsOfExperience;
                job.Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim();
                job.UpdatedAt = DateTime.UtcNow;

                // Replace skills: remove existing, add new
                var existingSkills = await _context.JobSkills
                    .Where(js => js.JobId == job.Id)
                    .ToListAsync();
                _context.JobSkills.RemoveRange(existingSkills);

                if (dto.SkillIds != null && dto.SkillIds.Count > 0)
                {
                    var jobSkills = dto.SkillIds.Distinct().Select(skillId => new JobSkill
                    {
                        JobId = job.Id,
                        SkillId = skillId
                    }).ToList();

                    _context.JobSkills.AddRange(jobSkills);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Job {JobId} updated by user {UserId}", jobId, userId);
                return await BuildJobResponseDto(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job {JobId} for user {UserId}", jobId, userId);
                return null;
            }
        }

        public async Task<bool> DeactivateJobAsync(int userId, int jobId)
        {
            try
            {
                var job = await GetOwnedJobAsync(userId, jobId);
                if (job == null) return false;

                job.IsActive = false;
                job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Job {JobId} deactivated by user {UserId}", jobId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating job {JobId} for user {UserId}", jobId, userId);
                return false;
            }
        }

        public async Task<bool> ReactivateJobAsync(int userId, int jobId)
        {
            try
            {
                var job = await GetOwnedJobAsync(userId, jobId);
                if (job == null) return false;

                job.IsActive = true;
                job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Job {JobId} reactivated by user {UserId}", jobId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reactivating job {JobId} for user {UserId}", jobId, userId);
                return false;
            }
        }

        public async Task<bool> DeleteJobAsync(int userId, int jobId)
        {
            try
            {
                var job = await GetOwnedJobAsync(userId, jobId);
                if (job == null) return false;

                // Hard delete — cascade removes JobSkills and Recommendations automatically
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Job {JobId} permanently deleted by user {UserId}", jobId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job {JobId} for user {UserId}", jobId, userId);
                return false;
            }
        }

        public async Task<JobListResponseDto?> GetMyJobsAsync(int userId, int page = 1, int pageSize = 10, bool? isActive = null)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.AccountType != AccountType.Recruiter)
                {
                    _logger.LogWarning("GetMyJobs denied: User {UserId} is not a recruiter", userId);
                    return null;
                }

                var recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);
                if (recruiter == null)
                    return new JobListResponseDto { Page = page, PageSize = pageSize };

                var query = _context.Jobs.Where(j => j.RecruiterId == recruiter.Id);
                if (isActive.HasValue) query = query.Where(j => j.IsActive == isActive.Value);

                var totalCount = await query.CountAsync();
                var jobs = await query
                    .OrderByDescending(j => j.PostedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = new List<JobResponseDto>();
                foreach (var job in jobs)
                    dtos.Add(await BuildJobResponseDto(job));

                return new JobListResponseDto
                {
                    Jobs = dtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting jobs for user {UserId}", userId);
                return new JobListResponseDto { Page = page, PageSize = pageSize };
            }
        }

        public async Task<JobResponseDto?> GetJobByIdAsync(int userId, int jobId)
        {
            try
            {
                var job = await GetOwnedJobAsync(userId, jobId);
                if (job == null) return null;
                return await BuildJobResponseDto(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting job {JobId} for user {UserId}", jobId, userId);
                return null;
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  REFERENCE DATA
        // ═══════════════════════════════════════════════════════════

        public async Task<List<SkillOptionDto>> GetSkillsAsync(string? search = null)
        {
            try
            {
                var query = _context.Skills.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchLower = search.Trim().ToLower();
                    query = query.Where(s => s.Name.ToLower().Contains(searchLower));
                }

                return await query
                    .OrderBy(s => s.Name)
                    .Select(s => new SkillOptionDto { Id = s.Id, Name = s.Name })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting skills list");
                return new List<SkillOptionDto>();
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  PRIVATE HELPERS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Returns a job only if it belongs to the recruiter account linked to userId.
        /// Returns null if not found or if ownership check fails.
        /// </summary>
        private async Task<Job?> GetOwnedJobAsync(int userId, int jobId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.AccountType != AccountType.Recruiter)
                return null;

            var recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);
            if (recruiter == null) return null;

            return await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId && j.RecruiterId == recruiter.Id);
        }

        /// <summary>
        /// Builds a full JobResponseDto including skills.
        /// </summary>
        private async Task<JobResponseDto> BuildJobResponseDto(Job job)
        {
            var skills = await _context.JobSkills
                .Where(js => js.JobId == job.Id)
                .Join(_context.Skills, js => js.SkillId, s => s.Id,
                    (js, s) => new JobSkillDto { Id = s.Id, Name = s.Name })
                .ToListAsync();

            return new JobResponseDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                EmploymentType = job.EmploymentType,
                MinYearsOfExperience = job.MinYearsOfExperience,
                Location = job.Location,
                PostedAt = job.PostedAt,
                UpdatedAt = job.UpdatedAt,
                IsActive = job.IsActive,
                CandidateCount = 0, // Reserved for future AI matching module
                Skills = skills
            };
        }
    }
}
