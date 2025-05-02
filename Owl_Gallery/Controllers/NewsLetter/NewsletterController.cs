using Microsoft.AspNetCore.Mvc;
using Owl_Gallery.Services;
using System.Threading.Tasks;

namespace Owl_Gallery.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly IEmailSender _emailSender;

        public NewsletterController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required");

            await _emailSender.SendEmailAsync(
                email,
                "Welcome to Owl Gallery!",
                "<h1>Thanks for subscribing 🦉✨</h1><p>We’ll send you updates soon!</p>"
            );

            return Ok();
        }
    }
}
