using Horizon.MVC.DTOs;
using Horizon.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.MVC.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly TeacherApiService _teacherService;

        public TeacherController(TeacherApiService teacherService)
        {
            _teacherService = teacherService;
        }

        // GET /Teacher  →  Teacher Dashboard with metrics
        public async Task<IActionResult> Index()
        {
            var dashboard = await _teacherService.GetDashboardAsync();
            return View(dashboard);
        }

        // GET /Teacher/Courses  →  Full course list
        public async Task<IActionResult> Courses()
        {
            var courses = await _teacherService.GetMyCoursesAsync();
            return View(courses);
        }

        // GET /Teacher/Create
        public IActionResult Create() => View(new TeacherCourseWriteDto { StartDate = DateTime.Today });

        // POST /Teacher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherCourseWriteDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var (success, error, _) = await _teacherService.CreateCourseAsync(dto);
            if (success)
            {
                TempData["Success"] = "Course created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", error ?? "Failed to create course.");
            return View(dto);
        }

        // GET /Teacher/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _teacherService.GetCourseAsync(id);
            if (course == null) return NotFound();

            var dto = new TeacherCourseWriteDto
            {
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Category = course.Category,
                Duration = course.Duration,
                ThumbnailUrl = course.ThumbnailUrl,
                StartDate = course.StartDate,
                IsPublished = course.IsPublished
            };
            ViewBag.CourseId = course.Id;
            ViewBag.CourseTitle = course.Title;
            ViewBag.Status = course.Status;
            ViewBag.CreatedAt = course.CreatedAt;
            ViewBag.Videos = course.Videos;
            return View(dto);
        }

        // POST /Teacher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherCourseWriteDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CourseId = id;
                return View(dto);
            }
            var (success, error) = await _teacherService.UpdateCourseAsync(id, dto);
            if (success)
            {
                TempData["Success"] = "Course updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", error ?? "Failed to update course.");
            ViewBag.CourseId = id;
            return View(dto);
        }

        // POST /Teacher/Publish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            var (success, error) = await _teacherService.PublishCourseAsync(id);
            TempData[success ? "Success" : "Error"] = success ? "Course published!" : (error ?? "Failed to publish course.");
            return RedirectToAction(nameof(Index));
        }

        // POST /Teacher/AddVideo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVideo(int courseId, string videoUrl)
        {
            if (!string.IsNullOrEmpty(videoUrl))
            {
                var (success, error) = await _teacherService.AddVideoAsync(courseId, videoUrl);
                TempData[success ? "Success" : "Error"] = success ? "Video added!" : (error ?? "Failed to add video.");
            }
            return RedirectToAction(nameof(Edit), new { id = courseId });
        }
    }
}
