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
    /// Seniority level based on years of experience (for question targeting and job seeker classification)
    /// </summary>
    public enum ExperienceSeniorityLevel
    {
        Junior = 1,    // 0-2 years
        Mid = 2,       // 3-5 years
        Senior = 3     // 6+ years
    }
}
