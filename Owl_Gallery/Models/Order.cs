using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Owl_Gallery.Models
{
    public class Order
    {
        [Key] public int Id { get; set; }
        public int UserId { get; set; }

        [Required, MaxLength(100)] public string FirstName { get; set; }
        [Required, MaxLength(100)] public string LastName { get; set; }
        [Required, MaxLength(200)] public string Address1 { get; set; }
        [MaxLength(200)] public string Address2 { get; set; }
        [Required, MaxLength(100)] public string City { get; set; }
        [Required, MaxLength(100)] public string State { get; set; }
        [Required, MaxLength(20)] public string PostalCode { get; set; }
        [Required, MaxLength(100)] public string Country { get; set; }

        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
