using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Owl_Gallery.Models
{
    public class OrderItem
    {
        [Key] public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))] public Order Order { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))] public Product Product { get; set; }

        [Range(1, 999)] public int Quantity { get; set; }
        [Range(0.01, 9999)] public decimal UnitPrice { get; set; }
    }
}
