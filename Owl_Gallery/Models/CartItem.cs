using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Owl_Gallery.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public Register User { get; set; }

        [Required, MaxLength(100)]
        public string UserName { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required, MaxLength(200)]
        public string ProductName { get; set; }

        [Range(1, 999)]
        public int Quantity { get; set; }
    }
}
