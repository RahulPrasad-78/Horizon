namespace Horizon.MVC.DTOs
{
    public class TeacherCourseReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string InstructorId { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime StartDate { get; set; }
        public List<TeacherCourseVideoDto> Videos { get; set; } = new();
    }

    public class TeacherCourseVideoDto
    {
        public int Id { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
    }

    public class TeacherCourseWriteDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsPublished { get; set; }
    }

    public class TeacherDashboardDto
    {
        public string TeacherId { get; set; } = string.Empty;
        public int TotalCourses { get; set; }
        public int PublishedCourses { get; set; }
        public int DraftCourses { get; set; }
        public int TotalStudents { get; set; }
        public decimal TotalEarnings { get; set; }
        public TeacherCourseReadDto? BestSellingCourse { get; set; }
        public int BestSellingCourseStudentCount { get; set; }
        public List<TeacherCourseMetricsDto> CourseMetrics { get; set; } = new();
    }

    public class TeacherCourseMetricsDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public decimal Price { get; set; }
        public decimal Earnings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int UnreadMessages { get; set; }
    }
}
