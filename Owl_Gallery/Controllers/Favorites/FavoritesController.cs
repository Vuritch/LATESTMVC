using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Models;

namespace Owl_Gallery.Controllers.Favorites
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public FavoritesController(ApplicationDbContext ctx) => _ctx = ctx;

        /* ------------------------ LIST ------------------------ */
        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });

            var favIds = _ctx.Favorites
                             .Where(f => f.UserId == userId)
                             .Select(f => f.ProductId)
                             .ToList();

            var products = _ctx.Products
                               .Where(p => favIds.Contains(p.Id))
                               .ToList();

            /* for price helper & heart‑state in view */
            ViewBag.ActiveSales = _ctx.Sales.AsQueryable();   // NEW
            ViewBag.Favorites = favIds;                    // NEW

            return View(products);                            // expects Views/Favorites/Index.cshtml
        }

        /* ------------------------ ADD ------------------------ */
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(int productId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });

            if (!_ctx.Favorites.Any(f => f.UserId == userId && f.ProductId == productId))
            {
                _ctx.Favorites.Add(new Favorite { UserId = userId, ProductId = productId });
                _ctx.SaveChanges();
                TempData["success"] = "❤️ Added to favorites!";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        /* ------------------------ REMOVE ------------------------ */
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });

            var fav = _ctx.Favorites.SingleOrDefault(f => f.UserId == userId && f.ProductId == productId);
            if (fav != null)
            {
                _ctx.Favorites.Remove(fav);
                _ctx.SaveChanges();
                TempData["success"] = "💔 Removed from favorites.";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
