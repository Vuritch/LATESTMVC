using System.ComponentModel.DataAnnotations;

namespace Owl_Gallery.ViewModels
{
    public class ShippingInfo
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address line 1 is required")]
        public string Address1 { get; set; }

        // <-- Make Address2 required as well
        [Required(ErrorMessage = "Address line 2 is required")]
        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
    }
}
