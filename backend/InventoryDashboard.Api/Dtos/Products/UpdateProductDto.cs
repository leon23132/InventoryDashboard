using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace InventoryDashboard.Api.Dtos.Products
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(100)]
        public string ProductTitle { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ProductDescription { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SupplierId { get; set; }

        [Required]
        [Range(0, 1_000_000)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        [Range(0, int.MaxValue)]
        public int MinimumStock { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }
    }
}