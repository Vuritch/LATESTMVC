using System.Collections.Generic;
using Owl_Gallery.Models;

namespace Owl_Gallery.ViewModels
{
    public class CheckoutViewModel
    {
        /* ---------- User‑entered data ---------- */
        public ShippingInfo Shipping { get; set; } = new();
        public PaymentInfo Payment { get; set; } = new();

        /* ---------- Cart items at checkout time ---------- */
        public IList<CartItem> CartItems { get; set; } = new List<CartItem>();

        /* ---------- Calculated grand total (sale‑aware) ---------- */
        public decimal Total { get; set; }          // <-- now has a setter
    }
}
