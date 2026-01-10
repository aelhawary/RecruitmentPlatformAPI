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
    /// Language proficiency levels for job seekers
    /// </summary>
    public enum LanguageProficiency
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3,
        Native = 4
    }
}
