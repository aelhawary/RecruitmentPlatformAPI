# Jobs Module — Quick Implementation Guide

> **Goal:** Implement job posting CRUD for recruiters

---

## Business Rules

- ✅ Only **Recruiter** accounts can create/edit/delete jobs
- ✅ Recruiters can only manage **their own** jobs
- ✅ Jobs can be deactivated (soft delete) or permanently deleted
- ✅ Each job can have 0-15 skills from the Skills reference table
- ❌ No AI matching or candidate features (deferred to future module)

---

## Files to Create

### 1. DTOs: `DTOs/Recruiter/JobDtos.cs`

**Purpose:** Define data contracts for job-related API requests and responses.

**What this file contains:**
- `JobRequestDto` - Validates incoming data when creating/updating jobs (title, description, requirements, employment type, experience, location, skill IDs)
- `JobResponseDto` - Shapes the data returned for a single job (includes all job fields + skills list + candidate count placeholder)
- `JobListResponseDto` - Wraps paginated job listings (total count, page info, jobs array)
- `JobSkillDto` - Represents a single skill attached to a job (ID + name)
- `SkillOptionDto` - Simple format for skills dropdown (ID + name)

**Key validations:** String lengths, required fields, range checks, max 15 skills per job.

```csharp
using System.ComponentModel.DataAnnotations;

namespace RecruitmentPlatformAPI.DTOs.Recruiter
{
    // ─────────────────────────────────────────────
    // REQUEST DTOs
    // ─────────────────────────────────────────────

    public class JobRequestDto
    {
        [Required(ErrorMessage = "Job title is required")]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job description is required")]
        [StringLength(1200, MinimumLength = 20)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Requirements are required")]
        [StringLength(1200, MinimumLength = 20)]
        public string Requirements { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employment type is required")]
        public string EmploymentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Minimum years of experience is required")]
        [Range(0, 30)]
        public int MinYearsOfExperience { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [MaxLength(15, ErrorMessage = "Maximum 15 skills allowed")]
        public List<int> SkillIds { get; set; } = new();
    }

    public class JobSkillDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
    }

    // ─────────────────────────────────────────────
    // RESPONSE DTOs
    // ─────────────────────────────────────────────

    public class JobResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public int MinYearsOfExperience { get; set; }
        public string? Location { get; set; }
        public DateTime PostedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int CandidateCount { get; set; }
        public List<JobSkillDto> Skills { get; set; } = new();
    }

    public class JobListResponseDto
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<JobResponseDto> Jobs { get; set; } = new();
    }

    public class SkillOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
```

---

### 2. Service Interface: `Services/Recruiter/IJobService.cs`

**Purpose:** Define the contract for all job-related business operations.

**What this file contains:**
- **Job CRUD methods** - Create, read (list + single), update, delete jobs
- **Status management** - Deactivate and reactivate jobs
- **Reference data** - Get skills for dropdowns with optional search

**Why it exists:** Separates the "what" (interface) from the "how" (implementation), enabling dependency injection and testability.

```csharp
using RecruitmentPlatformAPI.DTOs.Common;
using RecruitmentPlatformAPI.DTOs.Recruiter;

namespace RecruitmentPlatformAPI.Services.Recruiter
{
    public interface IJobService
    {
        // Job CRUD
        Task<ApiResponse<JobResponseDto>> CreateJobAsync(int recruiterId, JobRequestDto dto);
        Task<ApiResponse<JobListResponseDto>> GetJobsAsync(int recruiterId, int page = 1, int pageSize = 10, bool? isActive = null);
        Task<ApiResponse<JobResponseDto>> GetJobByIdAsync(int jobId, int recruiterId);
        Task<ApiResponse<JobResponseDto>> UpdateJobAsync(int jobId, int recruiterId, JobRequestDto dto);
        Task<ApiResponse<object>> DeleteJobAsync(int jobId, int recruiterId);

        // Status management
        Task<ApiResponse<object>> DeactivateJobAsync(int jobId, int recruiterId);
        Task<ApiResponse<object>> ReactivateJobAsync(int jobId, int recruiterId);

        // Reference data
        Task<ApiResponse<List<SkillOptionDto>>> GetSkillsAsync(string? search = null);
    }
}
```

---

### 3. Service Implementation: `Services/Recruiter/JobService.cs`

