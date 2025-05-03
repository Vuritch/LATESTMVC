using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Owl_Gallery.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        // → Foreign key back to your user table (Registers)
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public Register User { get; set; }   // navigation

        // → Foreign key into Products
        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } // navigation
    }
}
