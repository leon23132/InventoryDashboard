using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryDashboard.Api.Dtos.Categories
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        [Required]
        [StringLength(80)]
        public string Name { get; set; } = null!;
    }
}