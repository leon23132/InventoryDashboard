using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryDashboard.Api.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductTitle { get; set; } = null!;

        [StringLength(500)]
        public string? ProductDescription { get; set; }

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1_000_000)]
        public decimal Price { get; set; }

        /// <summary>
        /// Aktueller Lagerbestand.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        [Required]
        [ForeignKey(nameof(Supplier))]
        public int SupplierId { get; set; }

        public Supplier Supplier { get; set; } = null!;

        /// <summary>
        /// Optional: Lagerort im Betrieb (z.B. Regal A3, Lager Nord usw.).
        /// </summary>
        [StringLength(100)]
        public string? Location { get; set; }

        public ICollection<ProductProject> ProductProjects { get; set; } = new List<ProductProject>();
    }
}