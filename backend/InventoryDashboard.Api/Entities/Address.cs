using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryDashboard.Api.Entities
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        [StringLength(150)]
        public string StreetAddress { get; set; } = null!;

        [Required]
        [StringLength(80)]
        public string City { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; } = null!;

        [Required]
        [StringLength(80)]
        public string Country { get; set; } = null!;
    }
}