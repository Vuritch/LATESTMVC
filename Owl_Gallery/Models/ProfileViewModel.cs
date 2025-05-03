using System.ComponentModel.DataAnnotations;

namespace Owl_Gallery.ViewModels
{
    public class ProfileUpdateViewModel
    {
        public int Id { get; set; }

        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
