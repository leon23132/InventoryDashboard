using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.Dtos.Products;

namespace InventoryDashboard.Api.Services
{
    public class ProductService
    {
           private readonly InventoryDbContext _context;

        public ProductService(InventoryDbContext context)
        {
            _context = context;
        }

      public List<ProductDto> GetAll()
        {
            return _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.ProductId,
                    Title = p.ProductTitle,
                    Description = p.ProductDescription,
                    CategoryId = p.CategoryId,
                    SupplierId = p.SupplierId,
                    Price = p.Price,
                    QuantityInStock = p.QuantityInStock,
                    Location = p.Location
                })
                .ToList();
        }
        
    }
}