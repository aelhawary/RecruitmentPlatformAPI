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

namespace RecruitmentPlatformAPI.Enums {
/// <summary>
    /// Types of employment for work experience entries
    /// </summary>
    public enum EmploymentType
    {
        /// <summary>
        /// Full-time employment
        /// </summary>
        FullTime = 1,

        /// <summary>
        /// Part-time employment
        /// </summary>
        PartTime = 2,

        /// <summary>
        /// Freelance/Contract work
        /// </summary>
        Freelance = 3,

        /// <summary>
        /// Internship or training position
        /// </summary>
        Internship = 4
    }
}
