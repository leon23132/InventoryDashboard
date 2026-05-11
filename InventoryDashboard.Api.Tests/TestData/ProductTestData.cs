using InventoryDashboard.Api.Dtos.Products;
using InventoryDashboard.Api.Entities;

namespace InventoryDashboard.Api.Tests.TestData;

public static class ProductTestData
{
    public static CreateProductDto CreateProductDto(
        int categoryId,
        int supplierId,
        string productTitle = "Laptop",
        string? productDescription = "Business Laptop",
        decimal price = 1499.90m,
        int quantityInStock = 5,
        int minimumStock = 10,
        string location = "Regal A1"
        )
    {
        return new CreateProductDto
        {
            ProductTitle = productTitle,
            ProductDescription = productDescription,
            CategoryId = categoryId,
            SupplierId = supplierId,
            Price = price,
            QuantityInStock = quantityInStock,
            MinimumStock = minimumStock,
            Location = location
        };
    }

    public static UpdateProductDto UpdateProductDto(
        int categoryId,
        int supplierId,
        string productTitle = "Updated Laptop",
        string? productDescription = "Updated Description",
        decimal price = 1999.00m,
        int quantityInStock = 12,
        int minimumStock = 10,
        string location = "Regal C3")
    {
        return new UpdateProductDto
        {
            ProductTitle = productTitle,
            ProductDescription = productDescription,
            CategoryId = categoryId,
            SupplierId = supplierId,
            Price = price,
            QuantityInStock = quantityInStock,
            MinimumStock = minimumStock,
            Location = location
        };
    }

    public static Product CreateProductEntity(
        int categoryId,
        int supplierId,
        string productTitle = "Monitor",
        string? productDescription = "27 Zoll Monitor",
        decimal price = 299.50m,
        int quantityInStock = 8,
        int minimumStock = 5,
        string location = "Regal B2")
    {
        return new Product
        {
            ProductTitle = productTitle,
            ProductDescription = productDescription,
            CategoryId = categoryId,
            SupplierId = supplierId,
            Price = price,
            QuantityInStock = quantityInStock,
            MinimumStock = minimumStock,
            Location = location
        };
    }

    public static Product CreateProductEntityForList(
        string productTitle,
        string? productDescription,
        int categoryId,
        int supplierId,
        decimal price,
        int quantityInStock,
        int minimumStock,
        string location
        )
    {
        return new Product
        {
            ProductTitle = productTitle,
            ProductDescription = productDescription,
            CategoryId = categoryId,
            SupplierId = supplierId,
            Price = price,
            QuantityInStock = quantityInStock,
            MinimumStock = minimumStock,
            Location = location
        };
    }
}