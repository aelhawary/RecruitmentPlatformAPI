# Entity Relationship Diagram (ERD)
## JobIntel Recruitment Platform Database Schema

This document contains the ERD for the JobIntel Recruitment Platform database.

---

## Database Overview

**Total Tables:** 19
- **Core Entities:** 3 (User, JobSeeker, Recruiter)
- **Profile Data:** 6 (Education, Experience, Project, Resume, SocialAccount, Skill)
- **Job Management:** 2 (Job, Recommendation)
- **Reference/Lookup:** 3 (Country, Language, JobTitle)
- **Many-to-Many Junctions:** 2 (JobSeekerSkill, JobSkill)
- **Auth/Security:** 2 (EmailVerification, PasswordReset)

---

## ERD Diagram (Mermaid Format)

```mermaid
erDiagram
    %% Core User System
    User ||--o{ EmailVerification : "has many"
    User ||--o{ PasswordReset : "has many"
    User ||--o| JobSeeker : "has one (if AccountType=JobSeeker)"
    User ||--o| Recruiter : "has one (if AccountType=Recruiter)"
    
    %% JobSeeker Profile Components
    JobSeeker ||--o{ Education : "has many"
    JobSeeker ||--o{ Experience : "has many"
    JobSeeker ||--o{ Project : "has many"
    JobSeeker ||--o{ Resume : "has many"
    JobSeeker ||--o| SocialAccount : "has one"
    JobSeeker ||--o{ JobSeekerSkill : "has many"
    JobSeeker ||--o{ Recommendation : "receives many"
    
    %% JobSeeker Foreign Keys to Reference Tables
    JobSeeker }o--|| JobTitle : "belongs to"
    JobSeeker }o--|| Country : "located in"
    JobSeeker }o--|| Language : "first language"
    JobSeeker }o--|| Language : "second language (optional)"
    
    %% Recruiter and Jobs
    Recruiter ||--o{ Job : "posts many"
    Job ||--o{ JobSkill : "requires many"
    Job ||--o{ Recommendation : "generates many"
    
    %% Skills Many-to-Many
    JobSeekerSkill }o--|| JobSeeker : "belongs to"
    JobSeekerSkill }o--|| Skill : "references"
    JobSkill }o--|| Job : "belongs to"
    JobSkill }o--|| Skill : "references"
    
    %% Recommendation Junction
    Recommendation }o--|| Job : "for job"
    Recommendation }o--|| JobSeeker : "for candidate"
    
    %% Entity Definitions
    User {
        int Id PK
        string FirstName
        string LastName
        string Email UK
        string PasswordHash "nullable for OAuth"
        enum AuthProvider "Email or Google"
        string ProviderUserId "OAuth identifier"
        string ProfilePictureUrl
        enum AccountType "JobSeeker or Recruiter"
        bool IsEmailVerified
        bool IsActive
        int FailedLoginAttempts
        datetime LockoutEnd
        int ProfileCompletionStep "0-6"
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    JobSeeker {
        int Id PK
        int UserId FK "One-to-One"
        int JobTitleId FK
        int YearsOfExperience
        int CountryId FK
        string City
        string PhoneNumber
        int FirstLanguageId FK
        enum FirstLanguageProficiency
        int SecondLanguageId FK
        enum SecondLanguageProficiency
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    Recruiter {
        int Id PK
        int UserId FK "One-to-One"
        string CompanyName
        string CompanySize
        string Industry
        string Location
        string Website
        string LinkedIn
        string CompanyDescription
        string LogoUrl
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    Education {
        int Id PK
        int JobSeekerId FK
        string Institution
        string Degree
        string GradeOrGPA
        datetime StartDate
        datetime EndDate
        bool IsCurrent
        string Description
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    Experience {
        int Id PK
        int JobSeekerId FK
        string JobTitle
        string CompanyName
        string EmploymentStatus
        datetime StartDate
        datetime EndDate
        bool IsCurrent
        string Responsibilities
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    Project {
        int Id PK
        int JobSeekerId FK
        string Title
        string TechnologiesUsed
        string Description
        string ProjectLink
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    Resume {
        int Id PK
        int JobSeekerId FK
        string FileName
        string FileUrl
        datetime UploadedAt
        string ParseStatus "Pending/Processing/Completed/Failed"
        datetime ProcessedAt
        bool IsActive
    }
    
    SocialAccount {
        int Id PK
        int JobSeekerId FK "One-to-One"
        string LinkedIn
        string Github
        string Behance
        string Dribbble
        string PersonalWebsite
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    Job {
        int Id PK
        int RecruiterId FK
        string Title
        string Description
        string Requirements
        string EmploymentType
        int MinYearsOfExperience
        string Location
        datetime PostedAt
        datetime UpdatedAt
        bool IsActive
    }
    
    Skill {
        int Id PK
        string Name UK
        datetime CreatedAt
    }
    
    JobSeekerSkill {
        int Id PK
        int JobSeekerId FK
        int SkillId FK
        string Source "Self/CV/Manual"
    }
    
    JobSkill {
        int Id PK
        int JobId FK
        int SkillId FK
    }
    
    Recommendation {
        int Id PK
        int JobId FK
        int JobSeekerId FK
        decimal MatchScore "0.00-100.00"
        datetime GeneratedAt
    }
    
    Country {
        int Id PK
        string IsoCode UK "ISO 3166-1"
        string NameEn
        string NameAr
        string PhoneCode "+20, +1"
        bool IsActive
        int SortOrder
        datetime CreatedAt
    }
    
    Language {
        int Id PK
        string IsoCode UK "ISO 639-2/3"
        string NameEn
        string NameAr
        bool IsActive
        int SortOrder
        datetime CreatedAt
    }
    
    JobTitle {
        int Id PK
        string Title UK
        string Category "Technology/Design/Marketing/etc"
        bool IsActive
        datetime CreatedAt
    }
    
    EmailVerification {
        int Id PK
        int UserId FK
        string VerificationCode "6-digit"
        datetime CreatedAt
        datetime ExpiresAt "15 minutes"
        bool IsUsed
    }
    
    PasswordReset {
        int Id PK
        int UserId FK
        string OtpCode "6-digit"
        datetime CreatedAt
        datetime ExpiresAt "15 minutes"
        bool IsUsed
    }
```

