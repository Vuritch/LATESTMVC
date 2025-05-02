using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using Owl_Gallery.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace Owl_Gallery.Controllers.Account
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ApplicationDbContext context, ILogger<LoginController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Registers
                .SingleOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTime.UtcNow.AddDays(14) : (DateTime?)null
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties
            );

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // ← set a one-time message
            TempData["success"] = "You have been logged out successfully.";

            return RedirectToAction("Index", "Home");
        }

        //
        // ✨ External Login with Google
        //

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = "/")
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Login", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                TempData["Error"] = "Google authentication failed.";
                return RedirectToAction("Login");
            }

            var externalClaims = result.Principal.Claims.ToList();
            var email = externalClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = externalClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Google User";

            if (email == null)
            {
                TempData["Error"] = "Email not available from Google.";
                return RedirectToAction("Login");
            }

            // Check if user exists
            var user = await _context.Registers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                // Create a new user if not exists
                user = new Register
                {
                    FirstName = name.Split(' ').FirstOrDefault() ?? "Google",
                    LastName = name.Split(' ').Skip(1).FirstOrDefault() ?? "User",
                    Email = email,
                    Password = Guid.NewGuid().ToString() // random password for google users
                };
                _context.Registers.Add(user);
                await _context.SaveChangesAsync();
            }

            // Sign in the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
