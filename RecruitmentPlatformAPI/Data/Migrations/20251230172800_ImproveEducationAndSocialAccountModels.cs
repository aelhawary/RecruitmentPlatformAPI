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
    public partial class ImproveEducationAndSocialAccountModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Educations_JobSeekerId",
                table: "Educations");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Educations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Educations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Educations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Educations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Major",
                table: "Educations",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Education_JobSeekerId_IsDeleted",
                table: "Educations",
                columns: new[] { "JobSeekerId", "IsDeleted" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Education_EndDateAfterStartDate",
                table: "Educations",
                sql: "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Education_JobSeekerId_IsDeleted",
                table: "Educations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Education_EndDateAfterStartDate",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "Major",
                table: "Educations");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Educations",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_JobSeekerId",
                table: "Educations",
                column: "JobSeekerId");
        }
    }
}
