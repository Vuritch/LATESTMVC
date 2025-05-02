using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using System.Linq;
using System.Collections.Generic;

namespace Owl_Gallery.Controllers.Store
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public ProductsController(ApplicationDbContext ctx) => _ctx = ctx;

        // GET: /Products/Products?category=Rings&search=...
        public IActionResult Products(string category, string search, string sort, int page = 1)
        {
            int pageSize = 6; // Number of products per page
            var query = _ctx.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            switch (sort)
            {
                case "newest":
                    query = query.OrderByDescending(p => p.Id);
                    break;
                case "priceAsc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "priceDesc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                default:
                    query = query.OrderBy(p => p.Id);
                    break;
            }

            var count = query.Count();
            var products = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Category = category;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);

            return View(products);
        }
        // GET: /Products/Details/5
        public IActionResult Details(int id)
        {
            var product = _ctx.Products.Find(id);
            return product == null ? NotFound() : View(product);
        }

        // GET: /Products/Sale?filter=sale&search=...&sort=...
        [HttpGet]
        [Route("Products/Sale")]
        public IActionResult Sale(string filter, string search, string sort, int page = 1)
        {
            int pageSize = 6;

            var query = _ctx.Sales.Include(s => s.Product).AsQueryable();

            if (!string.IsNullOrEmpty(filter) && filter.ToLower() == "sale")
            {
                query = query.Where(s => s.SalePrice.HasValue);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Product.Name.Contains(search));
            }

            switch (sort)
            {
                case "newest":
                    query = query.OrderByDescending(s => s.Product.Id);
                    break;
                case "priceAsc":
                    query = query.OrderBy(s => s.Product.Price);
                    break;
                case "priceDesc":
                    query = query.OrderByDescending(s => s.Product.Price);
                    break;
                default:
                    query = query.OrderBy(s => s.Product.Id);
                    break;
            }

            var count = query.Count();

            var salesData = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            //Pass signed-in status to the view
            ViewBag.SignedIn = User.Identity.IsAuthenticated;

            ViewBag.Filter = filter;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);

            return View(salesData);
        }

        // GET: /Products/AddSale
        [HttpGet]
        public IActionResult AddSale()
        {
            // Get all products for the dropdown
            ViewBag.Products = _ctx.Products.ToList();
            return View();
        }

        // POST: /Products/AddSale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSale(Sale sale)
        {
            if (ModelState.IsValid)
            {
                _ctx.Sales.Add(sale);
                _ctx.SaveChanges();
                TempData["success"] = "Sale added successfully!";
                return RedirectToAction("Sale");
            }

            ViewBag.Products = _ctx.Products.ToList(); // In case of error, return products to the view
            return View(sale);
        }

        // GET: /Products/EditSale/5
        [HttpGet]
        public IActionResult EditSale(int id)
        {
            var sale = _ctx.Sales.Include(s => s.Product).FirstOrDefault(s => s.SaleIdId == id);
            if (sale == null)
                return NotFound();

            ViewBag.Products = _ctx.Products.ToList();
            return View(sale);
        }

        // POST: /Products/EditSale/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSale(Sale updatedSale)
        {
            if (ModelState.IsValid)
            {
                var saleInDb = _ctx.Sales.FirstOrDefault(s => s.SaleIdId == updatedSale.SaleIdId);
                if (saleInDb == null)
                    return NotFound();

                saleInDb.ProductId = updatedSale.ProductId;
                saleInDb.SalePrice = updatedSale.SalePrice;
                saleInDb.DiscountPercentage = updatedSale.DiscountPercentage;
                saleInDb.StartDate = updatedSale.StartDate;
                saleInDb.EndDate = updatedSale.EndDate;

                _ctx.SaveChanges();
                TempData["success"] = "Sale updated successfully!";
                return RedirectToAction("Sale");
            }

            ViewBag.Products = _ctx.Products.ToList();
            return View(updatedSale);
        }

        // POST: /Products/DeleteSale/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSale(int id)
        {
            var sale = _ctx.Sales.Find(id);
            if (sale == null)
                return NotFound();

            _ctx.Sales.Remove(sale);
            _ctx.SaveChanges();
            TempData["success"] = "Sale deleted successfully!";
            return RedirectToAction("Sale");
        }

    }
}
