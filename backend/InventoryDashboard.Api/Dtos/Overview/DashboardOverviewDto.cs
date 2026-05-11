using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryDashboard.Api.Dtos.Categories;
using InventoryDashboard.Api.Dtos.Products;
using InventoryDashboard.Api.Dtos.Suppliers;


namespace InventoryDashboard.Api.Dtos.Dashboard;

public class DashboardOverviewDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalSuppliers { get; set; }

    public int LowStockThreshold { get; set; }
    public int LowStockCount { get; set; }

    public List<CategoryCountDto> ProductsPerCategory { get; set; } = new();
    public List<ProductStockDto> TopProductsByStock { get; set; } = new();
    public List<SupplierCountDto> ProductsPerSupplier { get; set; } = new();
}

public class CategoryCountDto
{
    public int? CategoryId { get; set; } // null möglich, falls Produkt ohne Kategorie
    public string CategoryName { get; set; } = "Uncategorized";
    public int ProductCount { get; set; }
}

public class SupplierCountDto
{
    public int? SupplierId { get; set; } // null möglich
    public string SupplierName { get; set; } = "Unknown Supplier";
    public int ProductCount { get; set; }
}

public class ProductStockDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int StockQuantity { get; set; }
}