using Microsoft.EntityFrameworkCore;
using RecruitmentPlatformAPI.Data;
using RecruitmentPlatformAPI.DTOs.JobSeeker;
using RecruitmentPlatformAPI.Enums;
using RecruitmentPlatformAPI.Models.JobSeeker;

namespace RecruitmentPlatformAPI.Services.JobSeeker
{
    public class JobSeekerSkillService : IJobSeekerSkillService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<JobSeekerSkillService> _logger;

        public JobSeekerSkillService(AppDbContext context, ILogger<JobSeekerSkillService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SkillsResponseDto> GetSkillsAsync(int userId)
        {
            try
            {
                var jobSeeker = await GetJobSeekerAsync(userId);
                if (jobSeeker == null)
                    return SkillsResponseDto.FailureResult("Only job seekers can access skills");

                var skills = await _context.JobSeekerSkills
                    .Where(js => js.JobSeekerId == jobSeeker.Id)
                    .Include(js => js.Skill)
                    .OrderBy(js => js.Skill.Name)
                    .Select(js => new SkillDto
                    {
                        Id = js.Skill.Id,
                        Name = js.Skill.Name
                    })
                    .ToListAsync();

                return SkillsResponseDto.SuccessResult(skills,
                    skills.Count > 0 ? $"Found {skills.Count} skill(s)" : "No skills assigned");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting skills for user {UserId}", userId);
                return SkillsResponseDto.FailureResult("An error occurred");
            }
        }

        public async Task<SkillsResponseDto> UpdateSkillsAsync(int userId, UpdateSkillsRequestDto dto)
        {
            try
            {
                var jobSeeker = await GetJobSeekerAsync(userId);
                if (jobSeeker == null)
                    return SkillsResponseDto.FailureResult("Only job seekers can update skills");

                // Deduplicate
                var requestedIds = dto.SkillIds.Distinct().ToList();

                // Validate all skill IDs exist
                var validSkills = await _context.Skills
                    .Where(s => requestedIds.Contains(s.Id))
                    .ToListAsync();

                if (validSkills.Count != requestedIds.Count)
                {
                    var invalidIds = requestedIds.Except(validSkills.Select(s => s.Id));
                    return SkillsResponseDto.FailureResult($"Invalid skill IDs: {string.Join(", ", invalidIds)}");
                }

                // Remove all existing skills
                var existing = await _context.JobSeekerSkills
                    .Where(js => js.JobSeekerId == jobSeeker.Id)
                    .ToListAsync();
                _context.JobSeekerSkills.RemoveRange(existing);

                // Add new skills
                var newSkills = requestedIds.Select(skillId => new JobSeekerSkill
                {
                    JobSeekerId = jobSeeker.Id,
                    SkillId = skillId,
                    Source = "Self"
                }).ToList();

                _context.JobSeekerSkills.AddRange(newSkills);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated {Count} skills for user {UserId}", requestedIds.Count, userId);

                // Return the updated skill list
                var result = validSkills
                    .OrderBy(s => s.Name)
                    .Select(s => new SkillDto { Id = s.Id, Name = s.Name })
                    .ToList();

                return SkillsResponseDto.SuccessResult(result, $"Skills updated successfully ({result.Count} skill(s))");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating skills for user {UserId}", userId);
                return SkillsResponseDto.FailureResult("An error occurred while updating skills");
            }
        }

        public async Task<SkillsResponseDto> ClearSkillsAsync(int userId)
        {
            try
            {
                var jobSeeker = await GetJobSeekerAsync(userId);
                if (jobSeeker == null)
                    return SkillsResponseDto.FailureResult("Only job seekers can manage skills");

                var existing = await _context.JobSeekerSkills
                    .Where(js => js.JobSeekerId == jobSeeker.Id)
                    .ToListAsync();

                if (existing.Count == 0)
                    return SkillsResponseDto.SuccessResult(new List<SkillDto>(), "No skills to remove");

                _context.JobSeekerSkills.RemoveRange(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cleared all skills for user {UserId}", userId);
                return SkillsResponseDto.SuccessResult(new List<SkillDto>(), "All skills removed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing skills for user {UserId}", userId);
                return SkillsResponseDto.FailureResult("An error occurred while clearing skills");
            }
        }

        public async Task<List<SkillDto>> GetAvailableSkillsAsync()
        {
            return await _context.Skills
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new SkillDto { Id = s.Id, Name = s.Name })
                .ToListAsync();
        }

        private async Task<Models.JobSeeker.JobSeeker?> GetJobSeekerAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.AccountType != AccountType.JobSeeker)
                return null;

            return await _context.JobSeekers.FirstOrDefaultAsync(j => j.UserId == userId);
        }
    }
}
