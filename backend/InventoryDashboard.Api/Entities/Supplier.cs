using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryDashboard.Api.Entities
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = null!;

        /// <summary>
        /// Rechnungsadresse (Pflicht).
        /// </summary>
        [Required]
        [ForeignKey(nameof(BillingAddress))]
        public int BillingAddressId { get; set; }

        public Address BillingAddress { get; set; } = null!;

        /// <summary>
        /// Lieferadresse (optional, falls abweichend).
        /// </summary>
        [ForeignKey(nameof(ShippingAddress))]
        public int? ShippingAddressId { get; set; }

        public Address? ShippingAddress { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Phone]
        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [Url]
        [StringLength(200)]
        public string? Website { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}