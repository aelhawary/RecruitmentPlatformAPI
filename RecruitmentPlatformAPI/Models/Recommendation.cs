using System.ComponentModel.DataAnnotations;

namespace RecruitmentPlatformAPI.Models
{
    public class Recommendation
    {
        public int Id { get; set; }
        [Required]
        public int JobId { get; set; }
        [Required]
        public int JobSeekerId { get; set; }
        [Required]
        public decimal MatchScore { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Job Job { get; set; } = null!;
        public JobSeeker JobSeeker { get; set; } = null!;
    }
}
