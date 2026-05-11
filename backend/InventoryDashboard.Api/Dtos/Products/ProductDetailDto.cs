using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace InventoryDashboard.Api.Dtos.Products
{
    public class ProductDetailDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductTitle { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ProductDescription { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string? CategoryName { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public string? SupplierName { get; set; }

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