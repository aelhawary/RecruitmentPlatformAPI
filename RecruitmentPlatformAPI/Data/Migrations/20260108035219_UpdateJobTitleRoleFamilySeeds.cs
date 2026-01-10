using Microsoft.EntityFrameworkCore.Migrations;
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
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentPlatformAPI.Data.Migrations {
/// <inheritdoc />
    public partial class UpdateJobTitleRoleFamilySeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoleFamily",
                value: 2);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 2,
                column: "RoleFamily",
                value: 1);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 3,
                column: "RoleFamily",
                value: 3);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 4,
                column: "RoleFamily",
                value: 4);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 5,
                column: "RoleFamily",
                value: 4);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 6,
                column: "RoleFamily",
                value: 4);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 7,
                column: "RoleFamily",
                value: 6);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 8,
                column: "RoleFamily",
                value: 5);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 9,
                column: "RoleFamily",
                value: 5);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 10,
                column: "RoleFamily",
                value: 5);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 11,
                column: "RoleFamily",
                value: 5);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 12,
                column: "RoleFamily",
                value: 3);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 13,
                column: "RoleFamily",
                value: 7);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 14,
                column: "RoleFamily",
                value: 7);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 15,
                column: "RoleFamily",
                value: 6);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 16,
                column: "RoleFamily",
                value: 6);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 17,
                column: "RoleFamily",
                value: 6);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 18,
                column: "RoleFamily",
                value: 6);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 19,
                column: "RoleFamily",
                value: 6);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 20,
                column: "RoleFamily",
                value: 2);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 21,
                column: "RoleFamily",
                value: 3);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 22,
                column: "RoleFamily",
                value: 3);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 23,
                column: "RoleFamily",
                value: 6);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 26,
                column: "RoleFamily",
                value: 2);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 28,
                column: "RoleFamily",
                value: 5);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 29,
                column: "RoleFamily",
                value: 5);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 30,
                column: "RoleFamily",
                value: 5);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 31,
                column: "RoleFamily",
                value: 5);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 37,
                column: "RoleFamily",
                value: 3);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 41,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 42,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 43,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 44,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 45,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 46,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 47,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 48,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 49,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 50,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 51,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 52,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 53,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 54,
                column: "RoleFamily",
                value: 8);

            migrationBuilder.UpdateData(
                table: "JobTitle",
                keyColumn: "Id",
                keyValue: 55,
                column: "RoleFamily",
                value: 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                keyValue: 26,
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
                keyValue: 37,
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
        }
    }
}
