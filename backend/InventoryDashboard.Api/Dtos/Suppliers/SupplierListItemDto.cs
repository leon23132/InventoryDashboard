using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryDashboard.Api.Dtos.Addresses;

namespace InventoryDashboard.Api.Dtos.Suppliers
{
    public class SupplierListItemDto
    {
        public int SupplierId { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Website { get; set; }
        public string? ContactPerson { get; set; }

        public AddressDto BillingAddress { get; set; } = null!;
        public AddressDto? ShippingAddress { get; set; }
    }
}