---

## Relationship Details

### One-to-One Relationships
1. **User → JobSeeker** (Conditional: when AccountType = JobSeeker)
2. **User → Recruiter** (Conditional: when AccountType = Recruiter)
3. **JobSeeker → SocialAccount** (One job seeker has one social account)

### One-to-Many Relationships

#### User Relationships
- User → EmailVerification (1:N)
- User → PasswordReset (1:N)

#### JobSeeker Relationships
- JobSeeker → Education (1:N)
- JobSeeker → Experience (1:N)
- JobSeeker → Project (1:N)
- JobSeeker → Resume (1:N)
- JobSeeker → JobSeekerSkill (1:N)
- JobSeeker → Recommendation (1:N)

#### Reference Data Relationships
- JobTitle → JobSeeker (1:N)
- Country → JobSeeker (1:N)
- Language → JobSeeker as FirstLanguage (1:N)
- Language → JobSeeker as SecondLanguage (1:N)

#### Recruiter Relationships
- Recruiter → Job (1:N)

#### Job Relationships
- Job → JobSkill (1:N)
- Job → Recommendation (1:N)

### Many-to-Many Relationships

1. **JobSeeker ↔ Skill** (via JobSeekerSkill junction table)
   - A job seeker can have multiple skills
   - A skill can belong to multiple job seekers

2. **Job ↔ Skill** (via JobSkill junction table)
   - A job can require multiple skills
   - A skill can be required by multiple jobs

3. **Job ↔ JobSeeker** (via Recommendation junction table)
   - A job can be recommended to multiple job seekers
   - A job seeker can receive recommendations for multiple jobs
   - Contains computed MatchScore attribute

---

## Key Features & Constraints

### Authentication System
- **Multi-Provider Auth:** Supports Email and Google OAuth
- **Email Verification:** 6-digit OTP with 15-minute expiration
- **Password Reset:** Secure OTP-based password recovery
- **Account Lockout:** 5 failed attempts trigger 30-minute lockout

### Profile System
- **Wizard-Based Completion:** 6-step progressive profile building
  - Step 0: Not Started
  - Step 1: Personal Information
  - Step 2: Projects
  - Step 3: CV Upload
  - Step 4: Experience
  - Step 5: Education
  - Step 6: Social Links (Complete)

### Localization Support
- **Bilingual Reference Data:** Country, Language tables have English and Arabic names
- **Client-Side Selection:** Frontend sends preferred language (en/ar) in API requests
- **Dynamic Response:** API returns localized field names based on language parameter

### Data Integrity
- **Unique Constraints:**
  - User.Email (unique, case-insensitive)
  - Country.IsoCode
  - Language.IsoCode
  - JobTitle.Title
  - Skill.Name

