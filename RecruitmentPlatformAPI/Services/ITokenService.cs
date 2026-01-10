using RecruitmentPlatformAPI.Models;

namespace RecruitmentPlatformAPI.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        string GeneratePasswordResetToken(string email, int userId, int passwordResetId);
        (bool isValid, string email, int userId, int passwordResetId) ValidatePasswordResetToken(string token);
    }
}
