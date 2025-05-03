using System.ComponentModel.DataAnnotations;
namespace Owl_Gallery.Models
{
    // In Product.cs
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Category { get; set; }

        [Range(0, 99999)]
        public decimal Price { get; set; }

        [Required, MaxLength(250)]
        public string ImageUrl { get; set; }

        public int Quantity { get; set; }

        // Add this line for the description
        [MaxLength(500)]  // You can adjust the length as needed
        public string Description { get; set; }  // New description property
    }

}
