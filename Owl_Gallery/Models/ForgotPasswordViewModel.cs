// ViewModels/ForgotPasswordViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Owl_Gallery.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
