using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using Owl_Gallery.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace Owl_Gallery.Controllers.Home
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _ctx;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            // Get all products (optional, if needed for other purposes)
            var allProducts = _ctx.Products.ToList();

            // Get last 3 added products as trending
            var trendingProducts = _ctx.Products
                .OrderByDescending(p => p.Id)
                .Take(3)
                .ToList();

            // Get 3 products with the least quantity as best sellers
            var bestSellers = _ctx.Products
                .OrderBy(p => p.Quantity)
                .Take(3)
                .ToList();

            // Pass data to the view
            var viewModel = new HomeViewModel
            {
                Products = allProducts,
                TrendingProducts = trendingProducts,
                BestSellers = bestSellers
            };

            return View("~/Views/Index/Index.cshtml", viewModel);

        }
    }

    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
