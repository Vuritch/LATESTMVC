using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Owl_Gallery.Models
{ 
    public class Sale
    {
        [Key]
        public int SaleIdId { get; set; }

        [Required(ErrorMessage = "Please select a product.")]
        public int ProductId { get; set; }

        [Range(0, 100000, ErrorMessage = "Enter a valid sale price.")]
        public decimal? SalePrice { get; set; }

        [Required(ErrorMessage = "Please enter the start date.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please enter the end date.")]
        public DateTime EndDate { get; set; }

        [Range(0, 100, ErrorMessage = "Enter a discount between 0 % and 100 %.")]
        public decimal DiscountPercentage { get; set; }

        [ValidateNever]               
        public Product? Product { get; set; }
    }
}