- **Required Fields:** All foreign keys and core attributes are non-nullable
- **Optional Fields:** Profile completion allows progressive data entry

### Soft Deletes
- Resume.IsActive (allows multiple CV versions, only one active)
- Job.IsActive (archive jobs without deletion)
- Reference tables (Country, Language, JobTitle) have IsActive flags

---

## Database Seeding

### Initial Reference Data
- **Countries:** 65 countries with localized names (Egypt prioritized)
- **Languages:** 50 languages with ISO codes and localized names
- **Job Titles:** 90 titles across 8 categories
  - Technology (30)
  - Design (10)
  - Marketing (10)
  - Sales (10)
  - Finance (10)
  - Human Resources (5)
  - Operations (10)
  - Executive (5)

---

## Indexes & Performance

### Recommended Indexes (for production)
```sql
-- Authentication lookups
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_ProviderUserId ON Users(ProviderUserId);
CREATE INDEX IX_EmailVerifications_UserId ON EmailVerifications(UserId);
CREATE INDEX IX_PasswordResets_UserId_ExpiresAt ON PasswordResets(UserId, ExpiresAt);

-- Profile queries
CREATE INDEX IX_JobSeekers_UserId ON JobSeekers(UserId);
CREATE INDEX IX_Recruiters_UserId ON Recruiters(UserId);
CREATE INDEX IX_JobSeekers_CountryId ON JobSeekers(CountryId);
CREATE INDEX IX_JobSeekers_JobTitleId ON JobSeekers(JobTitleId);

-- Job searches
CREATE INDEX IX_Jobs_RecruiterId_IsActive ON Jobs(RecruiterId, IsActive);
CREATE INDEX IX_Jobs_PostedAt_IsActive ON Jobs(PostedAt, IsActive);

-- Recommendation system
CREATE INDEX IX_Recommendations_JobSeekerId_MatchScore ON Recommendations(JobSeekerId, MatchScore DESC);
CREATE INDEX IX_Recommendations_JobId_MatchScore ON Recommendations(JobId, MatchScore DESC);

-- Skills matching
CREATE INDEX IX_JobSeekerSkills_JobSeekerId ON JobSeekerSkills(JobSeekerId);
CREATE INDEX IX_JobSeekerSkills_SkillId ON JobSeekerSkills(SkillId);
CREATE INDEX IX_JobSkills_JobId ON JobSkills(JobId);
CREATE INDEX IX_JobSkills_SkillId ON JobSkills(SkillId);
```

---

## Technology Stack

- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core 9.0.10
- **Database:** SQL Server (LocalDB for dev)
- **Migrations:** Single consolidated InitialCreate migration
- **Seed Data:** Loaded via EF Core migration

---

## Notes for Graduation Documentation

1. **Normalization Level:** Database follows 3NF (Third Normal Form)
   - No transitive dependencies
   - All non-key attributes depend on primary key
   - Many-to-many relationships properly decomposed with junction tables

2. **Scalability Considerations:**
   - User-AgnosticProfile separation (User vs JobSeeker/Recruiter)
   - Reference data tables prevent redundancy
   - Junction tables enable efficient many-to-many queries
   - Soft deletes preserve historical data

3. **Security Features:**
   - Password hashing (never store plain text)
   - OTP expiration for time-limited operations
   - Account lockout mechanism
   - OAuth integration for third-party authentication

4. **Business Logic:**
   - Match scoring system for recommendations
   - Progressive profile completion wizard
   - Multi-language support for international audience
   - Role-based access (JobSeeker vs Recruiter)

---

## How to Export This Diagram

### Option 1: Render Mermaid Diagram Online
1. Copy the mermaid code block above
2. Go to https://mermaid.live/
3. Paste the code
4. Export as PNG, SVG, or PDF

### Option 2: Use VS Code Extension
1. Install "Markdown Preview Mermaid Support" extension
2. Open this file in VS Code
3. Press `Ctrl+Shift+V` to preview
4. Right-click on diagram → "Copy as Image"

### Option 3: Use Draw.io / Lucidchart
1. Import the relationship details above
2. Create a visual ERD using their drag-and-drop interface
3. Export as high-resolution image

### Option 4: Use Database Tools
1. Open SQL Server Management Studio (SSMS)
2. Right-click database → "Diagrams" → "New Database Diagram"
3. Select all tables
4. Arrange and export

---

**Created for:** JobIntel Recruitment Platform  
**Date:** December 29, 2025  
**Database:** RecruitmentPlatformDb  
**Version:** 1.0  
