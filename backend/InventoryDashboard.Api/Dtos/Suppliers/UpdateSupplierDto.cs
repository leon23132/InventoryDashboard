using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using InventoryDashboard.Api.Dtos.Addresses;

namespace InventoryDashboard.Api.Dtos.Suppliers
{
    public class UpdateSupplierDto
    {
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

        // Required: not null
        [Required]
        public UpdateAddressDto BillingAddress { get; set; } = null!;

        // Optional: can be null (-> no shipping address)
        public UpdateAddressDto? ShippingAddress { get; set; }
    }
}