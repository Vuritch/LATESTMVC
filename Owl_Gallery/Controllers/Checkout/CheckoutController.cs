using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Helpers;          // ← NEW
using Owl_Gallery.Models;
using Owl_Gallery.Services;
using Owl_Gallery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace Owl_Gallery.Controllers.Checkout
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IEmailSender _emailSender;

        public CheckoutController(ApplicationDbContext ctx, IEmailSender emailSender)
        {
            _ctx = ctx;
            _emailSender = emailSender;
        }

        /* ---------------------- CHECKOUT PAGE ---------------------- */
        [HttpGet]
        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var cart = _ctx.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToList();

            /* expose active sales for views */
            ViewBag.ActiveSales = _ctx.Sales.AsQueryable();                 // NEW

            var vm = new CheckoutViewModel
            {
                Shipping = new ShippingInfo(),
                Payment = new PaymentInfo(),
                CartItems = cart
            };

            /* ViewModel total should use sale price */
            vm.Total = cart.Sum(ci => ci.Quantity *
                ci.Product.GetCurrentPrice((IQueryable<Sale>)ViewBag.ActiveSales));  // NEW

            return View(vm);
        }

        /* ---------------------- PLACE ORDER ---------------------- */
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel vm)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            string email = User.FindFirstValue(ClaimTypes.Email)!;
            var salesQ = _ctx.Sales.AsQueryable();                       // NEW

            if (!ModelState.IsValid)
            {
                vm.CartItems = _ctx.CartItems
                    .Include(ci => ci.Product)
                    .Where(ci => ci.UserId == userId)
                    .ToList();

                ViewBag.ActiveSales = salesQ;                               // keep prices live
                vm.Total = vm.CartItems.Sum(ci => ci.Quantity *
                    ci.Product.GetCurrentPrice(salesQ));                    // NEW
                return View("Index", vm);
            }

            var cartItems = _ctx.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var ci in cartItems)
            {
                if (ci.Product.Quantity < ci.Quantity)
                {
                    TempData["Error"] = $"Only {ci.Product.Quantity} units of {ci.Product.Name} left.";
                    return RedirectToAction("Index", "Cart");
                }

                decimal unit = ci.Product.GetCurrentPrice(salesQ);          // NEW

                orderItems.Add(new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = unit                                        // NEW
                });

                total += ci.Quantity * unit;                                // NEW
                ci.Product.Quantity -= ci.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                FirstName = vm.Shipping.FirstName,
                LastName = vm.Shipping.LastName,
                Address1 = vm.Shipping.Address1,
                Address2 = vm.Shipping.Address2,
                City = vm.Shipping.City,
                State = vm.Shipping.State,
                PostalCode = vm.Shipping.PostalCode,
                Country = vm.Shipping.Country,
                CreatedAt = DateTime.UtcNow,
                Total = total,                                        // NEW (sale‑aware)
                Items = orderItems
            };

            _ctx.Orders.Add(order);
            _ctx.CartItems.RemoveRange(cartItems);
            await _ctx.SaveChangesAsync();

            /* PDF & email stay unchanged; they already use order.Total & UnitPrice */

            var pdfBytes = PdfReceiptGenerator.Generate(order, order.Items, email, salesQ);
            await _emailSender.SendEmailAsync(
                email,
                "Owl Gallery - Order Confirmation",
                $"<h3>Thank you for your order #{order.Id}!</h3><p>Your receipt is attached.</p>",
                pdfBytes,
                $"order_{order.Id}_receipt.pdf");

            return RedirectToAction("ThankYou", new { orderId = order.Id });
        }

        [HttpGet]
        public IActionResult ThankYou(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}
