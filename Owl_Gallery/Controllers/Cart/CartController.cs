using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace Owl_Gallery.Controllers.Cart
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public CartController(ApplicationDbContext ctx) => _ctx = ctx;

        /* ----------------- CART PAGE ----------------- */
        [HttpGet]
        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });

            var items = _ctx.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)                 // ensure Product is loaded
                .ToList();

            ViewBag.ActiveSales = _ctx.Sales.AsQueryable();        

            return View(items);
        }

        /* ----------------- ADD ----------------- */
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(int productId, int quantity = 1)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });

            var product = _ctx.Products.Find(productId);
            if (product == null || product.Quantity == 0)
            {
                TempData["Error"] = "❌ This product is out of stock.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            quantity = Math.Min(quantity, product.Quantity);   // clamp to stock

            var existing = _ctx.CartItems.SingleOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (existing != null)
            {
                existing.Quantity = Math.Min(existing.Quantity + quantity, product.Quantity);
            }
            else
            {
                _ctx.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    UserName = User.FindFirstValue(ClaimTypes.Name)!,
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity
                });
            }

            _ctx.SaveChanges();
            TempData["Success"] = $"✅ Added {quantity}× “{product.Name}” to your cart!";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        /* ----------------- UPDATE QTY ----------------- */
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });

            var item = _ctx.CartItems.SingleOrDefault(c => c.UserId == userId && c.ProductId == productId);
            var product = _ctx.Products.Find(productId);

            if (item == null || product == null) return RedirectToAction(nameof(Index));

            if (quantity <= 0)
            {
                _ctx.CartItems.Remove(item);
            }
            else if (quantity > product.Quantity)
            {
                TempData["Error"] = $"Only {product.Quantity} left in stock.";
            }
            else
            {
                item.Quantity = quantity;
            }

            _ctx.SaveChanges();
            TempData["Success"] = "♻️ Cart updated.";
            return RedirectToAction(nameof(Index));
        }

        /* ----------------- REMOVE ----------------- */
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });

            var item = _ctx.CartItems.SingleOrDefault(c => c.UserId == userId && c.ProductId == productId);
            if (item != null)
            {
                _ctx.CartItems.Remove(item);
                _ctx.SaveChanges();
            }

            TempData["Success"] = "🗑️ Removed from cart.";
            return RedirectToAction(nameof(Index));
        }
    }
}
