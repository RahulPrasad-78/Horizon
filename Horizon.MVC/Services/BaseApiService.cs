using System.Net.Http.Headers;

namespace Horizon.MVC.Services
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _client = factory.CreateClient("StudentAPI");
            _httpContextAccessor = httpContextAccessor;
        }

        protected void AttachToken()
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return;

            var token = ctx.Session.GetString("jwt")
                ?? ctx.Request.Cookies["JwtToken"]
                ?? ctx.User.FindFirst("Token")?.Value;

            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        protected bool IsTokenPresent()
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return false;
            return !string.IsNullOrEmpty(ctx.Session.GetString("jwt"))
                || !string.IsNullOrEmpty(ctx.Request.Cookies["JwtToken"]);
        }
    }
}