**Purpose:** Implement all business logic for job management with database interactions.

**What this file does:**
- **Validates business rules** - Employment type enum validation, skill ID existence checks, ownership verification
- **CRUD operations** - Creates jobs, lists with pagination/filtering, updates with skill re-assignment, hard deletes
- **Soft delete** - Deactivate/reactivate jobs by toggling `IsActive` flag
- **Data transformation** - Uses `BuildJobResponseDto` helper to fetch job + skills in a consistent format
- **Security** - `IsJobOwner` helper ensures recruiters can only access their own jobs

**Key patterns:** Always trim user input, use UTC timestamps, validate before saving, return `ApiResponse<T>` wrapper.

```csharp
using Microsoft.EntityFrameworkCore;
using RecruitmentPlatformAPI.Data;
using RecruitmentPlatformAPI.DTOs.Common;
using RecruitmentPlatformAPI.DTOs.Recruiter;
using RecruitmentPlatformAPI.Models.Jobs;

namespace RecruitmentPlatformAPI.Services.Recruiter
{
    public class JobService : IJobService
    {
        private readonly AppDbContext _context;

        public JobService(AppDbContext context)
        {
            _context = context;
        }

        // ═══════════════════════════════════════════════════════════════════
        // JOB CRUD
        // ═══════════════════════════════════════════════════════════════════

        public async Task<ApiResponse<JobResponseDto>> CreateJobAsync(int recruiterId, JobRequestDto dto)
        {
            // Validate employment type
            var validTypes = new[] { "FullTime", "PartTime", "Freelance", "Internship" };
            if (!validTypes.Contains(dto.EmploymentType))
            {
                return ApiResponse<JobResponseDto>.FailureResponse(
                    "Invalid employment type. Valid values: FullTime, PartTime, Freelance, Internship"
                );
            }

            // Validate skills exist
            if (dto.SkillIds.Any())
            {
                var existingSkillIds = await _context.Skills
                    .Where(s => dto.SkillIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync();

                var invalidIds = dto.SkillIds.Except(existingSkillIds).ToList();
                if (invalidIds.Any())
                {
                    return ApiResponse<JobResponseDto>.FailureResponse(
                        $"Invalid skill IDs: {string.Join(", ", invalidIds)}"
                    );
                }
            }

            var job = new Job
            {
                RecruiterId = recruiterId,
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

            // Attach skills
            if (dto.SkillIds.Any())
            {
                var jobSkills = dto.SkillIds.Select(skillId => new JobSkill
                {
                    JobId = job.Id,
                    SkillId = skillId
                }).ToList();

                _context.JobSkills.AddRange(jobSkills);
                await _context.SaveChangesAsync();
            }

            var response = await BuildJobResponseDto(job.Id);
            return ApiResponse<JobResponseDto>.SuccessResponse(response, "Job created successfully");
        }

        public async Task<ApiResponse<JobListResponseDto>> GetJobsAsync(int recruiterId, int page = 1, int pageSize = 10, bool? isActive = null)
        {
            var query = _context.Jobs.Where(j => j.RecruiterId == recruiterId);

            if (isActive.HasValue)
            {
                query = query.Where(j => j.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var jobs = await query
                .OrderByDescending(j => j.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(j => j.Id)
                .ToListAsync();

            var jobDtos = new List<JobResponseDto>();
            foreach (var jobId in jobs)
            {
                jobDtos.Add(await BuildJobResponseDto(jobId));
            }

            var result = new JobListResponseDto
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Jobs = jobDtos
            };

            return ApiResponse<JobListResponseDto>.SuccessResponse(result);
        }

        public async Task<ApiResponse<JobResponseDto>> GetJobByIdAsync(int jobId, int recruiterId)
        {
            if (!await IsJobOwner(jobId, recruiterId))
            {
                return ApiResponse<JobResponseDto>.FailureResponse("Job not found or access denied");
            }

            var job = await BuildJobResponseDto(jobId);
            return ApiResponse<JobResponseDto>.SuccessResponse(job);
        }

        public async Task<ApiResponse<JobResponseDto>> UpdateJobAsync(int jobId, int recruiterId, JobRequestDto dto)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null || job.RecruiterId != recruiterId)
            {
                return ApiResponse<JobResponseDto>.FailureResponse("Job not found or access denied");
            }

            // Validate employment type
            var validTypes = new[] { "FullTime", "PartTime", "Freelance", "Internship" };
            if (!validTypes.Contains(dto.EmploymentType))
            {
                return ApiResponse<JobResponseDto>.FailureResponse(
                    "Invalid employment type. Valid values: FullTime, PartTime, Freelance, Internship"
                );
            }

            // Validate skills
            if (dto.SkillIds.Any())
            {
                var existingSkillIds = await _context.Skills
                    .Where(s => dto.SkillIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync();

                var invalidIds = dto.SkillIds.Except(existingSkillIds).ToList();
                if (invalidIds.Any())
                {
                    return ApiResponse<JobResponseDto>.FailureResponse(
                        $"Invalid skill IDs: {string.Join(", ", invalidIds)}"
                    );
                }
            }

            // Update job
            job.Title = dto.Title.Trim();
            job.Description = dto.Description.Trim();
            job.Requirements = dto.Requirements.Trim();
            job.EmploymentType = dto.EmploymentType;
            job.MinYearsOfExperience = dto.MinYearsOfExperience;
            job.Location = dto.Location?.Trim();
            job.UpdatedAt = DateTime.UtcNow;

            // Update skills
            var existingSkills = await _context.JobSkills.Where(js => js.JobId == jobId).ToListAsync();
            _context.JobSkills.RemoveRange(existingSkills);

            if (dto.SkillIds.Any())
            {
                var newSkills = dto.SkillIds.Select(skillId => new JobSkill
                {
                    JobId = jobId,
                    SkillId = skillId
                }).ToList();

                _context.JobSkills.AddRange(newSkills);
            }

            await _context.SaveChangesAsync();

            var response = await BuildJobResponseDto(jobId);
            return ApiResponse<JobResponseDto>.SuccessResponse(response, "Job updated successfully");
        }

        public async Task<ApiResponse<object>> DeleteJobAsync(int jobId, int recruiterId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null || job.RecruiterId != recruiterId)
            {
                return ApiResponse<object>.FailureResponse("Job not found or access denied");
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return ApiResponse<object>.SuccessResponse(null, "Job deleted successfully");
        }

        // ═══════════════════════════════════════════════════════════════════
        // STATUS MANAGEMENT
        // ═══════════════════════════════════════════════════════════════════

        public async Task<ApiResponse<object>> DeactivateJobAsync(int jobId, int recruiterId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null || job.RecruiterId != recruiterId)
            {
                return ApiResponse<object>.FailureResponse("Job not found or access denied");
            }

            if (!job.IsActive)
            {
                return ApiResponse<object>.FailureResponse("Job is already deactivated");
            }

            job.IsActive = false;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ApiResponse<object>.SuccessResponse(null, "Job deactivated successfully");
        }

        public async Task<ApiResponse<object>> ReactivateJobAsync(int jobId, int recruiterId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null || job.RecruiterId != recruiterId)
            {
                return ApiResponse<object>.FailureResponse("Job not found or access denied");
            }

            if (job.IsActive)
            {
                return ApiResponse<object>.FailureResponse("Job is already active");
            }

            job.IsActive = true;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ApiResponse<object>.SuccessResponse(null, "Job reactivated successfully");
        }

        // ═══════════════════════════════════════════════════════════════════
        // REFERENCE DATA
        // ═══════════════════════════════════════════════════════════════════

        public async Task<ApiResponse<List<SkillOptionDto>>> GetSkillsAsync(string? search = null)
        {
            var query = _context.Skills.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.Name.Contains(search));
            }

            var skills = await query
                .OrderBy(s => s.Name)
                .Select(s => new SkillOptionDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();

            return ApiResponse<List<SkillOptionDto>>.SuccessResponse(skills);
        }

        // ═══════════════════════════════════════════════════════════════════
        // HELPERS
        // ═══════════════════════════════════════════════════════════════════

        private async Task<bool> IsJobOwner(int jobId, int recruiterId)
        {
            return await _context.Jobs.AnyAsync(j => j.Id == jobId && j.RecruiterId == recruiterId);
        }

        private async Task<JobResponseDto> BuildJobResponseDto(int jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null) throw new InvalidOperationException($"Job {jobId} not found");

            var skills = await _context.JobSkills
                .Where(js => js.JobId == jobId)
                .Include(js => js.Skill)
                .Select(js => new JobSkillDto
                {
                    SkillId = js.SkillId,
                    SkillName = js.Skill.Name
                })
                .ToListAsync();

            return new JobResponseDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                EmploymentType = job.EmploymentType ?? string.Empty,
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
```

