using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using Owl_Gallery.Services;
using Owl_Gallery.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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

        [HttpGet]
        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cart = _ctx.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToList();

            var vm = new CheckoutViewModel
            {
                Shipping = new ShippingInfo(),
                Payment = new PaymentInfo(),
                CartItems = cart
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel vm)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userEmail = User.FindFirstValue(ClaimTypes.Email)!;

            if (!ModelState.IsValid)
            {
                vm.CartItems = _ctx.CartItems
                    .Include(ci => ci.Product)
                    .Where(ci => ci.UserId == userId)
                    .ToList();
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

                orderItems.Add(new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                });

                total += ci.Quantity * ci.Product.Price;
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
                Total = total,
                Items = orderItems
            };

            _ctx.Orders.Add(order);
            _ctx.CartItems.RemoveRange(cartItems);
            await _ctx.SaveChangesAsync();

            // ✅ Generate PDF as byte[]
            var pdfBytes = PdfReceiptGenerator.Generate(order, order.Items, userEmail);
            var attachmentName = $"order_{order.Id}_receipt.pdf";

            // ✅ Send email with in-memory PDF
            var emailBody = $"<h3>Thank you for your order #{order.Id}!</h3><p>Your receipt is attached.</p>";
            await _emailSender.SendEmailAsync(userEmail, "Owl Gallery - Order Confirmation", emailBody, pdfBytes, attachmentName);

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
