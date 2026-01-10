using RecruitmentPlatformAPI.Models.Core;

namespace RecruitmentPlatformAPI.Services.Interfaces
{
public interface ITokenService
    {
        string GenerateJwtToken(User user);
        string GeneratePasswordResetToken(string email, int userId, int passwordResetId);
        (bool isValid, string email, int userId, int passwordResetId) ValidatePasswordResetToken(string token);
    }
}
