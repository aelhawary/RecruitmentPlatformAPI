# JobIntel - Detailed Technical Guide

## Table of Contents
1. [Project Overview](#1-project-overview)
2. [Architecture & Patterns](#2-architecture--patterns)
3. [Authentication System](#3-authentication-system)
4. [Job Seeker Module](#4-job-seeker-module)
5. [Recruiter Module](#5-recruiter-module)
6. [Database Design](#6-database-design)
7. [API Response Standards](#7-api-response-standards)
8. [Validation Strategy](#8-validation-strategy)
9. [File Upload Handling](#9-file-upload-handling)
10. [Implementation Decisions & Why](#10-implementation-decisions--why)

---

## 1. Project Overview

### What We're Building
A recruitment platform backend API that connects **Job Seekers** with **Recruiters**. Think of it like LinkedIn's backend.

### Tech Stack
| Component | Technology | Why We Chose It |
|-----------|------------|-----------------|
| Framework | ASP.NET Core 9.0 | Latest .NET, great performance, built-in DI |
| ORM | Entity Framework Core | Easy database operations, migrations |
| Database | SQL Server (LocalDB) | Free for development, scales well |
| Auth | JWT Bearer Tokens | Stateless, works with any frontend |
| Docs | Swagger/OpenAPI | Auto-generated API documentation |

### Project Structure Explained

```
RecruitmentPlatformAPI/
│
├── Controllers/                 # HTTP entry points
│   ├── AuthController.cs        # Login, register, password reset
│   ├── JobSeekerController.cs   # Job seeker personal info, wizard
│   ├── RecruiterController.cs   # Recruiter company info
│   ├── ExperienceController.cs  # Work experience CRUD
│   ├── EducationController.cs   # Education CRUD
│   ├── ProjectsController.cs    # Portfolio projects CRUD
│   ├── SocialAccountsController.cs
│   ├── ResumeController.cs      # CV upload/download
│   └── LocationsController.cs   # Countries, languages
│
├── Services/                    # Business logic layer
│   ├── Auth/
│   │   ├── IAuthService.cs      # Interface (contract)
│   │   ├── AuthService.cs       # Implementation
│   │   ├── ITokenService.cs
│   │   ├── TokenService.cs      # JWT generation
│   │   ├── IEmailService.cs
│   │   └── EmailService.cs      # OTP emails
│   │
│   ├── JobSeeker/
│   │   ├── IJobSeekerService.cs
│   │   ├── JobSeekerService.cs
│   │   ├── IExperienceService.cs
│   │   ├── ExperienceService.cs
│   │   ├── IEducationService.cs
│   │   ├── EducationService.cs
│   │   ├── IProjectService.cs
│   │   ├── ProjectService.cs
│   │   ├── IResumeService.cs
│   │   ├── ResumeService.cs
│   │   ├── IProfilePictureService.cs
│   │   ├── ProfilePictureService.cs
│   │   ├── ISocialAccountService.cs
│   │   └── SocialAccountService.cs
│   │
│   └── Recruiter/
│       ├── IRecruiterService.cs
│       └── RecruiterService.cs
│
├── Models/                      # Database entities (tables)
│   ├── Identity/
│   │   └── User.cs              # Base user (all account types)
│   ├── JobSeeker/
│   │   ├── JobSeeker.cs         # Extended job seeker info
│   │   ├── Experience.cs
│   │   ├── Education.cs
│   │   ├── Project.cs
│   │   ├── SocialAccount.cs
│   │   └── Resume.cs
│   ├── Recruiter/
│   │   └── Recruiter.cs         # Company info (1:1 with User)
│   └── Reference/
│       ├── JobTitle.cs          # 90 predefined titles
│       ├── Country.cs           # All countries
│       └── Language.cs          # All languages
│
├── DTOs/                        # Data Transfer Objects
│   ├── Auth/
│   │   ├── RegisterDto.cs
│   │   ├── LoginDto.cs
│   │   ├── AuthResponseDto.cs
│   │   └── ...
│   ├── JobSeeker/
│   │   ├── PersonalInfoRequestDto.cs
│   │   ├── PersonalInfoDto.cs
│   │   ├── ExperienceDtos.cs
│   │   └── ...
│   ├── Recruiter/
│   │   └── RecruiterProfileDtos.cs
│   └── Common/
│       ├── ApiResponse.cs       # Standard success wrapper
│       └── ApiErrorResponse.cs  # Standard error wrapper
│
├── Enums/
│   ├── AccountType.cs           # JobSeeker, Recruiter
│   ├── EmploymentType.cs        # FullTime, PartTime, Contract...
│   ├── ExperienceSeniorityLevel.cs
│   └── ...
│
├── Data/
│   ├── AppDbContext.cs          # EF Core database context
│   ├── Migrations/              # Database version history
│   └── Seed/                    # Initial data (countries, etc.)
│
├── Configuration/
│   ├── JwtSettings.cs
│   ├── EmailSettings.cs
│   └── FileStorageSettings.cs
│
├── Uploads/                     # File storage
│   ├── ProfilePictures/
│   └── Resumes/
│
├── Program.cs                   # App configuration & startup
├── appsettings.json             # Production settings
└── appsettings.Development.json # Dev settings (secrets here)
```

---

## 2. Architecture & Patterns

### Layered Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                      CLIENT (React)                         │
└─────────────────────────────────────────────────────────────┘
                              │ HTTP
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    CONTROLLER LAYER                         │
│  • Receives HTTP requests                                   │
│  • Validates model state                                    │
│  • Extracts user ID from JWT                               │
│  • Returns HTTP responses                                   │
└─────────────────────────────────────────────────────────────┘
                              │ Method calls
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                     SERVICE LAYER                           │
│  • Contains business logic                                  │
│  • Validates business rules                                 │
│  • Orchestrates database operations                         │
│  • Maps entities to DTOs                                    │
└─────────────────────────────────────────────────────────────┘
                              │ EF Core queries
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   DATA LAYER (EF Core)                      │
│  • AppDbContext                                             │
│  • Entity configurations                                    │
│  • Migrations                                               │
└─────────────────────────────────────────────────────────────┘
                              │ SQL
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      SQL SERVER                             │
└─────────────────────────────────────────────────────────────┘
```

### Dependency Injection
All services are registered in `Program.cs`:

```csharp
// Services are "Scoped" = one instance per HTTP request
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJobSeekerService, JobSeekerService>();
builder.Services.AddScoped<IRecruiterService, RecruiterService>();
// ... etc
```

**Why interfaces?** 
- Testability: Can mock services in unit tests
- Flexibility: Can swap implementations without changing controllers
- Clean contracts: Interface defines what the service does

### Repository Pattern?
We chose **NOT** to use Repository pattern. Here's why:

| Approach | Pros | Cons |
|----------|------|------|
| Repository Pattern | Extra abstraction, testable | More code, EF Core is already a repository |
| Direct DbContext | Less code, EF Core handles it | Tightly coupled to EF |

**Our decision:** DbContext directly in services. EF Core's `DbSet<T>` is already a repository. Adding another layer would be over-engineering for our scale.

---

## 3. Authentication System

### Registration Flow
```
1. User sends: { email, password, firstName, lastName, accountType }
2. Server validates input
3. Server checks if email exists → returns error if yes
4. Server hashes password with BCrypt
5. Server creates User record (IsEmailVerified = false)
6. Server generates 6-digit OTP, stores hash in database
7. Server sends OTP via email
8. User receives email, sends OTP to verify-email endpoint
9. Server verifies OTP → sets IsEmailVerified = true
10. User must now explicitly call /login to get JWT token
```

### Login Flow
```
1. User sends: { email, password }
2. Server finds user by email
3. Server checks IsEmailVerified → returns 403 if false
4. Server checks if account is locked (failed attempts)
5. Server verifies password with BCrypt
6. If wrong → increment FailedLoginAttempts
7. If correct → reset failed attempts, generate JWT
8. Return JWT token + user info
```

### JWT Token Structure
```json
{
  "sub": "4",                          // User ID
  "email": "user@example.com",
  "name": "John Doe",
  "role": "JobSeeker",                 // or "Recruiter"
  "FirstName": "John",
  "LastName": "Doe",
  "exp": 1771678879,                   // Expiration timestamp
  "iss": "RecruitmentPlatformAPI",
  "aud": "RecruitmentPlatformClient"
}
```

### Extracting User ID in Controllers
```csharp
private int GetCurrentUserId()
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return int.TryParse(userIdClaim, out var userId) ? userId : 0;
}
```

### Security Features
- **Password hashing**: BCrypt with cost factor 12
- **Account lockout**: 5 failed attempts → 15 min lockout
- **OTP expiration**: 15 minutes
- **JWT expiration**: 24 hours
- **Constant-time comparison**: For verification codes (timing attack prevention)

---

## 4. Job Seeker Module

### Profile Completion Wizard (6 Steps)

Each step updates `User.ProfileCompletionStep`:

| Step | Endpoint | What's Saved |
|------|----------|--------------|
| 1 | `POST /api/jobseeker/personal-info` | Name, bio, job title, location, languages |
| 2 | `POST /api/jobseeker/projects` | Portfolio projects (multiple) |
| 3 | `POST /api/jobseeker/resume/upload` | PDF resume file |
| 4 | `POST /api/jobseeker/experience` | Work history (multiple) |
| 5 | `POST /api/jobseeker/education` | Education entries (multiple) |
| 6 | `PUT /api/jobseeker/social-accounts` | LinkedIn, GitHub, etc. |

### Wizard Status Response
```json
{
  "success": true,
  "data": {
    "currentStep": 3,
    "isComplete": false,
    "stepName": "CV Upload",
    "completedSteps": ["Personal Information", "Projects"]
  }
}
```

### Personal Info - Foreign Key Design
Instead of storing strings like `"Software Engineer"`, we store IDs:

```csharp
public class PersonalInfoRequestDto
{
    public int JobTitleId { get; set; }      // FK to JobTitle table
    public int CountryId { get; set; }       // FK to Country table
    public List<int> LanguageIds { get; set; } // FKs to Language table
}
```

**Why IDs instead of strings?**
- Data integrity (can't type "Softwre Enginer")
- Localization (same ID → different language labels)
- Easier filtering/searching
- Smaller storage

### Localization Support
```
GET /api/jobseeker/personal-info?lang=en  → "Egypt"
GET /api/jobseeker/personal-info?lang=ar  → "مصر"
```

Implemented by storing both names in reference tables:
```csharp
public class Country
{
    public int Id { get; set; }
    public string NameEn { get; set; }  // "Egypt"
    public string NameAr { get; set; }  // "مصر"
    public string Code { get; set; }    // "EG"
}
```

---

## 5. Recruiter Module

### Single-Step Profile
Recruiters have a simpler flow - just company information:

```json
// POST /api/recruiter/company-info
{
  "companyName": "Acme Corp",
  "companySize": "51-200",
  "industry": "Technology",
  "location": "Cairo, Egypt",
  "website": "https://acme.com",          // optional
  "linkedIn": "https://linkedin.com/...", // optional
  "companyDescription": "We build..."     // optional
}
```

### Why Strings for Industry/Company Size (Not Enums)?

We considered creating enums:
```csharp
public enum Industry { Technology, Finance, Healthcare, ... }
public enum CompanySize { OneToTen, ElevenToFifty, ... }
```

**We chose strings instead because:**

| Enums | Strings |
|-------|---------|
| Compile-time safety | Runtime validation |
| Need code change to add new | Can add via database |
| Serializes as `0, 1, 2` by default | Human-readable in JSON |
| `"1-10"` doesn't work as enum name | Works perfectly |

**Our implementation:**
```csharp
// Static lists in RecruiterService
private static readonly List<IndustryDto> Industries = new()
{
    new() { Name = "Technology" },
    new() { Name = "Finance & Banking" },
    // ... 20 industries
};

private static readonly HashSet<string> ValidIndustries =
    new(Industries.Select(i => i.Name), StringComparer.OrdinalIgnoreCase);

// Validation
if (!ValidIndustries.Contains(dto.Industry))
{
    return new ProfileResponseDto { Success = false, Message = "Invalid industry" };
}
```

---

## 6. Database Design

### Entity Relationship Overview
```
User (1) ─────────────── (0..1) JobSeeker
  │                              │
  │                              ├── (0..n) Experience
  │                              ├── (0..n) Education
  │                              ├── (0..n) Project
  │                              ├── (0..1) SocialAccount
  │                              ├── (0..1) Resume
  │                              └── (n) ─── (n) Language
  │
  └──────────────────── (0..1) Recruiter

JobTitle (1) ─── (n) JobSeeker
Country (1) ──── (n) JobSeeker
```

### Key Tables

#### User (Identity)
```sql
CREATE TABLE [User] (
    Id INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Email NVARCHAR(255) UNIQUE,
    PasswordHash NVARCHAR(255),
    AccountType NVARCHAR(20),        -- 'JobSeeker' or 'Recruiter'
    ProfileCompletionStep INT DEFAULT 0,
    IsEmailVerified BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    ProfilePictureUrl NVARCHAR(500),
    -- ... more fields
);
```

#### JobSeeker (Extended Profile)
```sql
CREATE TABLE JobSeekers (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT UNIQUE FOREIGN KEY REFERENCES [User](Id),
    JobTitleId INT FOREIGN KEY REFERENCES JobTitle(Id),
    CountryId INT FOREIGN KEY REFERENCES Country(Id),
    Bio NVARCHAR(500),
    City NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    DateOfBirth DATE,
    -- ... more fields
);
```

### Soft Delete Pattern
We don't actually delete records. Instead:

```csharp
public class Project
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;  // false = "deleted"
    // ...
}

// In service
public async Task<bool> DeleteProjectAsync(int userId, int projectId)
{
    project.IsActive = false;  // Soft delete
    await _context.SaveChangesAsync();
}

// When querying
var projects = await _context.Projects
    .Where(p => p.UserId == userId && p.IsActive)  // Exclude "deleted"
    .ToListAsync();
```

**Why soft delete?**
- Can restore accidentally deleted data
- Audit trail preserved
- Foreign key integrity maintained

---

## 7. API Response Standards

### Success Response
```json
{
  "success": true,
  "data": { /* actual data */ },
  "message": null
}
```

### Error Response
```json
{
  "success": false,
  "message": "Invalid industry. Please select from the provided list."
}
```

### Validation Error (400)
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "CompanyName": ["Company name is required"],
    "Email": ["Invalid email format"]
  }
}
```

### Standard Response DTOs
```csharp
// For data responses
public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public T? Data { get; set; }
    public string? Message { get; set; }
}

// For error responses
public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; }
}
```

---

## 8. Validation Strategy

### Two-Layer Validation

#### Layer 1: Model Validation (DTOs)
Using Data Annotations - happens automatically before controller method runs:

```csharp
public class RecruiterCompanyInfoRequestDto
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(150, MinimumLength = 2)]
    public string CompanyName { get; set; }

    [Required]
    [RegularExpression(@"^(1-10|11-50|51-200|201-500|501-1000|1000\+)$",
        ErrorMessage = "Invalid company size")]
    public string CompanySize { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    [StringLength(300)]
    public string? Website { get; set; }  // ? = optional
}
```

#### Layer 2: Business Validation (Services)
For rules that need database access:

```csharp
public async Task<ProfileResponseDto> SaveCompanyInfoAsync(...)
{
    // Check account type
    if (user.AccountType != AccountType.Recruiter)
    {
        return new ProfileResponseDto 
        { 
            Success = false, 
            Message = "Only recruiter accounts can save company info" 
        };
    }

    // Validate against static list
    if (!ValidIndustries.Contains(dto.Industry))
    {
        return new ProfileResponseDto 
        { 
            Success = false, 
            Message = "Invalid industry" 
        };
    }
}
```

---

## 9. File Upload Handling

### Resume Upload (PDF only)
```csharp
[HttpPost("upload")]
[Consumes("multipart/form-data")]
[RequestSizeLimit(5 * 1024 * 1024)]  // 5 MB limit
public async Task<IActionResult> UploadResume(IFormFile file)
```

**Validation checks:**
1. File not null/empty
2. Content type is `application/pdf`
3. File extension is `.pdf`
4. PDF magic bytes check (starts with `%PDF`)
5. Size ≤ 5 MB

**Storage:**
```
Uploads/
└── Resumes/
    └── resume_4_20260220130551.pdf  // resume_{userId}_{timestamp}.pdf
```

### Profile Picture Upload
Similar pattern but:
- Accepts: JPEG, PNG, WebP
- Max size: 2 MB
- Stored in `Uploads/ProfilePictures/`

### File Serving
```csharp
[HttpGet("{userId}")]
public async Task<IActionResult> DownloadResume(int userId)
{
    var (stream, contentType, fileName) = await _resumeService.GetResumeFileAsync(userId);
    return File(stream, contentType, fileName);
}
```

---

## 10. Implementation Decisions & Why

### Decision 1: Separate Controllers per Feature
**Instead of:** One giant `ProfileController` with 50 endpoints

**We did:** Separate controllers (Experience, Education, Projects, etc.)

**Why:**
- Single Responsibility Principle
- Easier to find code
- Smaller files
- Can have different auth requirements per controller

### Decision 2: Wizard Step Tracking in User Table
**Instead of:** Separate `WizardProgress` table

**We did:** `ProfileCompletionStep` column in `User` table

**Why:**
- Simpler queries (no joins needed)
- One source of truth
- Each step increments the number (1→2→3...)

### Decision 3: No Generic Repository
**Instead of:**
```csharp
public class Repository<T> : IRepository<T>
{
    public async Task<T> GetByIdAsync(int id) { ... }
    public async Task<IEnumerable<T>> GetAllAsync() { ... }
}
```

**We did:** Direct `DbContext` in services

**Why:**
- EF Core's `DbSet<T>` IS a repository
- Less abstraction = easier to understand
- Can write optimized queries directly

### Decision 4: DTOs for Everything
**Instead of:** Returning entity models directly

**We did:** Separate Request/Response DTOs

**Why:**
- Don't expose database structure
- Can shape data for frontend needs
- Prevents over-posting attacks
- Version API without changing database

```csharp
// Entity (database)
public class JobSeeker
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }  // Navigation property
    // ... 20 more fields
}

// DTO (API response) - only what frontend needs
public class PersonalInfoDto
{
    public string FullName { get; set; }
    public string JobTitle { get; set; }  // Resolved from JobTitleId
    public string Country { get; set; }   // Resolved from CountryId
}
```

### Decision 5: Async Everything
All database operations use `async/await`:

```csharp
public async Task<ProfileResponseDto> SavePersonalInfoAsync(int userId, PersonalInfoRequestDto dto)
{
    var user = await _context.Users.FindAsync(userId);
    // ...
    await _context.SaveChangesAsync();
}
```

**Why:** Web servers handle many requests. Async frees up threads while waiting for database.

### Decision 6: Route Naming Convention
```
/api/jobseeker/*      → Job seeker endpoints
/api/recruiter/*      → Recruiter endpoints
/api/auth/*           → Authentication
/api/locations/*      → Reference data
```

**Why:** Clear ownership, easy to understand, RESTful

---

## Quick Reference: Common Patterns

### Creating a New CRUD Feature

1. **Create Model** (`Models/JobSeeker/MyFeature.cs`)
```csharp
public class MyFeature
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
```

2. **Add to DbContext** (`Data/AppDbContext.cs`)
```csharp
public DbSet<MyFeature> MyFeatures { get; set; }
```

3. **Create DTOs** (`DTOs/JobSeeker/MyFeatureDtos.cs` or `DTOs/Recruiter/MyFeatureDtos.cs`)
```csharp
public class MyFeatureRequestDto { ... }
public class MyFeatureResponseDto { ... }
```

4. **Create Service Interface** (`Services/JobSeeker/IMyFeatureService.cs`)
```csharp
public interface IMyFeatureService
{
    Task<MyFeatureResponseDto> CreateAsync(int userId, MyFeatureRequestDto dto);
    Task<List<MyFeatureResponseDto>> GetAllAsync(int userId);
}
```

5. **Implement Service** (`Services/JobSeeker/MyFeatureService.cs`)

6. **Create Controller** (`Controllers/MyFeatureController.cs`)

7. **Register in Program.cs**
```csharp
builder.Services.AddScoped<IMyFeatureService, MyFeatureService>();
```

8. **Create Migration**
```bash
dotnet ef migrations add AddMyFeature
dotnet ef database update
```

---

## Troubleshooting

### "User not authenticated" error
- Check if JWT token is in `Authorization: Bearer <token>` header
- Check if token is expired (24 hours)
- Check if endpoint has `[Authorize]` attribute

### "Invalid object name" SQL error
- Run `dotnet ef database update`
- Check if model is registered in `AppDbContext`

### Changes not reflecting in database
- Did you call `await _context.SaveChangesAsync()`?
- Did you run migrations?

### Validation errors not showing
- Check if `[ApiController]` attribute is on controller
- Check if DTO has validation attributes

---

## Need More Info?

- **Auth flow details:** `Docs/API/AUTH_API_INTEGRATION.md`
- **All endpoints:** `Docs/API/API_REFERENCE.md`
- **Database diagram:** `Docs/Database/ERD_DIAGRAM.md`
- **Setup from scratch:** `Docs/Guides/SETUP_GUIDE.md`

Happy coding! 🚀
