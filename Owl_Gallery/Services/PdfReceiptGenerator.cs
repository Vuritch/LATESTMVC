using Owl_Gallery.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Owl_Gallery.Services
{
    public static class PdfReceiptGenerator
    {
        public static byte[] Generate(Order order, List<OrderItem> items, string userEmail)
        {
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
            var logoBase64 = Convert.ToBase64String(File.ReadAllBytes(logoPath));

            var html = new StringBuilder();

            html.Append($@"
    <html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                padding: 40px;
                background: #f9f9ff;
            }}
            h1 {{
                color: #5d16c1;
                text-align: center;
            }}
            .logo {{
                text-align: center;
                margin-bottom: 20px;
            }}
            .info {{
                margin-top: 20px;
                margin-bottom: 30px;
                padding: 20px;
                background: #fff;
                border-radius: 8px;
                box-shadow: 0 2px 5px rgba(0,0,0,0.05);
            }}
            .info p {{
                margin: 5px 0;
                font-size: 15px;
            }}
            table {{
                width: 100%;
                border-collapse: collapse;
                background: #fff;
                border-radius: 8px;
                overflow: hidden;
            }}
            th, td {{
                padding: 12px;
                border: 1px solid #ddd;
                text-align: left;
            }}
            th {{
                background: #eee;
            }}
            .total {{
                text-align: right;
                margin-top: 20px;
                font-weight: bold;
                font-size: 18px;
            }}
        </style>
    </head>
    <body>
        <div class='logo'>
            <img src='data:image/png;base64,{logoBase64}' width='140' alt='Owl Gallery Logo' />
        </div>
        <h1>Order Receipt</h1>
        <p style='text-align:center;'>Thank you for shopping with Owl Gallery!</p>

        <div class='info'>
            <p><strong>Order ID:</strong> #{order.Id}</p>
            <p><strong>Name:</strong> {order.FirstName} {order.LastName}</p>
            <p><strong>Email:</strong> {userEmail}</p>
            <p><strong>Date:</strong> {order.CreatedAt.ToLocalTime():f}</p>
            <p><strong>Shipping Address:</strong><br/>
                {order.Address1}, {order.Address2}<br/>
                {order.City}, {order.State}, {order.PostalCode}<br/>
                {order.Country}
            </p>
        </div>

        <table>
            <thead>
                <tr>
                    <th>Product</th>
                    <th>Qty</th>
                    <th>Unit Price</th>
                    <th>Total</th>
                </tr>
            </thead>
            <tbody>");

            foreach (var item in items)
            {
                html.Append($@"
            <tr>
                <td>{item.Product?.Name}</td>
                <td>{item.Quantity}</td>
                <td>EGP {item.UnitPrice:F2}</td>
                <td>EGP {(item.UnitPrice * item.Quantity):F2}</td>
            </tr>");
            }

            html.Append($@"
            </tbody>
        </table>

        <p class='total'>Total Paid: EGP {order.Total:F2}</p>
    </body>
    </html>");

            var converter = new HtmlToPdfConverter(); // your own PDF converter
            return converter.ConvertHtmlToPdf(html.ToString());
        }
    }
}
