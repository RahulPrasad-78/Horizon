using Horizon.MVC.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Horizon.MVC.Services
{
    public class TeacherApiService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

        public TeacherApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _client = factory.CreateClient("TeacherAPI");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AttachToken()
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return;
            var token = ctx.Session.GetString("jwt")
                ?? ctx.Request.Cookies["JwtToken"]
                ?? ctx.User.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Unwrap ResponseWrapperFilter envelope { success, message, data }
        private static async Task<T?> UnwrapAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (root.TryGetProperty("data", out var dataEl) || root.TryGetProperty("Data", out dataEl))
                return JsonSerializer.Deserialize<T>(dataEl.GetRawText(), _jsonOpts);
            return JsonSerializer.Deserialize<T>(json, _jsonOpts);
        }

        public async Task<TeacherDashboardDto?> GetDashboardAsync()
        {
            try
            {
                AttachToken();
                var response = await _client.GetAsync("api/teacher/dashboard");
                if (!response.IsSuccessStatusCode) return null;
                return await UnwrapAsync<TeacherDashboardDto>(response);
            }
            catch { return null; }
        }

        public async Task<List<TeacherCourseReadDto>> GetMyCoursesAsync()
        {
            try
            {
                AttachToken();
                var response = await _client.GetAsync("api/teacher/courses");
                if (!response.IsSuccessStatusCode) return new();
                return await UnwrapAsync<List<TeacherCourseReadDto>>(response) ?? new();
            }
            catch { return new(); }
        }

        public async Task<TeacherCourseReadDto?> GetCourseAsync(int id)
        {
            try
            {
                AttachToken();
                var response = await _client.GetAsync($"api/teacher/courses/{id}");
                if (!response.IsSuccessStatusCode) return null;
                return await UnwrapAsync<TeacherCourseReadDto>(response);
            }
            catch { return null; }
        }

        public async Task<(bool Success, string? Error, TeacherCourseReadDto? Data)> CreateCourseAsync(TeacherCourseWriteDto dto)
        {
            try
            {
                AttachToken();
                var response = await _client.PostAsJsonAsync("api/teacher/courses", dto);
                if (response.IsSuccessStatusCode)
                    return (true, null, await UnwrapAsync<TeacherCourseReadDto>(response));
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    return (false, "A course with this title already exists.", null);
                return (false, "Failed to create course.", null);
            }
            catch (Exception ex) { return (false, ex.Message, null); }
        }

        public async Task<(bool Success, string? Error)> UpdateCourseAsync(int id, TeacherCourseWriteDto dto)
        {
            try
            {
                AttachToken();
                var response = await _client.PutAsJsonAsync($"api/teacher/courses/{id}", dto);
                return response.IsSuccessStatusCode
                    ? (true, null)
                    : (false, "Failed to update course.");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public async Task<(bool Success, string? Error)> PublishCourseAsync(int id)
        {
            try
            {
                AttachToken();
                var response = await _client.PostAsync($"api/teacher/courses/{id}/publish", null);
                return response.IsSuccessStatusCode ? (true, null) : (false, "Failed to publish course.");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public async Task<(bool Success, string? Error)> AddVideoAsync(int id, string videoUrl)
        {
            try
            {
                AttachToken();
                var response = await _client.PostAsJsonAsync($"api/teacher/courses/{id}/videos", videoUrl);
                return response.IsSuccessStatusCode ? (true, null) : (false, "Failed to add video.");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }
    }
}
