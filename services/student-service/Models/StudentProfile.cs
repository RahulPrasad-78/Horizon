using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.StudentService.Models
{
    public class StudentProfile
    {
        [Key]
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";
        public string? Bio { get; set; }
        public List<string> Skills { get; set; } = new();
        public string PreferredLevel { get; set; } = "Beginner";

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    }
}
