using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Owl_Gallery.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Include(s => s.Product)
                .Where(s => s.SalePrice.HasValue)
                .ToListAsync();

            return View("Products/Sale", sales);  
        }

    }
}
