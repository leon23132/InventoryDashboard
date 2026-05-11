using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using InventoryDashboard.Api.Dtos.Addresses;

namespace InventoryDashboard.Api.Dtos.Suppliers
{
    public class CreateSupplierDto
    {
        // Information about the supplier
        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = null!;

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

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        // Adressen
        [Required]
        public CreateAdressDto BillingAddress { get; set; } = null!;

        // Optional shipping address
        public CreateAdressDto? ShippingAddress { get; set; }
    }
}