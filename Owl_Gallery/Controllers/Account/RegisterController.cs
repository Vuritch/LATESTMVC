using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Owl_Gallery.ViewModels;
namespace Owl_Gallery.Controllers.Account
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(
            ApplicationDbContext context,
            ILogger<RegisterController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Register model)
        {
            if (!ModelState.IsValid)
                return View(model);

            /* ① duplicate-email check */
            if (_context.Registers.Any(r => r.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email),
                    "This e-mail address is already registered.");
                return View(model);                 // redisplay with validation error
            }

            _context.Registers.Add(model);
            _context.SaveChanges();
            TempData["Success"] = "Account created successfully. Please log in.";
            return RedirectToAction("Login", "Login");
        }
    }
}
