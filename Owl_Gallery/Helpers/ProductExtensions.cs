using System;
using System.Linq;
using Owl_Gallery.Models;        // access Product & Sale entities

namespace Owl_Gallery.Helpers     // ← the namespace your view will use
{
    public static class ProductExtensions
    {
        /// <summary>
        /// Returns the shopper‑visible price, factoring in any active Sale.
        /// </summary>
        public static decimal GetCurrentPrice(this Product p, IQueryable<Sale> sales)
        {
            var now = DateTime.UtcNow;
            var sale = sales.FirstOrDefault(s =>
                        s.ProductId == p.Id &&
                        s.StartDate <= now &&
                        s.EndDate >= now);

            if (sale == null)
                return p.Price;

            return sale.SalePrice
                   ?? Math.Round(p.Price * (1 - sale.DiscountPercentage / 100m), 2);
        }
    }
}
