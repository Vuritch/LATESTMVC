namespace Owl_Gallery.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "New password is required.")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string NewPassword { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Please confirm your password.")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
