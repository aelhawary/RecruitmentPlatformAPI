# Teammate Setup Guide — JobIntel Backend

> **Last updated:** February 21, 2026  
> **Time to set up:** ~10 minutes

---

## Prerequisites

- [ ] **.NET 9.0 SDK** — [download](https://dotnet.microsoft.com/download/dotnet/9.0)
- [ ] **SQL Server LocalDB** — included with Visual Studio, or install separately via [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [ ] **IDE** — Visual Studio 2022, VS Code (with C# Dev Kit), or JetBrains Rider

Verify .NET is installed:
```bash
dotnet --version
# Should show 9.0.x
```

---

## Step-by-Step Setup

### 1. Get the project

```bash
# If cloned via git:
git clone <repo-url>
cd Backend-2

# If received as a zip:
# Extract → open folder in your IDE
```

### 2. Create `appsettings.Development.json`

This file is **gitignored** — you must create it manually.

Create the file at: `RecruitmentPlatformAPI/appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharsLong12345"
  },
  "EmailSettings": {
    "SenderPassword": "ASK_TEAMMATE_FOR_APP_PASSWORD"
  },
  "GoogleOAuth": {
    "ClientId": "1094518034372-ka3p6p1dc6ur5d9os4pula12d2u9e7jl.apps.googleusercontent.com"
  }
}
```

> **Note:** Ask me for the Gmail app password (`SenderPassword`) — I'll send it to you directly. Everything else in `appsettings.json` (connection string, SMTP config, etc.) has safe defaults that work out of the box.

### 3. Restore packages

```bash
cd Backend-2
dotnet restore
```

### 4. Create the database

```bash
dotnet ef database update --project RecruitmentPlatformAPI
```

This will:
- Create `RecruitmentPlatformDb` on your LocalDB instance
- Run the single `InitialCreate` migration (all tables + seed data)
- Seed **90 job titles**, **65 countries**, **50 languages**, **assessment questions**, and **skills**

### 5. Build & Run

```bash
dotnet build
dotnet run --project RecruitmentPlatformAPI
```

### 6. Verify it works

Open in your browser: **http://localhost:5217/swagger**

You should see the Swagger UI with all API endpoints grouped by controller.

---

## Quick Verification Checklist

After setup, test these to confirm everything works:

- [ ] Swagger UI loads at `http://localhost:5217/swagger`
- [ ] `POST /api/auth/register` — register a test account (check for 200/201)
- [ ] Check your email for the verification link (confirms email service works)
- [ ] `POST /api/auth/verify-email` — verify the account
- [ ] `POST /api/auth/login` — login and get a JWT token
- [ ] Click **Authorize** in Swagger → paste the token → test an authenticated endpoint

---

## Project Structure (Quick Overview)

```
RecruitmentPlatformAPI/
├── Controllers/          # API endpoints
├── Services/
│   ├── Auth/             # Authentication, email, tokens
│   ├── JobSeeker/        # All JobSeeker profile services
│   └── Recruiter/        # Recruiter profile + Jobs (to be implemented)
├── DTOs/
│   ├── Auth/             # Auth request/response DTOs
│   ├── Common/           # Shared DTOs (ApiResponse, etc.)
│   ├── JobSeeker/        # JobSeeker-specific DTOs
│   └── Recruiter/        # Recruiter DTOs + JobDtos (to be created)
├── Models/
│   ├── Identity/         # User, EmailVerification, PasswordReset
│   ├── JobSeeker/        # JobSeeker profile entities
│   ├── Recruiter/        # Recruiter entity
│   ├── Jobs/             # Job, JobSkill, Recommendation
│   ├── Reference/        # Country, JobTitle, Language, Skill (seed data)
│   └── Assessment/       # Assessment quiz models
├── Data/
│   ├── AppDbContext.cs   # EF Core context with all configurations
│   ├── Migrations/       # Single clean migration
│   └── Seed/             # Seed data files
├── Enums/                # AccountType, EmploymentType, etc.
├── Configuration/        # Settings classes (JWT, Email, FileStorage)
├── Program.cs            # DI registration, middleware pipeline
└── appsettings.json      # Base config (safe to commit)
```

---

## Your Task: Jobs Module

See **`Docs/Guides/JOBS_MODULE_IMPLEMENTATION_GUIDE.md`** for the complete implementation guide.

**Summary — you need to create 4 files:**

| # | File | What it does |
|---|------|-------------|
| 1 | `DTOs/Recruiter/JobDtos.cs` | Request/response DTOs for jobs + candidates |
| 2 | `Services/Recruiter/IJobService.cs` | Service interface |
| 3 | `Services/Recruiter/JobService.cs` | Service implementation |
| 4 | `Controllers/JobsController.cs` | API controller (9 endpoints) |

**Plus one line in `Program.cs`:**
```csharp
builder.Services.AddScoped<IJobService, JobService>();
```

**No database migration needed** — the `Jobs`, `JobSkills`, `Recommendations`, and `Skills` tables are already in the schema.

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| `dotnet ef` not found | Run: `dotnet tool install --global dotnet-ef` |
| LocalDB connection fails | Open Visual Studio Installer → ensure "SQL Server Express LocalDB" is checked |
| Port 5217 in use | Change port in `Properties/launchSettings.json` |
| Email not sending | Make sure `SenderPassword` in `appsettings.Development.json` is the correct Gmail App Password (not your Gmail password) |
| Google OAuth fails | The `ClientId` in the config is shared — it should work as-is for development |
| Build errors about missing packages | Run `dotnet restore` first |

---

## Key Conventions to Follow

- **Response wrapper:** Always use `ApiResponse<T>` for success, `ApiErrorResponse` for errors
- **Logging:** Use structured logging: `_logger.LogInformation("Job {JobId} created", id)`
- **DateTime:** Always `DateTime.UtcNow`
- **Strings:** Always `.Trim()` user inputs
- **Namespace aliases:** Use when folder name = class name (e.g., `using JobSeekerEntity = ...JobSeeker.JobSeeker;`)
- **Auth:** Use `GetCurrentUserId()` helper in controllers, check ownership in services

See the full conventions table in the Jobs Module guide.
