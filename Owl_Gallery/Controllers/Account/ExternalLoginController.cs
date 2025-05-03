using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using System.Security.Claims;

namespace Owl_Gallery.Controllers.Account
{
    [AllowAnonymous]
    public class ExternalLoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExternalLoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return RedirectToAction("Login", "Login");

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims
                .ToDictionary(c => c.Type, c => c.Value);

            if (!claims.TryGetValue(ClaimTypes.Email, out var email) || string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Login", "Login");

            var fullName = claims.ContainsKey(ClaimTypes.Name) ? claims[ClaimTypes.Name] : "Google User";
            var firstName = fullName.Split(' ').FirstOrDefault() ?? "Google";
            var lastName = fullName.Split(' ').Skip(1).FirstOrDefault() ?? "User";

            var user = _context.Registers.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new Register
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Password = "", // Enforce password setup later
                    PasswordSet = false
                };

                _context.Registers.Add(user);
                await _context.SaveChangesAsync();
            }

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
