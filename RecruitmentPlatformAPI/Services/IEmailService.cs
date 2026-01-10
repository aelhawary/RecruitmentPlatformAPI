namespace RecruitmentPlatformAPI.Services
{
    public interface IEmailService
    {
        Task<bool> SendVerificationEmailAsync(string email, string firstName, string verificationCode);
        Task<bool> SendWelcomeEmailAsync(string email, string firstName);
        Task<bool> SendPasswordResetOtpAsync(string email, string firstName, string otpCode);
        Task<bool> SendAccountLockedEmailAsync(string email, string firstName, DateTime lockoutEnd);
        string GenerateVerificationCode();
    }
}
