using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using System.Security.Claims;

namespace Owl_Gallery.Controllers.Cart
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public CartController(ApplicationDbContext ctx) => _ctx = ctx;

        // GET /Cart
        [HttpGet]
        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
            {
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });
            }

            var items = _ctx.CartItems
                .Where(c => c.UserId == userId)
                .Select(c => new CartItem
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    Quantity = c.Quantity,
                    Product = c.Product
                })
                .ToList();

            return View(items);
        }

        // POST /Cart/Add
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(int productId, int quantity = 1)
        {

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var product = _ctx.Products.Find(productId);
            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
            {
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });
            }


            if (product == null || product.Quantity == 0)
            {
                TempData["Error"] = "❌ This product is out of stock.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            // Clamp the requested quantity to what's available
            quantity = Math.Min(quantity, product.Quantity);

            var existing = _ctx.CartItems
                .SingleOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (existing != null)
            {
                // Increase existing line item, but not beyond stock
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


        // POST /Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
            {
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });
            }

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

        // POST /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _ctx.Registers.FirstOrDefault(u => u.Id == userId);
            if (user != null && string.IsNullOrWhiteSpace(user.Password))
            {
                return RedirectToAction("Index", "SetPassword", new { email = user.Email });
            }

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
