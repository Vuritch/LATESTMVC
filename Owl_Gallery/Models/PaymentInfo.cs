using System.ComponentModel.DataAnnotations;

namespace Owl_Gallery.ViewModels
{
    public class PaymentInfo
    {
        [Required, Display(Name = "Name on card")]
        public string CardName { get; set; }

        // Allow either "dddddddddddddddd" or "dddd dddd dddd dddd"
        [Required]
        [RegularExpression(
            @"^(?:\d{16}|(?:\d{4}\s){3}\d{4})$",
            ErrorMessage = "Card number must be exactly 16 digits"
        )]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(
            @"^(0[1-9]|1[0-2])\/\d{2}$",
            ErrorMessage = "Expiry must be in MM/YY"
        )]
        public string Expiry { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVC must be 3 digits")]
        public string Cvc { get; set; }
    }
}
