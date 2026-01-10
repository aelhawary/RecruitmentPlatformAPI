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
    /// Job title role family groups for assessment score compatibility across job title changes
    /// </summary>
    public enum JobTitleRoleFamily
    {
        Frontend = 1,
        Backend = 2,
        FullStack = 3,
        Mobile = 4,
        Data = 5,
        DevOps = 6,
        QA = 7,
        Design = 8,
        Other = 9
    }
}
