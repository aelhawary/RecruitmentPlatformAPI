using RecruitmentPlatformAPI.DTOs.JobSeeker;
using RecruitmentPlatformAPI.DTOs.Recruiter;

namespace RecruitmentPlatformAPI.Services.Recruiter
{
    public interface IRecruiterService
    {
        Task<ProfileResponseDto> SaveCompanyInfoAsync(int userId, RecruiterCompanyInfoRequestDto dto);
        Task<RecruiterCompanyInfoDto?> GetCompanyInfoAsync(int userId);
        Task<WizardStatusDto> GetWizardStatusAsync(int userId);
        List<IndustryDto> GetIndustries();
        List<CompanySizeDto> GetCompanySizes();
    }
}
