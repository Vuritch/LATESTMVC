using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace Owl_Gallery.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public AdminController(ApplicationDbContext ctx) => _ctx = ctx;

        /* --------------------------- Dashboard --------------------------- */
        public IActionResult Index()
        {
            if (User.FindFirstValue(ClaimTypes.Email) != "admin@gmail.com")
                return Forbid();

            ViewBag.TotalProducts = _ctx.Products.Count();
            ViewBag.TotalUsers = _ctx.Registers.Count();
            ViewBag.TotalOrders = _ctx.Orders.Count();
            ViewBag.TotalSales = _ctx.Sales.Count();
            return View();
        }

        /* ---------------------- Product Management ---------------------- */
        public IActionResult ManageProducts(int page = 1, int pageSize = 10)
        {
            var total = _ctx.Products.Count();
            var list = _ctx.Products.OrderBy(p => p.Id)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToList();

            /* expose currently active sales for price display */
            var now = DateTime.UtcNow;
            ViewBag.ActiveSales = _ctx.Sales
                                      .Where(s => s.StartDate <= now && s.EndDate >= now)
                                      .AsQueryable();

            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
            ViewBag.CurrentPage = page;
            return View(list);
        }

        [HttpGet] public IActionResult CreateProduct() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateProduct(Product m)
        {
            if (!ModelState.IsValid) return View(m);

            _ctx.Products.Add(m);
            _ctx.SaveChanges();
            TempData["success"] = "Product created successfully!";
            return RedirectToAction(nameof(ManageProducts));
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var p = _ctx.Products.Find(id);
            return p == null ? NotFound() : View(p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditProduct(Product m)
        {
            if (!ModelState.IsValid) return View(m);

            var p = _ctx.Products.Find(m.Id);
            if (p == null) return NotFound();

            p.Name = m.Name;
            p.Category = m.Category;
            p.Price = m.Price;
            p.Quantity = m.Quantity;
            p.Description = m.Description;
            if (!string.IsNullOrWhiteSpace(m.ImageUrl))
                p.ImageUrl = m.ImageUrl;

            _ctx.SaveChanges();
            TempData["success"] = "Product updated successfully!";
            return RedirectToAction(nameof(ManageProducts));
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var p = _ctx.Products.Find(id);
            if (p == null) return NotFound();

            _ctx.Products.Remove(p);
            _ctx.SaveChanges();
            TempData["success"] = "Product deleted successfully!";
            return RedirectToAction(nameof(ManageProducts));
        }

        /* ------------------------ User Management ------------------------ */
        public IActionResult ManageUsers(int page = 1, int pageSize = 10)
        {
            var list = _ctx.Registers.OrderBy(u => u.Id)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)_ctx.Registers.Count() / pageSize);
            ViewBag.CurrentPage = page;
            return View(list);
        }

        [HttpGet] public IActionResult CreateUser() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateUser(Register m)
        {
            if (!ModelState.IsValid) return View(m);
            if (_ctx.Registers.Any(u => u.Email == m.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(m);
            }

            _ctx.Registers.Add(m);
            _ctx.SaveChanges();
            TempData["success"] = "User created successfully!";
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var u = _ctx.Registers.Find(id);
            if (u == null) return NotFound();

            _ctx.Registers.Remove(u);
            _ctx.SaveChanges();
            TempData["success"] = "User deleted successfully!";
            return RedirectToAction(nameof(ManageUsers));
        }

        /* ------------------------ Order Management ----------------------- */
        public IActionResult ManageOrders(int page = 1, int pageSize = 10)
        {
            var list = _ctx.Orders.OrderBy(o => o.Id)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)_ctx.Orders.Count() / pageSize);
            ViewBag.CurrentPage = page;
            return View(list);
        }

        [HttpPost]
        public IActionResult DeleteOrder(int id)
        {
            var o = _ctx.Orders.Find(id);
            if (o == null) return NotFound();

            _ctx.Orders.Remove(o);
            _ctx.SaveChanges();
            TempData["success"] = "Order deleted successfully!";
            return RedirectToAction(nameof(ManageOrders));
        }

        /* ------------------------- Sale Management ----------------------- */
        public IActionResult ManageSales(int page = 1, int pageSize = 10)
        {
            var list = _ctx.Sales.Include(s => s.Product)
                                 .OrderBy(s => s.SaleIdId)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)_ctx.Sales.Count() / pageSize);
            ViewBag.CurrentPage = page;
            return View(list);
        }

        /* ---------- Create Sale ---------- */
        [HttpGet]
        public IActionResult CreateSale()
        {
            ViewBag.Products = _ctx.Products.ToList();   // raw products (Id, Name, Price…)
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateSale(Sale s)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _ctx.Products.ToList();
                return View(s);
            }

            _ctx.Sales.Add(s);
            _ctx.SaveChanges();
            TempData["success"] = "Sale created successfully!";
            return RedirectToAction(nameof(ManageSales));
        }

        /* ---------- Edit Sale ------------ */
        [HttpGet]
        public IActionResult EditSale(int id)
        {
            var sale = _ctx.Sales.Include(s => s.Product)
                                 .FirstOrDefault(s => s.SaleIdId == id);
            if (sale == null) return NotFound();

            ViewBag.Products = _ctx.Products.ToList();
            return View(sale);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditSale(Sale m)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _ctx.Products.ToList();
                return View(m);
            }

            var s = _ctx.Sales.Find(m.SaleIdId);
            if (s == null) return NotFound();

            s.ProductId = m.ProductId;
            s.SalePrice = m.SalePrice;
            s.DiscountPercentage = m.DiscountPercentage;
            s.StartDate = m.StartDate;
            s.EndDate = m.EndDate;

            _ctx.SaveChanges();
            TempData["success"] = "Sale updated successfully!";
            return RedirectToAction(nameof(ManageSales));
        }

        /* ---------- Delete Sale ---------- */
        [HttpPost]
        public IActionResult DeleteSale(int id)
        {
            var s = _ctx.Sales.Find(id);
            if (s == null) return NotFound();

            _ctx.Sales.Remove(s);
            _ctx.SaveChanges();
            TempData["success"] = "Sale deleted successfully!";
            return RedirectToAction(nameof(ManageSales));
        }
    }
}
