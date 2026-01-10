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
    /// Authentication provider used for user registration and login
    /// </summary>
    public enum AuthProvider
    {
        /// <summary>
        /// Email/Password authentication
        /// </summary>
        Email = 1,
        
        /// <summary>
        /// Google OAuth authentication
        /// </summary>
        Google = 2
    }
}
