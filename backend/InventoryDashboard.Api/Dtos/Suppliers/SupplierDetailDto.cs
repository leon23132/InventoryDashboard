using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryDashboard.Api.Dtos.Addresses;

namespace InventoryDashboard.Api.Dtos.Suppliers
{
    public class SupplierDetailDto
    {
          public int SupplierId { get; set; }

        public string CompanyName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Website { get; set; }

        public string? ContactPerson { get; set; }

        // Adressen 
        public AddressDto BillingAddress { get; set; } = null!;

        public AddressDto? ShippingAddress { get; set; }
    }
}