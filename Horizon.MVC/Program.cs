using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Horizon.MVC.Handlers;
using Horizon.MVC.Services;

namespace Horizon.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            builder.Services.AddRazorPages();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".Horizon.Session";
            });

            builder.Services.AddTransient<BearerTokenHandler>();
            builder.Services.AddTransient<RetryHandler>();

            var gatewayUrl = builder.Configuration["GatewayUrl"] ?? "https://localhost:7000";

            // Auth API — base is gatewayUrl/auth/ so paths like api/Auth/login become /auth/api/Auth/login at gateway
            builder.Services.AddHttpClient<ApiService>(client =>
            {
                client.BaseAddress = new Uri($"{gatewayUrl}/auth/");
                client.Timeout = TimeSpan.FromSeconds(30);
            }).AddHttpMessageHandler<RetryHandler>();

            // Student API
            builder.Services.AddHttpClient("StudentAPI", client =>
            {
                client.BaseAddress = new Uri($"{gatewayUrl}/students/");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<BearerTokenHandler>()
            .AddHttpMessageHandler<RetryHandler>();

            // Course API
            builder.Services.AddHttpClient("LearningApi", client =>
            {
                client.BaseAddress = new Uri($"{gatewayUrl}/courses/");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<BearerTokenHandler>()
            .AddHttpMessageHandler<RetryHandler>();

            // Teacher API
            builder.Services.AddHttpClient("TeacherAPI", client =>
            {
                client.BaseAddress = new Uri($"{gatewayUrl}/teachers/");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<BearerTokenHandler>()
            .AddHttpMessageHandler<RetryHandler>();

            // Chat API
            builder.Services.AddHttpClient("ChatApi", client =>
            {
                client.BaseAddress = new Uri($"{gatewayUrl}/chat/");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<BearerTokenHandler>()
            .AddHttpMessageHandler<RetryHandler>();

            // Register Services
            builder.Services.AddScoped<BookmarkApiService>();
            builder.Services.AddScoped<DashboardApiService>();
            builder.Services.AddScoped<DiscoveryApiService>();
            builder.Services.AddScoped<EnrollmentApiService>();
            builder.Services.AddScoped<ProfileApiService>();
            builder.Services.AddScoped<ProgressApiService>();
            builder.Services.AddScoped<RecommendationApiService>();
            builder.Services.AddScoped<TeacherApiService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Auth/Login";
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
