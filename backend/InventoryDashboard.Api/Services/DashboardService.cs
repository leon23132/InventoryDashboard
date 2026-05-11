using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.Dtos.Dashboard;
using Microsoft.EntityFrameworkCore;
namespace InventoryDashboard.Api.Services
{
    public class DashboardService
    {
        private readonly InventoryDbContext _context;
        public DashboardService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardOverviewDto> GetOverviewAsync()
        {
            // Total counts
            var totalProductsTask = await _context.Products.AsNoTracking().CountAsync();
            // Total categories
            var totalCategoriesTask = await _context.Categories.AsNoTracking().CountAsync();
            // Total suppliers
            var totalSuppliersTask = await _context.Suppliers.AsNoTracking().CountAsync();

            // Products per category
            var lowStockCountTask = await _context.Products.AsNoTracking()
            .CountAsync(p => p.QuantityInStock <= p.MinimumStock);

            // Products per category
            var productsPerCategoryTask = await _context.Products.AsNoTracking()
                .GroupBy(p => new
                {
                    p.CategoryId,
                    CategoryName = p.Category != null ?
                p.Category.Name : "Unknown"
                })
                .Select(
                    g => new CategoryCountDto
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.CategoryName,
                        ProductCount = g.Count()
                    }
                ).OrderByDescending(g => g.ProductCount).ToListAsync();

            var topProductsByStockTask = await _context.Products
                .AsNoTracking()
                .OrderByDescending(p => p.QuantityInStock)
                .ThenBy(p => p.ProductTitle)
                .Take(10)
                .Select(
                    p => new ProductStockDto
                    {
                        Id = p.ProductId,
                        Name = p.ProductTitle,
                        StockQuantity = p.QuantityInStock
                    }
                ).ToListAsync();

            var productsPerSupplierTask = await _context.Products
                .AsNoTracking()
                .GroupBy(p => new
                {
                    p.SupplierId,
                    CompanyName = p.Supplier != null
                    ? p.Supplier.CompanyName : "Unknown"
                }).Select(
                    g => new SupplierCountDto
                    {
                        SupplierId = g.Key.SupplierId,
                        SupplierName = g.Key.CompanyName,
                        ProductCount = g.Count()
                    }
                ).OrderByDescending(g => g.ProductCount).ToListAsync();


            return new DashboardOverviewDto
            {
                TotalProducts = totalProductsTask,
                TotalCategories = totalCategoriesTask,
                TotalSuppliers = totalSuppliersTask,
                LowStockCount = lowStockCountTask,
                ProductsPerCategory = productsPerCategoryTask,
                TopProductsByStock = topProductsByStockTask,
                ProductsPerSupplier = productsPerSupplierTask,
            };
        }
    }
}