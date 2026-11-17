
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class InventoryItem
    {
        public int ItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, 99999999)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(100)]
        public string Supplier { get; set; }
    }
}
