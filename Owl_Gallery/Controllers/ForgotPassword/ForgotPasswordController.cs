using Microsoft.AspNetCore.Mvc;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using Owl_Gallery.Services;
using Owl_Gallery.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Owl_Gallery.Controllers.ForgotPassword
{
    public class ForgotPasswordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Registers.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                TempData["Error"] = "No account found with this email.";
                return View(model);
            }

            // Generate 6-digit code
            var code = new Random().Next(100000, 999999).ToString();
            user.ResetCode = code;
            user.CodeSentAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send email
            var body = $"Your password reset code is: <strong>{code}</strong>";
            await _emailSender.SendEmailAsync(user.Email, "Reset Your Password", body);

            TempData["Success"] = "Reset code sent! Please check your email.";
            return RedirectToAction("Verify", "ForgotPassword", new { email = user.Email });
        }
        [HttpGet]
        public IActionResult Verify(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Email is missing.";
                return RedirectToAction("Index");
            }

            var vm = new VerifyCodeViewModel { Email = email };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Verify(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Registers.FirstOrDefault(u => u.Email == model.Email);

            if (user == null || user.ResetCode != model.Code || !user.CodeSentAt.HasValue || user.CodeSentAt.Value.AddMinutes(10) < DateTime.UtcNow)
            {
                TempData["Error"] = "Invalid or expired reset code.";
                return View(model);
            }


            return RedirectToAction("Reset", "ForgotPassword", new { email = user.Email });
        }

        [HttpGet]
        public IActionResult Reset(string email)
        {
            return View(new SetPasswordViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> Reset(SetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = _context.Registers.FirstOrDefault(u => u.Email == vm.Email);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return View(vm);
            }

            user.Password = vm.Password;
            user.PasswordSet = true;
            user.ResetCode = null;
            user.CodeSentAt = null;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Password reset successfully!";
            return RedirectToAction("Login", "Login");
        }
    }
}
