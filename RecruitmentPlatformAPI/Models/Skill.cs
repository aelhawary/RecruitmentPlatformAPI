using System.ComponentModel.DataAnnotations;

namespace RecruitmentPlatformAPI.Models
{
    public class Skill
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
