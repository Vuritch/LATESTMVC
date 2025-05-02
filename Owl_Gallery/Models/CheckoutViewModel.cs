using System.Collections.Generic;
using System.Linq;
using Owl_Gallery.Models;

namespace Owl_Gallery.ViewModels
{
    public class CheckoutViewModel
    {
        public ShippingInfo Shipping { get; set; }
        public PaymentInfo Payment { get; set; }
        public List<CartItem> CartItems { get; set; } = new();

        public decimal Total => CartItems?.Sum(ci => ci.Product.Price * ci.Quantity) ?? 0m;
    }
}
