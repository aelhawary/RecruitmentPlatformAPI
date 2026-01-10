using System.ComponentModel.DataAnnotations;
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentPlatformAPI.Models.Authentication {
public class PasswordReset
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string OtpCode { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public bool IsUsed { get; set; }
    }
}