---

### 4. Controller: `Controllers/JobsController.cs`

**Purpose:** Expose HTTP endpoints for job operations, handle authorization, and route requests to the service layer.

**What this file does:**
- **Route mapping** - Maps HTTP verbs (GET/POST/PUT/PATCH/DELETE) to service methods
- **Authentication** - Uses `[Authorize]` attribute for recruiter-only endpoints (except skills dropdown)
- **Authorization** - `GetRecruiterIdOrFail()` helper extracts user ID from JWT and verifies account type
- **HTTP responses** - Returns appropriate status codes (200 OK, 201 Created, 400 Bad Request, 401 Unauthorized)
- **Public endpoint** - `/api/jobs/skills` is `[AllowAnonymous]` for frontend dropdown population

**Endpoint structure:** 1 public reference endpoint + 7 recruiter-only job management endpoints.

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPlatformAPI.DTOs.Recruiter;
using RecruitmentPlatformAPI.Enums;
using RecruitmentPlatformAPI.Services.Recruiter;
using System.Security.Claims;

namespace RecruitmentPlatformAPI.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        // ─────────────────────────────────────────────────────────────────
        // REFERENCE DATA (no auth)
        // ─────────────────────────────────────────────────────────────────

        /// <summary>Get all skills (for job creation form dropdown)</summary>
        /// <param name="search">Optional search filter</param>
        [HttpGet("skills")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSkills([FromQuery] string? search = null)
        {
            var response = await _jobService.GetSkillsAsync(search);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // ─────────────────────────────────────────────────────────────────
        // JOB CRUD (Recruiter only)
        // ─────────────────────────────────────────────────────────────────

        /// <summary>List own job postings</summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetJobs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isActive = null)
        {
            var recruiterId = GetRecruiterIdOrFail();
            if (recruiterId == null) return BadRequest(new { Success = false, Message = "Only recruiters can access this endpoint" });

            var response = await _jobService.GetJobsAsync(recruiterId.Value, page, pageSize, isActive);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>Get single job by ID</summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetJobById(int id)
        {
            var recruiterId = GetRecruiterIdOrFail();
            if (recruiterId == null) return BadRequest(new { Success = false, Message = "Only recruiters can access this endpoint" });

            var response = await _jobService.GetJobByIdAsync(id, recruiterId.Value);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>Create a new job posting</summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateJob([FromBody] JobRequestDto dto)
        {
            var recruiterId = GetRecruiterIdOrFail();
            if (recruiterId == null) return BadRequest(new { Success = false, Message = "Only recruiters can access this endpoint" });

            var response = await _jobService.CreateJobAsync(recruiterId.Value, dto);
            return response.Success ? CreatedAtAction(nameof(GetJobById), new { id = response.Data?.Id }, response) : BadRequest(response);
        }

        /// <summary>Update an existing job</summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobRequestDto dto)
        {
            var recruiterId = GetRecruiterIdOrFail();
            if (recruiterId == null) return BadRequest(new { Success = false, Message = "Only recruiters can access this endpoint" });

            var response = await _jobService.UpdateJobAsync(id, recruiterId.Value, dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>Deactivate a job (soft delete)</summary>
        [HttpPatch("{id}/deactivate")]
        [Authorize]
        public async Task<IActionResult> DeactivateJob(int id)
        {
            var recruiterId = GetRecruiterIdOrFail();
            if (recruiterId == null) return BadRequest(new { Success = false, Message = "Only recruiters can access this endpoint" });

            var response = await _jobService.DeactivateJobAsync(id, recruiterId.Value);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>Reactivate a deactivated job</summary>
        [HttpPatch("{id}/reactivate")]
        [Authorize]
        public async Task<IActionResult> ReactivateJob(int id)
        {
            var recruiterId = GetRecruiterIdOrFail();
            if (recruiterId == null) return BadRequest(new { Success = false, Message = "Only recruiters can access this endpoint" });

            var response = await _jobService.ReactivateJobAsync(id, recruiterId.Value);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>Permanently delete a job</summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var recruiterId = GetRecruiterIdOrFail();
            if (recruiterId == null) return BadRequest(new { Success = false, Message = "Only recruiters can access this endpoint" });

            var response = await _jobService.DeleteJobAsync(id, recruiterId.Value);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // ─────────────────────────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────────────────────────

        private int? GetRecruiterIdOrFail()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var accountTypeClaim = User.FindFirst("AccountType")?.Value;

            if (userIdClaim == null || accountTypeClaim != AccountType.Recruiter.ToString())
                return null;

            return int.Parse(userIdClaim);
        }
    }
}
```

---

### 5. Register Service in `Program.cs`

Add this line in the services section:

```csharp
builder.Services.AddScoped<IJobService, JobService>();
```

---

## Validation Rules

| Field | Validation |
|-------|-----------|
| Title | Required, 3-150 chars |
| Description | Required, 20-1200 chars |
| Requirements | Required, 20-1200 chars |
| EmploymentType | Required, must be: `FullTime`, `PartTime`, `Freelance`, or `Internship` |
| MinYearsOfExperience | Required, 0-30 |
| Location | Optional, max 100 chars |
| SkillIds | Optional, max 15, all IDs must exist in Skills table |

---

## API Endpoints (8 total)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/jobs/skills` | None | Get skills dropdown (with optional search) |
| `GET` | `/api/jobs` | Recruiter | List own jobs (paginated, filterable) |
| `GET` | `/api/jobs/{id}` | Recruiter | Get job details |
| `POST` | `/api/jobs` | Recruiter | Create job |
| `PUT` | `/api/jobs/{id}` | Recruiter | Update job |
| `PATCH` | `/api/jobs/{id}/deactivate` | Recruiter | Deactivate job |
| `PATCH` | `/api/jobs/{id}/reactivate` | Recruiter | Reactivate job |
| `DELETE` | `/api/jobs/{id}` | Recruiter | Delete job |

---

## Testing Checklist

Test at `http://localhost:5217/swagger`

**Prerequisites:**
1. Register a Recruiter account
2. Login and copy the JWT token
3. Click "Authorize" in Swagger and paste token

**Tests:**
- [ ] `GET /api/jobs/skills` - Returns all skills (no auth needed)
- [ ] `GET /api/jobs/skills?search=java` - Returns filtered skills
- [ ] `POST /api/jobs` - Create job with valid data → 201 Created
- [ ] `POST /api/jobs` - With skill IDs → skills appear in response
- [ ] `POST /api/jobs` - Invalid skill ID → 400 error
- [ ] `POST /api/jobs` - Missing required fields → 400 with validation errors
- [ ] `POST /api/jobs` - JobSeeker token → 400 (not a recruiter)
- [ ] `GET /api/jobs` - Returns own jobs only
- [ ] `GET /api/jobs?isActive=true` - Returns active jobs only
- [ ] `GET /api/jobs/{id}` - Returns job details
- [ ] `PUT /api/jobs/{id}` - Update own job → 200 OK
- [ ] `PUT /api/jobs/{id}` - Try to update another recruiter's job → 400
- [ ] `PATCH /api/jobs/{id}/deactivate` - Deactivates job
- [ ] `PATCH /api/jobs/{id}/reactivate` - Reactivates job
- [ ] `DELETE /api/jobs/{id}` - Permanently deletes job

---

## Code Conventions

Follow existing patterns in the codebase:

- **Namespaces**: Match folder structure (`DTOs.Recruiter`, `Services.Recruiter`, `Models.Jobs`)
- **Response format**: Use `ApiResponse<T>` wrapper (already exists in `DTOs/Common/`)
- **DateTime**: Always use `DateTime.UtcNow`
- **String handling**: Always `.Trim()` user input
- **Authorization**: Check `AccountType.Recruiter` claim
- **Ownership**: Always verify `job.RecruiterId == recruiterId`
- **Error messages**: Clear and user-friendly

---

## No Migration Needed

The database tables (`Jobs`, `JobSkills`, `Skills`) already exist. This is **pure code implementation** - no schema changes required.

---

**Questions?** Check the full guide at `JOBS_MODULE_IMPLEMENTATION_GUIDE.md` for detailed explanations.
