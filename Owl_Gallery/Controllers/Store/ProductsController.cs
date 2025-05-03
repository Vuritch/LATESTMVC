using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using System;
using System.Linq;

namespace Owl_Gallery.Controllers.Store
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public ProductsController(ApplicationDbContext ctx) => _ctx = ctx;

        /* --------------------------- CATALOG --------------------------- */
        public IActionResult Products(string category, string search, string sort, int page = 1)
        {
            const int pageSize = 9;
            IQueryable<Product> query = _ctx.Products;

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => EF.Functions.Like(p.Name, $"%{search}%"));

            query = sort switch
            {
                "newest" => query.OrderByDescending(p => p.Id),
                "priceAsc" => query.OrderBy(p => p.Price),
                "priceDesc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Id)
            };

            int count = query.Count();
            var products = query.Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            ViewBag.ActiveSales = _ctx.Sales.AsQueryable();
            ViewBag.Category = category;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);

            return View(products);
        }

        /* --------------------------- DETAILS --------------------------- */
        public IActionResult Details(int id)
        {
            var product = _ctx.Products.Find(id);
            if (product == null) return NotFound();

            ViewBag.ActiveSales = _ctx.Sales.AsQueryable();
            return View(product);
        }

        /* -------------------------- SALE PAGE -------------------------- */
        [HttpGet]                     //  ➜  /Products/Sale
        public IActionResult Sale(string filter, string search, string sort, int page = 1)
        {
            const int pageSize = 6;
            var query = _ctx.Sales.Include(s => s.Product).AsQueryable();

            // show only discounted items unless user clears filter
            if (string.Equals(filter, "sale", StringComparison.OrdinalIgnoreCase))
                query = query.Where(s => s.SalePrice.HasValue || s.DiscountPercentage > 0);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => EF.Functions.Like(s.Product.Name, $"%{search}%"));

            query = sort switch
            {
                "newest" => query.OrderByDescending(s => s.Product.Id),
                "priceAsc" => query.OrderBy(s => s.Product.Price),
                "priceDesc" => query.OrderByDescending(s => s.Product.Price),
                _ => query.OrderBy(s => s.Product.Id)
            };

            int count = query.Count();
            var salesData = query.Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            ViewBag.SignedIn = User.Identity.IsAuthenticated;
            ViewBag.Filter = filter;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);

            return View("Sale", salesData);   // expects Views/Products/Sale.cshtml
        }
    }
}
