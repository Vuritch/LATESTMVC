using System.ComponentModel.DataAnnotations;
namespace Owl_Gallery.Models
{
    public class Sale
    {
        [Key]
        public int SaleIdId { get; set; }
        public int ProductId { get; set; }  // Foreign key to Product
        public decimal? SalePrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountPercentage { get; set; }
        public Product Product { get; set; }  
    }
}