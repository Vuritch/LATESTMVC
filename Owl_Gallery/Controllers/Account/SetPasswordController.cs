using Microsoft.AspNetCore.Mvc;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using Owl_Gallery.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Owl_Gallery.Controllers
{
    public class SetPasswordController : Controller
    {
        private readonly ApplicationDbContext _ctx;

        public SetPasswordController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public IActionResult Index(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Login", "Login");

            var vm = new SetPasswordViewModel { Email = email };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = _ctx.Registers.FirstOrDefault(u => u.Email == vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(vm);
            }

            user.Password = vm.Password; // Ideally hashed
            user.PasswordSet = true;
            await _ctx.SaveChangesAsync();

            TempData["Success"] = "Your password has been set successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}
