using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Owl_Gallery.ViewModels;


namespace Owl_Gallery.Controllers.Store
{
    public class ProductsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public ProductsApiController(ApplicationDbContext ctx) => _ctx = ctx;

        // GET /api/products/all
        [HttpGet("all")]
        public IActionResult All()
        {
            var data = _ctx.Products.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                cat = p.Category,
                image = p.ImageUrl,
                price = p.Price
            }).ToList();

            return Ok(data);
        }
    }
}
