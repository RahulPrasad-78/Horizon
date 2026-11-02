using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace Horizon.MVC.Models
{
    public class CourseVideo
    {
        public int Id { get; set; }

        public string VideoUrl { get; set; } = string.Empty;

        public int CourseId { get; set; }
        [JsonIgnore]
        [ValidateNever]
        public Course? Course { get; set; }
    }
}

