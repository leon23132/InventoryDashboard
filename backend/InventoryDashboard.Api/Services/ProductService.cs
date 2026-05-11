using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.Dtos.Products;
using Microsoft.EntityFrameworkCore;

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
                    MinimumStock = p.MinimumStock,
                    Location = p.Location

                })
                .ToList();
        }

        //GetProducts with Filters
        public async Task<List<ProductListItemDto>> GetAllAsync(
            string? q, int? categoryId, int? supplierID, int page, int pageSize)
        {
            //Query Start
            var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .AsQueryable();

            //Filters for Title
            if (!string.IsNullOrWhiteSpace(q))
            {
                var s = q.Trim();

                query = query.Where(p =>
                    p.ProductTitle.Contains(s) ||
                    (p.ProductDescription != null && p.ProductDescription.Contains(s))
                    );
            }

            //Filter for Category
            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);

            //Filter for Supplier
            if (supplierID.HasValue) query = query.Where(p => p.SupplierId == supplierID.Value);

            // Pagination validation
            if (page <= 0) page = 1;

            if (pageSize <= 0) pageSize = 10;

            if (pageSize > 100) pageSize = 100;


            //Execute Query and Return Results
            return await query
                .OrderBy(p => p.ProductTitle)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductListItemDto
                {
                    ProductId = p.ProductId,
                    ProductTitle = p.ProductTitle,
                    CategoryName = p.Category.Name,
                    SupplierName = p.Supplier.CompanyName,
                    Price = p.Price,
                    QuantityInStock = p.QuantityInStock,
                    MinimumStock = p.MinimumStock,
                    Location = p.Location
                }).ToListAsync();

        }

        //Get Product by Id
        public async Task<ProductDetailDto?> GetByIdAsync(int id)
        {
            //Query and Return Result
            return await _context.Products.AsNoTracking()
          .Where(p => p.ProductId == id)
          .Include(p => p.Category)
          .Include(p => p.Supplier)
          .Select(p => new ProductDetailDto
          {
              ProductId = p.ProductId,
              ProductTitle = p.ProductTitle,
              ProductDescription = p.ProductDescription,
              CategoryId = p.CategoryId,
              CategoryName = p.Category.Name,
              SupplierId = p.SupplierId,
              SupplierName = p.Supplier.CompanyName,
              Price = p.Price,
              QuantityInStock = p.QuantityInStock,
              MinimumStock = p.MinimumStock,
              Location = p.Location
          })
          .FirstOrDefaultAsync();
        }

        //Create Product
        public async Task<int> CreateAsync(CreateProductDto dto)
        {
            var entity = new Entities.Product
            {
                ProductTitle = dto.ProductTitle,
                ProductDescription = dto.ProductDescription,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                Price = dto.Price,
                QuantityInStock = dto.QuantityInStock,
                MinimumStock = dto.MinimumStock,
                Location = dto.Location
            };

            _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity.ProductId;
        }

        //Update Product
        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var entity = await _context.Products.FindAsync(id);
            if (entity is null) return false;

            entity.ProductTitle = dto.ProductTitle;
            entity.ProductDescription = dto.ProductDescription;
            entity.CategoryId = dto.CategoryId;
            entity.SupplierId = dto.SupplierId;
            entity.Price = dto.Price;
            entity.QuantityInStock = dto.QuantityInStock;
            entity.MinimumStock = dto.MinimumStock;
            entity.Location = dto.Location;

            await _context.SaveChangesAsync();
            return true;
        }

        //Delete Product
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Products.FindAsync(id);
            if (entity is null) return false;

            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}