using System.ComponentModel.DataAnnotations;
namespace Owl_Gallery.ViewModels
{
    public class VerifyCodeViewModel
    {
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
