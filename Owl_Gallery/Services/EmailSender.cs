using System.Threading.Tasks;

namespace Owl_Gallery.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendEmailAsync(string toEmail, string subject, string body, byte[] attachment, string attachmentName);
    }
}
