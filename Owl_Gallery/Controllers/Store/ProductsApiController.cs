using Microsoft.AspNetCore.Mvc;
using Owl_Gallery.Data;
using Owl_Gallery.Helpers;    // ⬅ price helper
using System.Linq;

namespace Owl_Gallery.Controllers.Store
{
    [Route("api/products")]
    public class ProductsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        public ProductsApiController(ApplicationDbContext ctx) => _ctx = ctx;

        // GET /api/products/all
        [HttpGet("all")]
        public IActionResult All()
        {
            var sales = _ctx.Sales.AsQueryable();
            var data = _ctx.Products.ToList().Select(p => new
            {
                id = p.Id,
                name = p.Name,
                cat = p.Category,
                image = p.ImageUrl,
                price = p.GetCurrentPrice(sales)   // ⬅ returns live sale price
            });

            return Ok(data);
        }
    }
}
