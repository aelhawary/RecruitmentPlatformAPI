using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecruitmentPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsoCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTitle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RoleFamily = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsoCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    AuthProvider = table.Column<int>(type: "integer", nullable: false),
                    ProviderUserId = table.Column<string>(type: "text", nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "text", nullable: true),
                    AccountType = table.Column<string>(type: "text", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LastFailedLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LockoutEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LockoutReason = table.Column<string>(type: "text", nullable: true),
                    LastSuccessfulLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProfileCompletionStep = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    RoleFamily = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: true),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    SeniorityLevel = table.Column<int>(type: "integer", nullable: false),
                    Options = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CorrectAnswerIndex = table.Column<int>(type: "integer", nullable: false),
                    TimePerQuestion = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Explanation = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentQuestion_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    VerificationCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailVerifications_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSeekers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JobTitleId = table.Column<int>(type: "integer", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FirstLanguageId = table.Column<int>(type: "integer", nullable: true),
                    FirstLanguageProficiency = table.Column<string>(type: "text", nullable: true),
                    SecondLanguageId = table.Column<int>(type: "integer", nullable: true),
                    SecondLanguageProficiency = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentAssessmentScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    LastAssessmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssessmentJobTitleId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSeekers_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSeekers_JobTitle_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSeekers_Language_FirstLanguageId",
                        column: x => x.FirstLanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSeekers_Language_SecondLanguageId",
                        column: x => x.SecondLanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSeekers_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResets_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recruiters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CompanySize = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Industry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Website = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    LinkedIn = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CompanyDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recruiters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recruiters_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentAttempt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobSeekerId = table.Column<int>(type: "integer", nullable: false),
                    JobTitleId = table.Column<int>(type: "integer", nullable: false),
                    OverallScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    TechnicalScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    SoftSkillsScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TimeLimitMinutes = table.Column<int>(type: "integer", nullable: false),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false),
                    QuestionsAnswered = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScoreExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RetakeNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentAttempt_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentAttempt_JobTitle_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobSeekerId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    IssuingOrganization = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FileName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    StoredFileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FilePath = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                    table.CheckConstraint("CK_Certificate_ExpirationDateAfterIssueDate", "[ExpirationDate] IS NULL OR [IssueDate] IS NULL OR [ExpirationDate] >= [IssueDate]");
                    table.ForeignKey(
                        name: "FK_Certificates_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobSeekerId = table.Column<int>(type: "integer", nullable: false),
                    Institution = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Degree = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Major = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    GradeOrGPA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.Id);
                    table.CheckConstraint("CK_Education_EndDateAfterStartDate", "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
                    table.ForeignKey(
                        name: "FK_Educations_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Experiences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobSeekerId = table.Column<int>(type: "integer", nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    EmploymentType = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    Responsibilities = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiences", x => x.Id);
                    table.CheckConstraint("CK_Experience_EndDateAfterStartDate", "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
                    table.ForeignKey(
                        name: "FK_Experiences_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSeekerSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobSeekerId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSeekerSkills_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSeekerSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobSeekerId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TechnologiesUsed = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: true),
                    ProjectLink = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resumes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobSeekerId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    StoredFileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    ParseStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resumes_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobSeekerId = table.Column<int>(type: "integer", nullable: false),
                    LinkedIn = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Github = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Behance = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Dribbble = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    PersonalWebsite = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialAccounts_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecruiterId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false),
                    Requirements = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false),
                    EmploymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MinYearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Recruiters_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "Recruiters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssessmentAttemptId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    SelectedAnswerIndex = table.Column<int>(type: "integer", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "integer", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentAnswer_AssessmentAttempt_AssessmentAttemptId",
                        column: x => x.AssessmentAttemptId,
                        principalTable: "AssessmentAttempt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentAnswer_AssessmentQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "AssessmentQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSkills_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    JobSeekerId = table.Column<int>(type: "integer", nullable: false),
                    MatchScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recommendations_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recommendations_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "Id", "CreatedAt", "IsActive", "IsoCode", "NameAr", "NameEn", "PhoneCode", "SortOrder" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "EG", "مصر", "Egypt", "+20", 1 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "SA", "المملكة العربية السعودية", "Saudi Arabia", "+966", 2 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "AE", "الإمارات العربية المتحدة", "United Arab Emirates", "+971", 3 },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "KW", "الكويت", "Kuwait", "+965", 4 },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "QA", "قطر", "Qatar", "+974", 5 },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BH", "البحرين", "Bahrain", "+973", 6 },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "OM", "عمان", "Oman", "+968", 7 },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "JO", "الأردن", "Jordan", "+962", 8 },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "LB", "لبنان", "Lebanon", "+961", 9 },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "IQ", "العراق", "Iraq", "+964", 10 },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "SY", "سوريا", "Syria", "+963", 11 },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "YE", "اليمن", "Yemen", "+967", 12 },
                    { 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "PS", "فلسطين", "Palestine", "+970", 13 },
                    { 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "LY", "ليبيا", "Libya", "+218", 14 },
                    { 15, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "TN", "تونس", "Tunisia", "+216", 15 },
                    { 16, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "DZ", "الجزائر", "Algeria", "+213", 16 },
                    { 17, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "MA", "المغرب", "Morocco", "+212", 17 },
                    { 18, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "SD", "السودان", "Sudan", "+249", 18 },
                    { 19, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "US", "الولايات المتحدة", "United States", "+1", 100 },
                    { 20, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "GB", "المملكة المتحدة", "United Kingdom", "+44", 101 },
                    { 21, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "CA", "كندا", "Canada", "+1", 102 },
                    { 22, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "AU", "أستراليا", "Australia", "+61", 103 },
                    { 23, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "DE", "ألمانيا", "Germany", "+49", 104 },
                    { 24, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "FR", "فرنسا", "France", "+33", 105 },
                    { 25, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "IT", "إيطاليا", "Italy", "+39", 106 },
                    { 26, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ES", "إسبانيا", "Spain", "+34", 107 },
                    { 27, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "NL", "هولندا", "Netherlands", "+31", 108 },
                    { 28, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "SE", "السويد", "Sweden", "+46", 109 },
                    { 29, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "NO", "النرويج", "Norway", "+47", 110 },
                    { 30, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "CH", "سويسرا", "Switzerland", "+41", 111 },
                    { 31, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "CN", "الصين", "China", "+86", 200 },
                    { 32, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "JP", "اليابان", "Japan", "+81", 201 },
                    { 33, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "IN", "الهند", "India", "+91", 202 },
                    { 34, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "KR", "كوريا الجنوبية", "South Korea", "+82", 203 },
                    { 35, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "SG", "سنغافورة", "Singapore", "+65", 204 },
                    { 36, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "MY", "ماليزيا", "Malaysia", "+60", 205 },
                    { 37, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "TH", "تايلاند", "Thailand", "+66", 206 },
                    { 38, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "PH", "الفلبين", "Philippines", "+63", 207 },
                    { 39, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ID", "إندونيسيا", "Indonesia", "+62", 208 },
                    { 40, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "VN", "فيتنام", "Vietnam", "+84", 209 },
                    { 41, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "PK", "باكستان", "Pakistan", "+92", 210 },
                    { 42, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BD", "بنغلاديش", "Bangladesh", "+880", 211 },
                    { 43, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "TR", "تركيا", "Turkey", "+90", 212 },
                    { 44, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "IR", "إيران", "Iran", "+98", 214 },
                    { 45, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ZA", "جنوب أفريقيا", "South Africa", "+27", 300 },
                    { 46, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "NG", "نيجيريا", "Nigeria", "+234", 301 },
                    { 47, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "KE", "كينيا", "Kenya", "+254", 302 },
                    { 48, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ET", "إثيوبيا", "Ethiopia", "+251", 303 },
                    { 49, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "GH", "غانا", "Ghana", "+233", 304 },
                    { 50, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BR", "البرازيل", "Brazil", "+55", 400 },
                    { 51, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "MX", "المكسيك", "Mexico", "+52", 401 },
                    { 52, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "AR", "الأرجنتين", "Argentina", "+54", 402 },
                    { 53, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "CL", "تشيلي", "Chile", "+56", 403 },
                    { 54, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "CO", "كولومبيا", "Colombia", "+57", 404 },
                    { 55, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "PL", "بولندا", "Poland", "+48", 500 },
                    { 56, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "RO", "رومانيا", "Romania", "+40", 501 },
                    { 57, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "GR", "اليونان", "Greece", "+30", 502 },
                    { 58, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "PT", "البرتغال", "Portugal", "+351", 503 },
                    { 59, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "BE", "بلجيكا", "Belgium", "+32", 504 },
                    { 60, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "AT", "النمسا", "Austria", "+43", 505 },
                    { 61, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "DK", "الدنمارك", "Denmark", "+45", 506 },
                    { 62, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "FI", "فنلندا", "Finland", "+358", 507 },
                    { 63, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "IE", "أيرلندا", "Ireland", "+353", 508 },
                    { 64, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "NZ", "نيوزيلندا", "New Zealand", "+64", 509 },
                    { 65, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "RU", "روسيا", "Russia", "+7", 510 }
                });

            migrationBuilder.InsertData(
                table: "JobTitle",
                columns: new[] { "Id", "Category", "CreatedAt", "IsActive", "RoleFamily", "Title" },
                values: new object[,]
                {
                    { 1, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2, "Backend Developer" },
                    { 2, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, "Frontend Developer" },
                    { 3, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, "Full Stack Developer" },
                    { 4, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 4, "Mobile Developer" },
                    { 5, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 4, "iOS Developer" },
                    { 6, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 4, "Android Developer" },
                    { 7, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 6, "DevOps Engineer" },
                    { 8, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, "Data Scientist" },
                    { 9, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, "Data Engineer" },
                    { 10, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, "Machine Learning Engineer" },
                    { 11, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, "AI Engineer" },
                    { 12, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, "Software Engineer" },
                    { 13, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 7, "QA Engineer" },
                    { 14, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 7, "Test Automation Engineer" },
                    { 15, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 6, "Cloud Engineer" },
                    { 16, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 6, "Security Engineer" },
                    { 17, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 6, "Cybersecurity Analyst" },
                    { 18, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 6, "Network Engineer" },
                    { 19, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 6, "Systems Administrator" },
                    { 20, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2, "Database Administrator" },
                    { 21, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, "Solutions Architect" },
                    { 22, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, "Technical Architect" },
                    { 23, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 6, "Site Reliability Engineer" },
                    { 24, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Embedded Systems Engineer" },
                    { 25, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Game Developer" },
                    { 26, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2, "Blockchain Developer" },
                    { 27, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "IoT Engineer" },
                    { 28, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, "Computer Vision Engineer" },
                    { 29, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, "NLP Engineer" },
                    { 30, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, "Business Intelligence Analyst" },
                    { 31, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, "Data Analyst" },
                    { 32, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "IT Support Specialist" },
                    { 33, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Technical Support Engineer" },
                    { 34, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "IT Manager" },
                    { 35, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "CTO" },
                    { 36, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Engineering Manager" },
                    { 37, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, "Technical Lead" },
                    { 38, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Scrum Master" },
                    { 39, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Product Manager" },
                    { 40, "Technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Technical Product Manager" },
                    { 41, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "UX Designer" },
                    { 42, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "UI Designer" },
                    { 43, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "UX/UI Designer" },
                    { 44, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "Graphic Designer" },
                    { 45, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "Web Designer" },
                    { 46, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "Visual Designer" },
                    { 47, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "Product Designer" },
                    { 48, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "Interaction Designer" },
                    { 49, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "Motion Designer" },
                    { 50, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "3D Designer" },
                    { 51, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "Game Designer" },
                    { 52, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "UX Researcher" },
                    { 53, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "Creative Director" },
                    { 54, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "Art Director" },
                    { 55, "Design", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 8, "Brand Designer" },
                    { 56, "Marketing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Digital Marketing Specialist" },
                    { 57, "Marketing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "SEO Specialist" },
                    { 58, "Marketing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Content Marketing Manager" },
                    { 59, "Marketing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Social Media Manager" },
                    { 60, "Marketing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Marketing Manager" },
                    { 61, "Marketing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Brand Manager" },
                    { 62, "Marketing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Growth Manager" },
                    { 63, "Marketing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Email Marketing Specialist" },
                    { 64, "Marketing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Marketing Analyst" },
                    { 65, "Marketing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Content Writer" },
                    { 66, "Sales", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Sales Representative" },
                    { 67, "Sales", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Account Executive" },
                    { 68, "Sales", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Sales Manager" },
                    { 69, "Sales", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Business Development Manager" },
                    { 70, "Sales", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Customer Success Manager" },
                    { 71, "Finance", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Accountant" },
                    { 72, "Finance", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Financial Analyst" },
                    { 73, "Finance", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Finance Manager" },
                    { 74, "Finance", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "CFO" },
                    { 75, "Finance", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Investment Analyst" },
                    { 76, "Human Resources", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "HR Manager" },
                    { 77, "Human Resources", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Recruiter" },
                    { 78, "Human Resources", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Talent Acquisition Specialist" },
                    { 79, "Human Resources", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "HR Business Partner" },
                    { 80, "Human Resources", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "People Operations Manager" },
                    { 81, "Operations", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Operations Manager" },
                    { 82, "Operations", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Project Manager" },
                    { 83, "Operations", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Program Manager" },
                    { 84, "Operations", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Supply Chain Manager" },
                    { 85, "Operations", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "Logistics Coordinator" },
                    { 86, "Executive", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "CEO" },
                    { 87, "Executive", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "COO" },
                    { 88, "Executive", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "VP of Engineering" },
                    { 89, "Executive", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "VP of Product" },
                    { 90, "Executive", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 9, "VP of Sales" }
                });

            migrationBuilder.InsertData(
                table: "Language",
                columns: new[] { "Id", "CreatedAt", "IsActive", "IsoCode", "NameAr", "NameEn", "SortOrder" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ara", "العربية", "Arabic", 1 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "eng", "الإنجليزية", "English", 2 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "tur", "التركية", "Turkish", 10 },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "fas", "الفارسية", "Persian (Farsi)", 11 },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "heb", "العبرية", "Hebrew", 12 },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "kur", "الكردية", "Kurdish", 13 },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "urd", "الأردية", "Urdu", 14 },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "fra", "الفرنسية", "French", 20 },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "deu", "الألمانية", "German", 21 },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "spa", "الإسبانية", "Spanish", 22 },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ita", "الإيطالية", "Italian", 23 },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "por", "البرتغالية", "Portuguese", 24 },
                    { 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "rus", "الروسية", "Russian", 25 },
                    { 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "nld", "الهولندية", "Dutch", 26 },
                    { 15, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "pol", "البولندية", "Polish", 27 },
                    { 16, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ukr", "الأوكرانية", "Ukrainian", 28 },
                    { 17, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "swe", "السويدية", "Swedish", 29 },
                    { 18, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "nor", "النرويجية", "Norwegian", 30 },
                    { 19, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "dan", "الدنماركية", "Danish", 31 },
                    { 20, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "fin", "الفنلندية", "Finnish", 32 },
                    { 21, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ell", "اليونانية", "Greek", 33 },
                    { 22, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "zho", "الصينية (الماندرين)", "Chinese (Mandarin)", 40 },
                    { 23, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "jpn", "اليابانية", "Japanese", 41 },
                    { 24, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "kor", "الكورية", "Korean", 42 },
                    { 25, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "hin", "الهندية", "Hindi", 43 },
                    { 26, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ben", "البنغالية", "Bengali", 44 },
                    { 27, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "vie", "الفيتنامية", "Vietnamese", 45 },
                    { 28, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "tha", "التايلاندية", "Thai", 46 },
                    { 29, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ind", "الإندونيسية", "Indonesian", 47 },
                    { 30, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "msa", "الماليزية", "Malay", 48 },
                    { 31, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "fil", "الفلبينية (تاغالوغ)", "Filipino (Tagalog)", 49 },
                    { 32, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "swa", "السواحيلية", "Swahili", 50 },
                    { 33, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "amh", "الأمهرية", "Amharic", 51 },
                    { 34, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "hau", "الهوسا", "Hausa", 52 },
                    { 35, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "yor", "اليوروبا", "Yoruba", 53 },
                    { 36, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "zul", "الزولو", "Zulu", 54 },
                    { 37, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ron", "الرومانية", "Romanian", 60 },
                    { 38, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ces", "التشيكية", "Czech", 61 },
                    { 39, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "hun", "المجرية", "Hungarian", 62 },
                    { 40, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "bul", "البلغارية", "Bulgarian", 63 },
                    { 41, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "hrv", "الكرواتية", "Croatian", 64 },
                    { 42, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "srp", "الصربية", "Serbian", 65 },
                    { 43, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "slk", "السلوفاكية", "Slovak", 66 },
                    { 44, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "slv", "السلوفينية", "Slovenian", 67 },
                    { 45, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "lit", "الليتوانية", "Lithuanian", 68 },
                    { 46, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "lav", "اللاتفية", "Latvian", 69 },
                    { 47, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "est", "الإستونية", "Estonian", 70 },
                    { 48, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "cat", "الكتالونية", "Catalan", 71 },
                    { 49, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "glg", "الجاليكية", "Galician", 72 },
                    { 50, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "eus", "الباسكية", "Basque", 73 }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "C#" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "JavaScript" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TypeScript" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Python" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Java" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "C++" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PHP" },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ruby" },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Go" },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Swift" },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "React" },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Angular" },
                    { 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vue.js" },
                    { 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Next.js" },
                    { 15, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HTML/CSS" },
                    { 16, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tailwind CSS" },
                    { 17, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ASP.NET Core" },
                    { 18, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Node.js" },
                    { 19, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Django" },
                    { 20, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Spring Boot" },
                    { 21, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Express.js" },
                    { 22, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flask" },
                    { 23, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SQL Server" },
                    { 24, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PostgreSQL" },
                    { 25, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MySQL" },
                    { 26, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MongoDB" },
                    { 27, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Redis" },
                    { 28, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Entity Framework" },
                    { 29, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AWS" },
                    { 30, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Azure" },
                    { 31, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Docker" },
                    { 32, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kubernetes" },
                    { 33, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CI/CD" },
                    { 34, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Git" },
                    { 35, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Linux" },
                    { 36, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "React Native" },
                    { 37, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flutter" },
                    { 38, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Android" },
                    { 39, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "iOS" },
                    { 40, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Machine Learning" },
                    { 41, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Data Analysis" },
                    { 42, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Power BI" },
                    { 43, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "REST APIs" },
                    { 44, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "GraphQL" },
                    { 45, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agile/Scrum" },
                    { 46, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Unit Testing" },
                    { 47, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Problem Solving" },
                    { 48, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Communication" },
                    { 49, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Project Management" },
                    { 50, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "UI/UX Design" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAnswer_Attempt_Question",
                table: "AssessmentAnswer",
                columns: new[] { "AssessmentAttemptId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAnswer_QuestionId",
                table: "AssessmentAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAttempt_JobSeeker_Active",
                table: "AssessmentAttempt",
                columns: new[] { "JobSeekerId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAttempt_JobSeeker_Status",
                table: "AssessmentAttempt",
                columns: new[] { "JobSeekerId", "Status", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAttempt_JobTitleId",
                table: "AssessmentAttempt",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAttempt_SingleInProgress",
                table: "AssessmentAttempt",
                column: "JobSeekerId",
                unique: true,
                filter: "[Status] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQuestion_Filtering",
                table: "AssessmentQuestion",
                columns: new[] { "RoleFamily", "Category", "Difficulty", "SeniorityLevel", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQuestion_SkillId",
                table: "AssessmentQuestion",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_JobSeekerId_IsDeleted",
                table: "Certificates",
                columns: new[] { "JobSeekerId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Country_IsoCode",
                table: "Country",
                column: "IsoCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Education_JobSeekerId_IsDeleted",
                table: "Educations",
                columns: new[] { "JobSeekerId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_UserId",
                table: "EmailVerifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_JobSeekerId_IsDeleted",
                table: "Experiences",
                columns: new[] { "JobSeekerId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_RecruiterId",
                table: "Jobs",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekers_CountryId",
                table: "JobSeekers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekers_FirstLanguageId",
                table: "JobSeekers",
                column: "FirstLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekers_JobTitleId",
                table: "JobSeekers",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekers_SecondLanguageId",
                table: "JobSeekers",
                column: "SecondLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekers_UserId",
                table: "JobSeekers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerSkills_JobSeekerId_SkillId",
                table: "JobSeekerSkills",
                columns: new[] { "JobSeekerId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerSkills_SkillId",
                table: "JobSeekerSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_JobId_SkillId",
                table: "JobSkills",
                columns: new[] { "JobId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_SkillId",
                table: "JobSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTitle_Title",
                table: "JobTitle",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Language_IsoCode",
                table: "Language",
                column: "IsoCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_UserId",
                table: "PasswordResets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_JobSeekerId_IsDeleted",
                table: "Projects",
                columns: new[] { "JobSeekerId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_JobId_JobSeekerId",
                table: "Recommendations",
                columns: new[] { "JobId", "JobSeekerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_JobSeekerId",
                table: "Recommendations",
                column: "JobSeekerId");

            migrationBuilder.CreateIndex(
                name: "IX_Recruiters_UserId",
                table: "Recruiters",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resume_JobSeekerId_Active_Unique",
                table: "Resumes",
                column: "JobSeekerId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Resume_JobSeekerId_IsDeleted",
                table: "Resumes",
                columns: new[] { "JobSeekerId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialAccounts_JobSeekerId",
                table: "SocialAccounts",
                column: "JobSeekerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentAnswer");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "Educations");

            migrationBuilder.DropTable(
                name: "EmailVerifications");

            migrationBuilder.DropTable(
                name: "Experiences");

            migrationBuilder.DropTable(
                name: "JobSeekerSkills");

            migrationBuilder.DropTable(
                name: "JobSkills");

            migrationBuilder.DropTable(
                name: "PasswordResets");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Recommendations");

            migrationBuilder.DropTable(
                name: "Resumes");

            migrationBuilder.DropTable(
                name: "SocialAccounts");

            migrationBuilder.DropTable(
                name: "AssessmentAttempt");

            migrationBuilder.DropTable(
                name: "AssessmentQuestion");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "JobSeekers");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Recruiters");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "JobTitle");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
