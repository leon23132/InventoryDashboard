using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryDashboard.Api.Dtos.Products
{
  public class ProductDto
    {
         public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public int SupplierId { get; set; }

        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public string? Location { get; set; }
    }
}