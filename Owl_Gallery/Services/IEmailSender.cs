using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Owl_Gallery.Models;
using System.IO;

namespace Owl_Gallery.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var smtp = new SmtpClient(_settings.SmtpServer)
            {
                Port = int.Parse(_settings.Port),
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true,
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            await smtp.SendMailAsync(message);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, byte[] attachment, string attachmentName)
        {
            using var smtp = new SmtpClient(_settings.SmtpServer)
            {
                Port = int.Parse(_settings.Port),
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true,
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            if (attachment != null && attachment.Length > 0)
            {
                var stream = new MemoryStream(attachment);
                var pdfAttachment = new Attachment(stream, attachmentName, "application/pdf");
                message.Attachments.Add(pdfAttachment);
            }

            await smtp.SendMailAsync(message);
        }
    }
}
