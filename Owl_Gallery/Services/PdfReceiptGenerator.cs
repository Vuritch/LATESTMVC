using Owl_Gallery.Helpers;                 // ← to use GetCurrentPrice
using Owl_Gallery.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Owl_Gallery.Services
{
    public static class PdfReceiptGenerator
    {
        /// <summary>
        /// Generates a nice looking receipt that is aware of any active sales.
        /// </summary>
        /// <param name="order">Order header</param>
        /// <param name="items">Order items (Product navigation property must be included!)</param>
        /// <param name="userEmail">Customer email</param>
        /// <param name="activeSales">IQueryable of active Sale rows</param>
        public static byte[] Generate(
            Order order,
            List<OrderItem> items,
            string userEmail,
            IQueryable<Sale>? activeSales = null)
        {
            // ───── logo ─────
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
            var logoBase64 = Convert.ToBase64String(File.ReadAllBytes(logoPath));

            // ───── HTML build ─────
            var html = new StringBuilder(@$"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding:40px; background:#f9f9ff; }}
        h1   {{ color:#5d16c1; text-align:center; }}
        .logo {{ text-align:center; margin-bottom:20px; }}
        .info {{ margin:20px 0 30px; padding:20px; background:#fff;
                 border-radius:8px; box-shadow:0 2px 5px rgba(0,0,0,.05); }}
        .info p {{ margin:5px 0; font-size:15px; }}
        table {{ width:100%; border-collapse:collapse; background:#fff;
                 border-radius:8px; overflow:hidden; }}
        th,td {{ padding:12px; border:1px solid #ddd; text-align:left; font-size:14px; }}
        th {{ background:#eee; }}
        .old {{ text-decoration:line-through; color:#999; font-size:13px; }}
        .sale {{ color:#d32f2f; font-weight:600; }}
        .total {{ text-align:right; margin-top:20px; font-weight:bold; font-size:18px; }}
    </style>
</head>
<body>
    <div class='logo'>
        <img src='data:image/png;base64,{logoBase64}' width='140' alt='Owl Gallery Logo' />
    </div>
    <h1>Order Receipt</h1>
    <p style='text-align:center;'>Thank you for shopping with Owl Gallery!</p>

    <hr style='margin:25px 0' />

    <div class='info'>
        <p><strong>Order ID:</strong> #{order.Id}</p>
        <p><strong>Name:</strong> {order.FirstName} {order.LastName}</p>
        <p><strong>Email:</strong> {userEmail}</p>
        <p><strong>Date:</strong> {order.CreatedAt.ToLocalTime():f}</p>
        <p><strong>Ship To:</strong><br/>
            {order.Address1}, {order.Address2}<br/>
            {order.City}, {order.State}, {order.PostalCode}<br/>
            {order.Country}
        </p>
    </div>

    <table>
        <thead>
            <tr>
                <th style='width:45%'>Product</th>
                <th style='width:10%'>Qty</th>
                <th style='width:20%'>Unit Price</th>
                <th style='width:25%'>Total</th>
            </tr>
        </thead>
        <tbody>");

            // ───── rows ─────
            decimal grand = 0;
            foreach (var it in items)
            {
                var prod = it.Product ?? throw new InvalidOperationException("Product nav missing");

                // live price (sale aware)
                decimal live = activeSales == null
                               ? it.UnitPrice                          // fallback
                               : prod.GetCurrentPrice(activeSales);

                bool onSale = live < prod.Price;

                string unitHtml = onSale
                    ? $"<span class='old'>EGP {prod.Price:F2}</span><br/><span class='sale'>EGP {live:F2}</span>"
                    : $"EGP {live:F2}";

                decimal line = live * it.Quantity;
                grand += line;

                html.Append($@"
            <tr>
                <td>{prod.Name}</td>
                <td>{it.Quantity}</td>
                <td>{unitHtml}</td>
                <td>EGP {line:F2}</td>
            </tr>");
            }

            html.Append($@"
        </tbody>
    </table>

    <p class='total'>Total Paid: EGP {grand:F2}</p>
</body>
</html>");

            // ───── convert to PDF ─────
            var converter = new HtmlToPdfConverter();
            return converter.ConvertHtmlToPdf(html.ToString());
        }
    }
}
