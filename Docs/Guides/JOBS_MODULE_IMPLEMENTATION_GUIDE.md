# Jobs Module — Implementation Guide

> **For:** Backend team member  
> **Date:** February 2026  
> **Purpose:** Step-by-step guide to implement the Jobs module, consistent with existing codebase patterns  
> **Estimated effort:** 2–3 days

---

## Table of Contents

1. [Business Flow](#1-business-flow)
2. [What Already Exists](#2-what-already-exists)
3. [DTOs](#3-dtos)
4. [Service Interface](#4-service-interface)
5. [Service Implementation](#5-service-implementation)
6. [Controller](#6-controller)
7. [Registration in Program.cs](#7-registration-in-programcs)
8. [Database Migration](#8-database-migration)
9. [API Endpoints Summary](#9-api-endpoints-summary)
10. [Validation Rules](#10-validation-rules)
11. [Testing Checklist](#11-testing-checklist)
12. [Code Conventions Cheat-Sheet](#12-code-conventions-cheat-sheet)

---

## 1. Business Flow

> **Important:** JobSeekers do NOT browse, see, or apply to jobs. The platform uses a silent AI matching system.

```
Recruiter posts job
       │
       ▼
AI engine generates Recommendation records
(compares each JobSeeker's profile vs job requirements → MatchScore)
       │
       ▼
Recruiter views ranked candidate list for their job
(GET /api/jobs/{id}/candidates  →  shows JobSeekers ordered by MatchScore)
       │
       ▼
Recruiter contacts promising candidates (out of scope for this module)
```

### Key business rules

| Rule | Detail |
|------|--------|
| Who can create/edit/delete jobs? | Only authenticated **Recruiter** accounts |
| Who can view jobs? | Only the **owning Recruiter** (their own postings) |
| Who can see candidates? | Only the **owning Recruiter** |
| JobSeekers | Cannot see jobs and are unaware they appear as candidates |
| Skills | A job can require 0–15 skills from the `Skills` table |
| Soft deactivate | `IsActive = false` hides a job instead of hard-deleting it |
| Ownership guard | A recruiter can only edit/delete/view candidates for **their own** jobs |

---

## 2. What Already Exists

### Models (no changes needed)

The database models are organized into these folders:

```
Models/
├── Identity/      # User, EmailVerification, PasswordReset
├── JobSeeker/     # JobSeeker profile entities
├── Recruiter/     # Recruiter profile entity
├── Jobs/          # ← Job-related business entities (CORRECT location)
│   ├── Job.cs
│   ├── JobSkill.cs
│   └── Recommendation.cs
├── Reference/     # ← True lookup/seed data only
│   ├── Country.cs
│   ├── JobTitle.cs
│   ├── Language.cs
│   └── Skill.cs
└── Assessment/    # AssessmentQuestion, AssessmentAttempt, AssessmentAnswer
```

> `Job.cs`, `JobSkill.cs`, `Recommendation.cs` are in **`Models/Jobs/`** with namespace `RecruitmentPlatformAPI.Models.Jobs`.

### DbContext has these DbSets already

```csharp
public DbSet<Job> Jobs { get; set; }
public DbSet<JobSkill> JobSkills { get; set; }
public DbSet<Recommendation> Recommendations { get; set; }
public DbSet<Skill> Skills { get; set; }
```

All table relationships (cascade rules, indexes, precision constraints) are already in `AppDbContext.OnModelCreating`. **No migration is needed.**

### Existing `Job` model (for reference)

```csharp
// Models/Jobs/Job.cs — namespace RecruitmentPlatformAPI.Models.Jobs
public class Job
{
    public int Id { get; set; }
    public int RecruiterId { get; set; }           // FK → Recruiter
    public string Title { get; set; }               // max 150
    public string Description { get; set; }         // max 1200
    public string Requirements { get; set; }        // max 1200
    public string? EmploymentType { get; set; }     // max 50, stored as string
    public int MinYearsOfExperience { get; set; }
    public string? Location { get; set; }           // max 100
    public DateTime PostedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public RecruiterEntity Recruiter { get; set; }  // navigation
}
```

### Existing `Recommendation` model (for reference)

```csharp
// Models/Jobs/Recommendation.cs — namespace RecruitmentPlatformAPI.Models.Jobs
public class Recommendation
{
    public int Id { get; set; }
    public int JobId { get; set; }           // FK → Job
    public int JobSeekerId { get; set; }     // FK → JobSeeker
    public decimal MatchScore { get; set; }  // 0.00 – 100.00  (precision 5,2)
    public DateTime GeneratedAt { get; set; }
    public Job Job { get; set; }
    public JobSeeker JobSeeker { get; set; }
}
```

---

## 3. DTOs

Create file: **`DTOs/Recruiter/JobDtos.cs`**

```csharp
using System.ComponentModel.DataAnnotations;

namespace RecruitmentPlatformAPI.DTOs.Recruiter
{
    // ─────────────────────────────────────────────
    // REQUEST DTOs
    // ─────────────────────────────────────────────

    /// <summary>
    /// Request DTO for creating or updating a job posting
    /// </summary>
    public class JobRequestDto
    {
        /// <summary>Job title / position name</summary>
        /// <example>Senior Backend Developer</example>
        [Required(ErrorMessage = "Job title is required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Job title must be between 3 and 150 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>Detailed job description and responsibilities</summary>
        /// <example>We are looking for an experienced backend developer...</example>
        [Required(ErrorMessage = "Job description is required")]
        [StringLength(1200, MinimumLength = 20, ErrorMessage = "Description must be between 20 and 1200 characters")]
        public string Description { get; set; } = string.Empty;

        /// <summary>Qualifications and requirements for the role</summary>
        /// <example>3+ years of experience with C# and .NET, strong SQL skills...</example>
        [Required(ErrorMessage = "Requirements are required")]
        [StringLength(1200, MinimumLength = 20, ErrorMessage = "Requirements must be between 20 and 1200 characters")]
        public string Requirements { get; set; } = string.Empty;

        /// <summary>
        /// Employment type. Allowed values: FullTime, PartTime, Freelance, Internship
        /// </summary>
        /// <example>FullTime</example>
        [Required(ErrorMessage = "Employment type is required")]
        [RegularExpression(@"^(FullTime|PartTime|Freelance|Internship)$",
            ErrorMessage = "Invalid employment type. Allowed: FullTime, PartTime, Freelance, Internship")]
        public string EmploymentType { get; set; } = string.Empty;

        /// <summary>Minimum years of experience (0 = entry-level)</summary>
        /// <example>3</example>
        [Required(ErrorMessage = "Minimum years of experience is required")]
        [Range(0, 30, ErrorMessage = "Years of experience must be between 0 and 30")]
        public int MinYearsOfExperience { get; set; }

        /// <summary>Job location. Use "Remote" for remote positions.</summary>
        /// <example>Cairo, Egypt</example>
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string? Location { get; set; }

        /// <summary>
        /// IDs of required skills from the Skills table. Max 15.
        /// Fetch available IDs via GET /api/jobs/skills
        /// </summary>
        /// <example>[1, 5, 12]</example>
        [MaxLength(15, ErrorMessage = "A job can have at most 15 required skills")]
        public List<int>? SkillIds { get; set; }
    }

    // ─────────────────────────────────────────────
    // RESPONSE DTOs
    // ─────────────────────────────────────────────

    /// <summary>
    /// Response DTO for a job posting (recruiter view)
    /// </summary>
    public class JobResponseDto
    {
        /// <summary>Unique job ID</summary>
        public int Id { get; set; }
        /// <summary>Job title</summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>Job description</summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>Requirements and qualifications</summary>
        public string Requirements { get; set; } = string.Empty;
        /// <summary>Employment type</summary>
        public string EmploymentType { get; set; } = string.Empty;
        /// <summary>Minimum years of experience</summary>
        public int MinYearsOfExperience { get; set; }
        /// <summary>Job location</summary>
        public string? Location { get; set; }
        /// <summary>When the job was posted</summary>
        public DateTime PostedAt { get; set; }
        /// <summary>When the job was last updated</summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>Whether the listing is currently active</summary>
        public bool IsActive { get; set; }
        /// <summary>Number of AI-matched candidates (from Recommendation table)</summary>
        public int CandidateCount { get; set; }
        /// <summary>Required skills for this job</summary>
        public List<JobSkillDto> Skills { get; set; } = new();
    }

    /// <summary>
    /// Skill information attached to a job
    /// </summary>
    public class JobSkillDto
    {
        /// <summary>Skill ID</summary>
        public int Id { get; set; }
        /// <summary>Skill name</summary>
        /// <example>C#</example>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Paginated list of the recruiter's job postings
    /// </summary>
    public class JobListResponseDto
    {
        public List<JobResponseDto> Jobs { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// A single AI-matched candidate returned by GET /api/jobs/{id}/candidates
    /// </summary>
    public class JobCandidateDto
    {
        // ── Match info ──
        /// <summary>AI-computed match score (0.00 – 100.00). Higher = better fit.</summary>
        public decimal MatchScore { get; set; }
        /// <summary>When the AI generated this recommendation</summary>
        public DateTime GeneratedAt { get; set; }

        // ── JobSeeker profile ──
        /// <summary>JobSeeker record ID</summary>
        public int JobSeekerId { get; set; }
        /// <summary>Full name (FirstName + LastName from User table)</summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary>Profile picture URL</summary>
        public string? ProfilePictureUrl { get; set; }
        /// <summary>Desired job title / role</summary>
        public string? JobTitle { get; set; }
        /// <summary>Total years of experience</summary>
        public int? YearsOfExperience { get; set; }
        /// <summary>City</summary>
        public string? City { get; set; }
        /// <summary>Country name</summary>
        public string? Country { get; set; }
        /// <summary>Brief bio</summary>
        public string? Bio { get; set; }
        /// <summary>Candidate's skills (useful for overlap display on the frontend)</summary>
        public List<string> Skills { get; set; } = new();
    }

    /// <summary>
    /// Paginated list of AI-matched candidates for a job
    /// </summary>
    public class JobCandidateListResponseDto
    {
        public List<JobCandidateDto> Candidates { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// Available skill option for the job creation form dropdown
    /// </summary>
    public class SkillOptionDto
    {
        /// <summary>Skill ID (send this in JobRequestDto.SkillIds)</summary>
        public int Id { get; set; }
        /// <summary>Skill name</summary>
        /// <example>C#</example>
        public string Name { get; set; } = string.Empty;
    }
}
```

---

## 4. Service Interface

Create file: **`Services/Recruiter/IJobService.cs`**

```csharp
using RecruitmentPlatformAPI.DTOs.Recruiter;

namespace RecruitmentPlatformAPI.Services.Recruiter
{
    public interface IJobService
    {
        // ── Job management (authenticated Recruiter) ──
        Task<JobResponseDto?> CreateJobAsync(int userId, JobRequestDto dto);
        Task<JobResponseDto?> UpdateJobAsync(int userId, int jobId, JobRequestDto dto);
        Task<bool> DeactivateJobAsync(int userId, int jobId);
        Task<bool> ReactivateJobAsync(int userId, int jobId);
        Task<bool> DeleteJobAsync(int userId, int jobId);
        Task<JobListResponseDto> GetMyJobsAsync(int userId, int page = 1, int pageSize = 10, bool? isActive = null);
        Task<JobResponseDto?> GetJobByIdAsync(int userId, int jobId);

        // ── Candidate matching (authenticated Recruiter) ──
        Task<JobCandidateListResponseDto?> GetCandidatesAsync(int userId, int jobId, int page = 1, int pageSize = 10);

        // ── Reference data (no auth needed) ──
        Task<List<SkillOptionDto>> GetSkillsAsync(string? search = null);
    }
}
```

---

## 5. Service Implementation

Create file: **`Services/Recruiter/JobService.cs`**

```csharp
using Microsoft.EntityFrameworkCore;
using RecruitmentPlatformAPI.Data;
using RecruitmentPlatformAPI.DTOs.Recruiter;
using RecruitmentPlatformAPI.Enums;
using RecruitmentPlatformAPI.Models.Jobs;
using RecruitmentPlatformAPI.Models.Reference;
using JobSeekerEntity = RecruitmentPlatformAPI.Models.JobSeeker.JobSeeker;

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

        // ─────────────────────────────────────
        // JOB MANAGEMENT
        // ─────────────────────────────────────

        public async Task<JobResponseDto?> CreateJobAsync(int userId, JobRequestDto dto)
        {
            try
            {
                // 1. Verify the user is a Recruiter
                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.AccountType != AccountType.Recruiter)
                {
                    _logger.LogWarning("Non-recruiter user {UserId} attempted to create a job", userId);
                    return null;
                }

                // 2. Get Recruiter profile
                var recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);
                if (recruiter == null) return null;

                // 3. Validate skill IDs
                var validSkillIds = new List<int>();
                if (dto.SkillIds != null && dto.SkillIds.Count > 0)
                {
                    validSkillIds = await _context.Skills
                        .Where(s => dto.SkillIds.Contains(s.Id))
                        .Select(s => s.Id)
                        .ToListAsync();

                    if (validSkillIds.Count != dto.SkillIds.Count)
                    {
                        _logger.LogWarning("Invalid skill IDs for job creation by user {UserId}", userId);
                        return null;
                    }
                }

                // 4. Create job entity
                var job = new Job
                {
                    RecruiterId = recruiter.Id,
                    Title = dto.Title.Trim(),
                    Description = dto.Description.Trim(),
                    Requirements = dto.Requirements.Trim(),
                    EmploymentType = dto.EmploymentType,
                    MinYearsOfExperience = dto.MinYearsOfExperience,
                    Location = dto.Location?.Trim(),
                    PostedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();

                // 5. Add required skills
                if (validSkillIds.Count > 0)
                {
                    _context.JobSkills.AddRange(validSkillIds.Select(skillId => new JobSkill
                    {
                        JobId = job.Id,
                        SkillId = skillId
                    }));
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Job {JobId} '{Title}' created by recruiter user {UserId}", job.Id, job.Title, userId);
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

                // Validate skill IDs
                var validSkillIds = new List<int>();
                if (dto.SkillIds != null && dto.SkillIds.Count > 0)
                {
                    validSkillIds = await _context.Skills
                        .Where(s => dto.SkillIds.Contains(s.Id))
                        .Select(s => s.Id)
                        .ToListAsync();

                    if (validSkillIds.Count != dto.SkillIds.Count)
                        return null;
                }

                // Update fields
                job.Title = dto.Title.Trim();
                job.Description = dto.Description.Trim();
                job.Requirements = dto.Requirements.Trim();
                job.EmploymentType = dto.EmploymentType;
                job.MinYearsOfExperience = dto.MinYearsOfExperience;
                job.Location = dto.Location?.Trim();
                job.UpdatedAt = DateTime.UtcNow;

                // Replace skills — remove old, add new
                var existingSkills = await _context.JobSkills.Where(js => js.JobId == jobId).ToListAsync();
                _context.JobSkills.RemoveRange(existingSkills);

                if (validSkillIds.Count > 0)
                {
                    _context.JobSkills.AddRange(validSkillIds.Select(skillId => new JobSkill
                    {
                        JobId = jobId,
                        SkillId = skillId
                    }));
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
            var job = await GetOwnedJobAsync(userId, jobId);
            if (job == null) return false;

            job.IsActive = false;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Job {JobId} deactivated by user {UserId}", jobId, userId);
            return true;
        }

        public async Task<bool> ReactivateJobAsync(int userId, int jobId)
        {
            var job = await GetOwnedJobAsync(userId, jobId);
            if (job == null) return false;

            job.IsActive = true;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Job {JobId} reactivated by user {UserId}", jobId, userId);
            return true;
        }

        public async Task<bool> DeleteJobAsync(int userId, int jobId)
        {
            var job = await GetOwnedJobAsync(userId, jobId);
            if (job == null) return false;

            // Hard delete — cascade removes JobSkills and Recommendations automatically
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Job {JobId} permanently deleted by user {UserId}", jobId, userId);
            return true;
        }

        public async Task<JobListResponseDto> GetMyJobsAsync(int userId, int page = 1, int pageSize = 10, bool? isActive = null)
        {
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

        public async Task<JobResponseDto?> GetJobByIdAsync(int userId, int jobId)
        {
            var job = await GetOwnedJobAsync(userId, jobId);
            if (job == null) return null;
            return await BuildJobResponseDto(job);
        }

        // ─────────────────────────────────────
        // CANDIDATE MATCHING
        // ─────────────────────────────────────

        public async Task<JobCandidateListResponseDto?> GetCandidatesAsync(int userId, int jobId, int page = 1, int pageSize = 10)
        {
            // Verify ownership first
            var job = await GetOwnedJobAsync(userId, jobId);
            if (job == null) return null;

            var query = _context.Recommendations.Where(r => r.JobId == jobId);
            var totalCount = await query.CountAsync();

            var recommendations = await query
                .OrderByDescending(r => r.MatchScore)   // Best matches first
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var candidates = new List<JobCandidateDto>();
            foreach (var rec in recommendations)
            {
                var candidate = await BuildCandidateDto(rec);
                if (candidate != null) candidates.Add(candidate);
            }

            return new JobCandidateListResponseDto
            {
                Candidates = candidates,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        // ─────────────────────────────────────
        // REFERENCE DATA
        // ─────────────────────────────────────

        public async Task<List<SkillOptionDto>> GetSkillsAsync(string? search = null)
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

        // ─────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────

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
        /// Builds a full JobResponseDto including skills and candidate count.
        /// </summary>
        private async Task<JobResponseDto> BuildJobResponseDto(Job job)
        {
            var skills = await _context.JobSkills
                .Where(js => js.JobId == job.Id)
                .Join(_context.Skills, js => js.SkillId, s => s.Id,
                    (js, s) => new JobSkillDto { Id = s.Id, Name = s.Name })
                .ToListAsync();

            var candidateCount = await _context.Recommendations
                .CountAsync(r => r.JobId == job.Id);

            return new JobResponseDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                EmploymentType = job.EmploymentType ?? "FullTime",
                MinYearsOfExperience = job.MinYearsOfExperience,
                Location = job.Location,
                PostedAt = job.PostedAt,
                UpdatedAt = job.UpdatedAt,
                IsActive = job.IsActive,
                CandidateCount = candidateCount,
                Skills = skills
            };
        }

        /// <summary>
        /// Builds a JobCandidateDto from a Recommendation record by loading the
        /// JobSeeker's profile, user row, skills, and location data.
        /// </summary>
        private async Task<JobCandidateDto?> BuildCandidateDto(Recommendation rec)
        {
            var jobSeeker = await _context.JobSeekers.FindAsync(rec.JobSeekerId);
            if (jobSeeker == null) return null;

            var user = await _context.Users.FindAsync(jobSeeker.UserId);
            if (user == null) return null;

            // Candidate's skills (names only)
            var skills = await _context.JobSeekerSkills
                .Where(js => js.JobSeekerId == rec.JobSeekerId)
                .Join(_context.Skills, js => js.SkillId, s => s.Id, (js, s) => s.Name)
                .ToListAsync();

            // Country name
            string? countryName = null;
            if (jobSeeker.CountryId.HasValue)
            {
                var country = await _context.Countries.FindAsync(jobSeeker.CountryId);
                countryName = country?.Name;
            }

            // Job title name
            string? jobTitleName = null;
            if (jobSeeker.JobTitleId.HasValue)
            {
                var jobTitle = await _context.JobTitles.FindAsync(jobSeeker.JobTitleId);
                jobTitleName = jobTitle?.Title;
            }

            return new JobCandidateDto
            {
                JobSeekerId = rec.JobSeekerId,
                MatchScore = rec.MatchScore,
                GeneratedAt = rec.GeneratedAt,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                ProfilePictureUrl = user.ProfilePictureUrl,
                JobTitle = jobTitleName,
                YearsOfExperience = jobSeeker.YearsOfExperience,
                City = jobSeeker.City,
                Country = countryName,
                Bio = jobSeeker.Bio,
                Skills = skills
            };
        }
    }
}
```

> **Note on `BuildCandidateDto`:** This method makes several individual DB calls (N+1 pattern). For a paginated list of ≤10–50 candidates per page this is acceptable. If you need to optimize later, switch to a single query using `Include().ThenInclude()`.

---

## 6. Controller

Create file: **`Controllers/JobsController.cs`**

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RecruitmentPlatformAPI.DTOs.Common;
using RecruitmentPlatformAPI.DTOs.Recruiter;
using RecruitmentPlatformAPI.Services.Recruiter;

namespace RecruitmentPlatformAPI.Controllers
{
    /// <summary>
    /// Job postings and AI candidate matching — Recruiter only
    /// </summary>
    [ApiController]
    [Route("api/jobs")]
    [Produces("application/json")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IJobService jobService, ILogger<JobsController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        // ══════════════════════════════════════════
        // REFERENCE DATA  (no auth required)
        // ══════════════════════════════════════════

        /// <summary>
        /// Get available skills list for the job creation form
        /// </summary>
        /// <param name="search">Optional: filter by skill name (partial match)</param>
        [HttpGet("skills")]
        [ProducesResponseType(typeof(ApiResponse<List<SkillOptionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSkills([FromQuery] string? search = null)
        {
            var result = await _jobService.GetSkillsAsync(search);
            return Ok(new ApiResponse<List<SkillOptionDto>>(result));
        }

        // ══════════════════════════════════════════
        // JOB MANAGEMENT  (Recruiter only)
        // ══════════════════════════════════════════

        /// <summary>
        /// Get all job postings created by the authenticated recruiter
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
        /// <param name="isActive">Filter: true = active only, false = inactive only, omit = all</param>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<JobListResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyJobs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isActive = null)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new ApiErrorResponse("User not authenticated"));

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var result = await _jobService.GetMyJobsAsync(userId, page, pageSize, isActive);
            return Ok(new ApiResponse<JobListResponseDto>(result));
        }

        /// <summary>
        /// Get a specific job posting (must be owned by the recruiter)
        /// </summary>
        /// <param name="id">Job ID</param>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<JobResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetJob(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new ApiErrorResponse("User not authenticated"));

            var result = await _jobService.GetJobByIdAsync(userId, id);
            if (result == null)
                return NotFound(new ApiErrorResponse("Job not found or you don't have permission to view it"));

            return Ok(new ApiResponse<JobResponseDto>(result));
        }

        /// <summary>
        /// Create a new job posting (Recruiter only)
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<JobResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateJob([FromBody] JobRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new ApiErrorResponse("User not authenticated"));

            var result = await _jobService.CreateJobAsync(userId, dto);
            if (result == null)
                return BadRequest(new ApiErrorResponse(
                    "Failed to create job. Ensure you have a Recruiter account and all skill IDs are valid."));

            return CreatedAtAction(nameof(GetJob), new { id = result.Id },
                new ApiResponse<JobResponseDto>(result));
        }

        /// <summary>
        /// Update an existing job posting (own jobs only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<JobResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new ApiErrorResponse("User not authenticated"));

            var result = await _jobService.UpdateJobAsync(userId, id, dto);
            if (result == null)
                return NotFound(new ApiErrorResponse("Job not found or you don't have permission to edit it"));

            return Ok(new ApiResponse<JobResponseDto>(result));
        }

        /// <summary>
        /// Deactivate a job — hides it from candidate matching (own jobs only)
        /// </summary>
        [HttpPatch("{id}/deactivate")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeactivateJob(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new ApiErrorResponse("User not authenticated"));

            var result = await _jobService.DeactivateJobAsync(userId, id);
            if (!result)
                return NotFound(new ApiErrorResponse("Job not found or you don't have permission"));

            return Ok(new ApiResponse<string>("Job deactivated successfully"));
        }

        /// <summary>
        /// Reactivate a previously deactivated job (own jobs only)
        /// </summary>
        [HttpPatch("{id}/reactivate")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ReactivateJob(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new ApiErrorResponse("User not authenticated"));

            var result = await _jobService.ReactivateJobAsync(userId, id);
            if (!result)
                return NotFound(new ApiErrorResponse("Job not found or you don't have permission"));

            return Ok(new ApiResponse<string>("Job reactivated successfully"));
        }

        /// <summary>
        /// Permanently delete a job posting (own jobs only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new ApiErrorResponse("User not authenticated"));

            var result = await _jobService.DeleteJobAsync(userId, id);
            if (!result)
                return NotFound(new ApiErrorResponse("Job not found or you don't have permission"));

            return Ok(new ApiResponse<string>("Job deleted successfully"));
        }

        // ══════════════════════════════════════════
        // CANDIDATE MATCHING  (Recruiter only)
        // ══════════════════════════════════════════

        /// <summary>
        /// Get the AI-matched candidate list for a job, ranked by MatchScore descending.
        /// Only the owning Recruiter can call this. JobSeekers are unaware they appear here.
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Candidates per page (default: 10, max: 50)</param>
        [HttpGet("{id}/candidates")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<JobCandidateListResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCandidates(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new ApiErrorResponse("User not authenticated"));

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var result = await _jobService.GetCandidatesAsync(userId, id, page, pageSize);
            if (result == null)
                return NotFound(new ApiErrorResponse("Job not found or you don't have permission to view candidates"));

            return Ok(new ApiResponse<JobCandidateListResponseDto>(result));
        }

        // ─── Helper ───
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
    }
}
```

---

## 7. Registration in Program.cs

Add **one line** alongside the existing service registrations:

```csharp
// In Program.cs — alongside existing builder.Services.AddScoped lines:
builder.Services.AddScoped<IJobService, JobService>();
```

The `using RecruitmentPlatformAPI.Services.Recruiter;` directive is already in `Program.cs` — no extra `using` needed.

---

## 8. Database Migration

**No migration needed.**

The `Jobs`, `JobSkills`, `Skills`, and `Recommendations` tables already exist in the database. Moving the C# model files from `Models/Reference/` → `Models/Jobs/` was a code organization change only — the database schema is unchanged.

---

## 9. API Endpoints Summary

> **There are no public or JobSeeker-facing job endpoints.**  
> All endpoints require a **Recruiter JWT token**, except `GET /api/jobs/skills`.

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `GET` | `/api/jobs/skills` | None | Skills dropdown (for job creation form) |
| `GET` | `/api/jobs` | ✅ Recruiter | List own job postings (paginated, filterable by isActive) |
| `GET` | `/api/jobs/{id}` | ✅ Recruiter | Get one of own jobs |
| `POST` | `/api/jobs` | ✅ Recruiter | Create a new job posting |
| `PUT` | `/api/jobs/{id}` | ✅ Recruiter | Update own job |
| `PATCH` | `/api/jobs/{id}/deactivate` | ✅ Recruiter | Soft-hide job (stops AI matching) |
| `PATCH` | `/api/jobs/{id}/reactivate` | ✅ Recruiter | Re-enable a deactivated job |
| `DELETE` | `/api/jobs/{id}` | ✅ Recruiter | Permanently delete job |
| `GET` | `/api/jobs/{id}/candidates` | ✅ Recruiter | View AI-matched candidates, ranked by MatchScore |

---

## 10. Validation Rules

| Field | Rule |
|-------|------|
| Title | Required, 3–150 chars |
| Description | Required, 20–1200 chars |
| Requirements | Required, 20–1200 chars |
| EmploymentType | Required; must be one of: `FullTime`, `PartTime`, `Freelance`, `Internship` |
| MinYearsOfExperience | Required, range 0–30 |
| Location | Optional, max 100 chars |
| SkillIds | Optional, max 15 items; each ID must exist in the `Skills` table |
| Account type | Only `AccountType.Recruiter` accounts can use these endpoints |
| Ownership | A recruiter can only manage and view candidates for **their own** jobs |

---

## 11. Testing Checklist

Test via Swagger UI at `http://localhost:5217/swagger`.  
For all recruiter endpoints: register a Recruiter account → log in → paste the JWT into Swagger **Authorize**.

### Reference data (no auth)
- [ ] `GET /api/jobs/skills` — returns full skill list
- [ ] `GET /api/jobs/skills?search=C` — returns filtered skills (e.g. C#, CSS)

### Job CRUD (Recruiter token required)
- [ ] `POST /api/jobs` — valid payload → `201 Created` with job in body
- [ ] `POST /api/jobs` — with skill IDs → skills appear in response
- [ ] `POST /api/jobs` — invalid skill ID (e.g. 9999) → `400`
- [ ] `POST /api/jobs` — missing required fields → `400` with validation errors
- [ ] `POST /api/jobs` — no token → `401 Unauthorized`
- [ ] `POST /api/jobs` — JobSeeker token → `400` (not a Recruiter)
- [ ] `GET /api/jobs` — returns own jobs, ordered newest first
- [ ] `GET /api/jobs?isActive=false` — returns only deactivated jobs
- [ ] `GET /api/jobs/{id}` — returns job with skills list and candidateCount
- [ ] `GET /api/jobs/{id}` — another recruiter's job ID → `404`
- [ ] `PUT /api/jobs/{id}` — update own job — `200` with updated data
- [ ] `PUT /api/jobs/{id}` — another recruiter's job ID → `404`
- [ ] `PATCH /api/jobs/{id}/deactivate` → `200`, `IsActive` becomes `false`
- [ ] `PATCH /api/jobs/{id}/reactivate` → `200`, `IsActive` becomes `true`
- [ ] `DELETE /api/jobs/{id}` → `200`, job no longer in `GET /api/jobs`

### Candidates (Recruiter token required)
- [ ] `GET /api/jobs/{id}/candidates` — returns empty list when no AI data exists yet
- [ ] `GET /api/jobs/{id}/candidates` — another recruiter's job → `404`
- [ ] `GET /api/jobs/{id}/candidates?page=1&pageSize=5` — pagination params accepted

---

## 12. Code Conventions Cheat-Sheet

Follow these patterns exactly — they are used consistently throughout the project:

| Convention | Example / Detail |
|-----------|------------------|
| **Response wrapper** | `ApiResponse<T>` for success; `ApiErrorResponse` for errors |
| **Logging** | `_logger.LogInformation/Warning/Error` with named params `{UserId}`, `{JobId}` |
| **Error handling** | Wrap all service methods in `try/catch`; log exceptions; return `null` or `false` |
| **Input trimming** | Always call `.Trim()` on strings; use `?.Trim()` for optional fields |
| **DateTime** | Always `DateTime.UtcNow` — never `DateTime.Now` |
| **Namespace aliases** | Needed when a folder name matches a class name — e.g. `using JobSeekerEntity = ...JobSeeker.JobSeeker;` |
| **Auth helper** | `GetCurrentUserId()` in controller; return `Unauthorized()` if it returns `0` |
| **Account type guard** | Check `user.AccountType != AccountType.Recruiter` at the top of service methods |
| **Ownership guard** | Always verify the resource belongs to the requesting user before any mutation |
| **Enums as strings** | Already configured globally in `Program.cs` — no extra setup needed |
| **XML doc comments** | Add `/// <summary>` to all public DTOs, interfaces, and controller actions |
| **Pagination in controller** | Clamp: `Math.Max(1, page)` and `Math.Clamp(pageSize, 1, 50)` |

### File locations (summary)

| File | Path |
|------|------|
| DTOs | `DTOs/Recruiter/JobDtos.cs` |
| Interface | `Services/Recruiter/IJobService.cs` |
| Service | `Services/Recruiter/JobService.cs` |
| Controller | `Controllers/JobsController.cs` |
| Job model | `Models/Jobs/Job.cs` |
| JobSkill model | `Models/Jobs/JobSkill.cs` |
| Recommendation model | `Models/Jobs/Recommendation.cs` |
| Skill model (reference data) | `Models/Reference/Skill.cs` |

---

## Quick Start

```bash
# 1. Create the 4 new files listed above
# 2. Add one line to Program.cs:
#    builder.Services.AddScoped<IJobService, JobService>();
# 3. Build
dotnet build
# 4. Run
dotnet run --project RecruitmentPlatformAPI
# 5. Open Swagger
# http://localhost:5217/swagger
```

No database migrations are needed.
