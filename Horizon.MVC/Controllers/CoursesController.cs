using Horizon.MVC.DTOs;
using Horizon.MVC.Models;
using Horizon.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Horizon.MVC.Controllers
{
    public class CoursesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly EnrollmentApiService _enrollmentService;
        private readonly TeacherApiService _teacherService;

        public CoursesController(
            IHttpClientFactory httpClientFactory,
            EnrollmentApiService enrollmentService,
            TeacherApiService teacherService)
        {
            _httpClient = httpClientFactory.CreateClient("LearningApi");
            _enrollmentService = enrollmentService;
            _teacherService = teacherService;
        }

        // ── Student-facing ───────────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/CoursesApi");
                if (response.IsSuccessStatusCode)
                {
                    var wrapped = await response.Content.ReadFromJsonAsync<ApiResponse<PagedCourseResponse>>();
                    return View(wrapped?.Data?.Data ?? new List<CourseReadDTO>());
                }
            }
            catch { }
            return View(new List<CourseReadDTO>());
        }

        public async Task<IActionResult> Details(int id)
        {
            CourseReadDTO? course = null;
            try
            {
                var wrapped = await _httpClient.GetFromJsonAsync<ApiResponse<CourseReadDTO>>($"api/CoursesApi/{id}");
                course = wrapped?.Data;
            }
            catch { return NotFound(); }

            if (course == null) return NotFound();

            ViewBag.IsEnrolled = false;
            if (User.Identity?.IsAuthenticated == true)
                ViewBag.IsEnrolled = await _enrollmentService.IsEnrolledAsync(id);

            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<List<CommentResponseDto>>>($"api/CommentsApi/Course/{id}");
                ViewBag.Comments = apiResponse?.Data ?? new List<CommentResponseDto>();
            }
            catch { ViewBag.Comments = new List<CommentResponseDto>(); }

            return View(course);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            try
            {
                await _enrollmentService.EnrollAsync(courseId);
                TempData["Success"] = "Successfully enrolled!";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int courseId, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var dto = new CreateCommentDto { CourseId = courseId, Content = content };
                await _httpClient.PostAsJsonAsync("api/CommentsApi", dto);
            }
            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        // ── Teacher-facing (via teacher service) ─────────────────────────────

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherDashboard()
        {
            var dashboard = await _teacherService.GetDashboardAsync();
            return View(dashboard);
        }

        [Authorize(Roles = "Teacher")]
        public IActionResult Create() => View(new TeacherCourseWriteDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(TeacherCourseWriteDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var (success, error, _) = await _teacherService.CreateCourseAsync(dto);
            if (success) return RedirectToAction(nameof(TeacherDashboard));
            ModelState.AddModelError("", error ?? "Failed to create course.");
            return View(dto);
        }

        [Authorize(Roles = "Teacher")]
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
            ViewBag.Id = course.Id;
            ViewBag.Status = course.Status;
            ViewBag.CreatedAt = course.CreatedAt;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, TeacherCourseWriteDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var (success, error) = await _teacherService.UpdateCourseAsync(id, dto);
            if (success) return RedirectToAction(nameof(TeacherDashboard));
            ModelState.AddModelError("", error ?? "Failed to update course.");
            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Publish(int id)
        {
            await _teacherService.PublishCourseAsync(id);
            return RedirectToAction(nameof(TeacherDashboard));
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddVideo(int courseId, string videoUrl)
        {
            if (!string.IsNullOrEmpty(videoUrl))
                await _teacherService.AddVideoAsync(courseId, videoUrl);
            return RedirectToAction(nameof(Details), new { id = courseId });
        }
    }

    internal class PagedCourseResponse
    {
        public List<CourseReadDTO>? Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
