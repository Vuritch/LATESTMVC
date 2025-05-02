using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using Owl_Gallery.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;

namespace Owl_Gallery.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user1 = _context.Registers.FirstOrDefault(u => u.Id == userId);
            if (user1 != null && string.IsNullOrWhiteSpace(user1.Password))
            {
                return RedirectToAction("Index", "SetPassword", new { email = user1.Email });
            }

            var user = await _context.Registers.FindAsync(userId);

            if (user == null) return NotFound();

            var model = new ProfileUpdateViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return View(model);
        }

        // POST: /Profile/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProfileUpdateViewModel model)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user1 = _context.Registers.FirstOrDefault(u => u.Id == userId);
            if (user1 != null && string.IsNullOrWhiteSpace(user1.Password))
            {
                return RedirectToAction("Index", "SetPassword", new { email = user1.Email });
            }

            var user = await _context.Registers.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            // Check email uniqueness
            if (await _context.Registers.AnyAsync(u => u.Email == model.Email && u.Id != userId))
            {
                ModelState.AddModelError("Email", "This email is already in use.");
                ViewBag.OpenPassword = false;
                return View("Index", model);
            }

            // Password change logic
            bool passwordChangeAttempted = !string.IsNullOrEmpty(model.CurrentPassword)
                                         || !string.IsNullOrEmpty(model.NewPassword)
                                         || !string.IsNullOrEmpty(model.ConfirmPassword);

            if (passwordChangeAttempted)
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required.");
                    ViewBag.OpenPassword = true;
                    return View("Index", model);
                }

                if (model.CurrentPassword != user.Password)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    ViewBag.OpenPassword = true;
                    return View("Index", model);
                }

                if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 6)
                {
                    ModelState.AddModelError("NewPassword", "New password must be at least 6 characters long.");
                    ViewBag.OpenPassword = true;
                    return View("Index", model);
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "New password and confirmation do not match.");
                    ViewBag.OpenPassword = true;
                    return View("Index", model);
                }

                user.Password = model.NewPassword;
            }
            else
            {
                ViewBag.OpenPassword = false;
            }

            // Update basic info
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            await _context.SaveChangesAsync();

            // Refresh claims to reflect new name
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
                // Add other claims as needed
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Profile/Orders
        public async Task<IActionResult> Orders()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _context.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
            {
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });
            }

            var orders = await _context.Orders
                                       .Where(o => o.UserId == userId)
                                       .Include(o => o.Items)
                                       .ThenInclude(i => i.Product)
                                       .OrderByDescending(o => o.CreatedAt)
                                       .ToListAsync();

            return View(orders);
        }
    }
}
