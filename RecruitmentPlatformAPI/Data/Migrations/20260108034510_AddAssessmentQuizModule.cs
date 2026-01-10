using System;
using RecruitmentPlatformAPI.DTOs;
using RecruitmentPlatformAPI.DTOs.Auth;
using RecruitmentPlatformAPI.DTOs.Profile;
using RecruitmentPlatformAPI.DTOs.Reference;
using RecruitmentPlatformAPI.Models.Core;
using RecruitmentPlatformAPI.Models.Reference;
using RecruitmentPlatformAPI.Models.Authentication;
using RecruitmentPlatformAPI.Models.Assessment;
using RecruitmentPlatformAPI.Services;
using RecruitmentPlatformAPI.Services.Interfaces;
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentPlatformAPI.Data.Migrations {
/// <inheritdoc />
    public partial class AddAssessmentQuizModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleFamily",
                table: "JobTitle",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssessmentJobTitleId",
                table: "JobSeekers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentAssessmentScore",
                table: "JobSeekers",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAssessmentDate",
                table: "JobSeekers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssessmentAttempt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobSeekerId = table.Column<int>(type: "int", nullable: false),
                    JobTitleId = table.Column<int>(type: "int", nullable: false),
                    OverallScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    TechnicalScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    SoftSkillsScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeLimitMinutes = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScoreExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "QuestionBank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: true),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    SeniorityLevel = table.Column<int>(type: "int", nullable: false),
                    Options = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CorrectAnswerIndex = table.Column<int>(type: "int", nullable: false),
                    TimePerQuestion = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBank_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentSkillScore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentAttemptId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    QuestionsAttempted = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentSkillScore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentSkillScore_AssessmentAttempt_AssessmentAttemptId",
                        column: x => x.AssessmentAttemptId,
                        principalTable: "AssessmentAttempt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentSkillScore_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentAttemptId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedAnswerIndex = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                        name: "FK_AssessmentAnswer_QuestionBank_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 2,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 3,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 4,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 5,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 6,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 7,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 8,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 9,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 10,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 11,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 12,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 13,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 14,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 15,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 16,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 17,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 18,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 19,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 20,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 21,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 22,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 23,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 24,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 25,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 26,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 27,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 28,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 29,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 30,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 31,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 32,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 33,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 34,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 35,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 36,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 37,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 38,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 39,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 40,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 41,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 42,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 43,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 44,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 45,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 46,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 47,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 48,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 49,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 50,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 51,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 52,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 53,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 54,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 55,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 56,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 57,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 58,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 59,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 60,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 61,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 62,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 63,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 64,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 65,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 66,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 67,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 68,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 69,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 70,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 71,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 72,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 73,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 74,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 75,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 76,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 77,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 78,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 79,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 80,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 81,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 82,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 83,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 84,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 85,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 86,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 87,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 88,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 89,
                column: "RoleFamily",
                value: 9);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 90,
                column: "RoleFamily",
                value: 9);

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
                name: "IX_AssessmentSkillScore_Attempt_Skill",
                table: "AssessmentSkillScore",
                columns: new[] { "AssessmentAttemptId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentSkillScore_SkillId",
                table: "AssessmentSkillScore",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_Filtering",
                table: "QuestionBank",
                columns: new[] { "Category", "Difficulty", "SeniorityLevel", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_SkillId",
                table: "QuestionBank",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentAnswer");

            migrationBuilder.DropTable(
                name: "AssessmentSkillScore");

            migrationBuilder.DropTable(
                name: "QuestionBank");

            migrationBuilder.DropTable(
                name: "AssessmentAttempt");

            migrationBuilder.DropColumn(
                name: "RoleFamily",
                table: "JobTitle");

            migrationBuilder.DropColumn(
                name: "AssessmentJobTitleId",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "CurrentAssessmentScore",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "LastAssessmentDate",
                table: "JobSeekers");
        }
    }
}
