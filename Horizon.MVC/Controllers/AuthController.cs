using Horizon.MVC.DTOs;
using Horizon.MVC.Models;
using Horizon.MVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Horizon.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["HideSidebar"] = true;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["HideSidebar"] = true;
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.LoginAsync(model);

            if (result.Success)
            {
                // Store JWT for API calls
                HttpContext.Session.SetString("jwt", result.Token);
                HttpContext.Session.SetString("name", result.Name ?? model.Email);
                HttpContext.Session.SetString("email", model.Email);

                Response.Cookies.Append("JwtToken", result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddHours(2),
                    SameSite = SameSiteMode.Lax
                });

                // Cookie auth for MVC [Authorize]
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.Name ?? model.Email),
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.NameIdentifier, result.UserId),
                    new Claim("Token", result.Token)
                };

                var roles = result.Roles.Length > 0 ? result.Roles : new[] { "Student" };
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties { IsPersistent = true });

                if (roles.Contains("Admin"))
                    return RedirectToAction("Dashboard", "Admin");

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", result.Message ?? "Invalid login attempt");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["HideSidebar"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["HideSidebar"] = true;
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.RegisterAsync(model);

            if (result.Success)
            {
                ViewData["Message"] = "Your account has been created successfully!";
                return View("LoginSuccess");
            }

            ModelState.AddModelError("", result.Message ?? "Registration failed");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            Response.Cookies.Delete("JwtToken");
            return RedirectToAction("Login");
        }
    }
}
