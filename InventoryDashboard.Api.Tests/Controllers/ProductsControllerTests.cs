using FluentAssertions;
using InventoryDashboard.Api.Controllers;
using InventoryDashboard.Api.Services;
using InventoryDashboard.Api.Tests.Helpers;
using InventoryDashboard.Api.Tests.TestData;
using Microsoft.AspNetCore.Mvc;
using InventoryDashboard.Api.Entities;
namespace InventoryDashboard.Api.Tests.Controllers;

public class ProductsControllerTests
{
    [Fact]
    public async Task GetAll_Should_Return_Ok_With_Product_List()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        context.Products.AddRange(
            ProductTestData.CreateProductEntityForList("Laptop", "Business Laptop", category.CategoryId, supplier.SupplierId, 1499.90m, 5, 10, "A1"),
            ProductTestData.CreateProductEntityForList("Monitor", "27 Zoll Monitor", category.CategoryId, supplier.SupplierId, 299.50m, 8, 5, "B2")
        );

        await context.SaveChangesAsync();

        var service = new ProductService(context);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.GetAll(null, null, null, 1, 10);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();

        var items = okResult.Value.Should().BeAssignableTo<List<InventoryDashboard.Api.Dtos.Products.ProductListItemDto>>().Subject;
        items.Should().HaveCount(2);
        items.Select(x => x.ProductTitle).Should().ContainInOrder("Laptop", "Monitor");
    }

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Product_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        var product = ProductTestData.CreateProductEntity(
            category.CategoryId,
            supplier.SupplierId,
            productTitle: "Monitor",
            productDescription: "27 Zoll Monitor",
            price: 299.50m,
            quantityInStock: 8,
            minimumStock: 5,
            location: "Regal B2");

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.GetById(product.ProductId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();

        var dto = okResult.Value.Should().BeAssignableTo<InventoryDashboard.Api.Dtos.Products.ProductDetailDto>().Subject;
        dto.ProductId.Should().Be(product.ProductId);
        dto.ProductTitle.Should().Be("Monitor");
        dto.ProductDescription.Should().Be("27 Zoll Monitor");
        dto.CategoryId.Should().Be(category.CategoryId);
        dto.CategoryName.Should().Be(category.Name);
        dto.SupplierId.Should().Be(supplier.SupplierId);
        dto.SupplierName.Should().Be(supplier.CompanyName);
        dto.Price.Should().Be(299.50m);
        dto.QuantityInStock.Should().Be(8);
        dto.MinimumStock.Should().Be(5);
        dto.Location.Should().Be("Regal B2");
    }

    [Fact]
    public async Task GetById_Should_Throw_Exception_When_Product_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new ProductService(context);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction_When_Product_Is_Created()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        var dto = ProductTestData.CreateProductDto(category.CategoryId, supplier.SupplierId);
        var service = new ProductService(context);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.Create(dto);

        // Assert
        var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtResult.ActionName.Should().Be(nameof(ProductsController.GetById));
        createdAtResult.RouteValues.Should().ContainKey("id");

        var createdId = (int)createdAtResult.RouteValues!["id"]!;
        createdId.Should().BeGreaterThan(0);

        var product = await context.Products.FindAsync(createdId);
        product.Should().NotBeNull();
        product!.ProductTitle.Should().Be(dto.ProductTitle);
        product.ProductDescription.Should().Be(dto.ProductDescription);
        product.CategoryId.Should().Be(dto.CategoryId);
        product.SupplierId.Should().Be(dto.SupplierId);
        product.Price.Should().Be(dto.Price);
        product.QuantityInStock.Should().Be(dto.QuantityInStock);
        product.MinimumStock.Should().Be(dto.MinimumStock);
        product.Location.Should().Be(dto.Location);
    }

    [Fact]
    public async Task Update_Should_Return_NoContent_When_Product_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var oldCategory = new Category { Name = "Hardware" };
        var newCategory = new Category { Name = "Software" };

        var oldSupplier = new Supplier
        {
            CompanyName = "Supplier Alt",
            Email = "alt@test.ch",
            PhoneNumber = "0441111111",
            Website = "https://alt.ch",
            ContactPerson = "Alt Person",
            BillingAddress = new Address
            {
                StreetAddress = "Altweg 1",
                City = "Bern",
                PostalCode = "3000",
                Country = "Schweiz"
            }
        };

        var newSupplier = new Supplier
        {
            CompanyName = "Supplier Neu",
            Email = "neu@test.ch",
            PhoneNumber = "0442222222",
            Website = "https://neu.ch",
            ContactPerson = "Neu Person",
            BillingAddress = new Address
            {
                StreetAddress = "Neuweg 2",
                City = "Zürich",
                PostalCode = "8000",
                Country = "Schweiz"
            }
        };

        context.Categories.AddRange(oldCategory, newCategory);
        context.Suppliers.AddRange(oldSupplier, newSupplier);
        await context.SaveChangesAsync();

        var product = ProductTestData.CreateProductEntity(oldCategory.CategoryId, oldSupplier.SupplierId);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var dto = ProductTestData.UpdateProductDto(newCategory.CategoryId, newSupplier.SupplierId);
        var service = new ProductService(context);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.Update(product.ProductId, dto);

        // Assert
        result.Result.Should().BeOfType<NoContentResult>();

        var updatedProduct = await context.Products.FindAsync(product.ProductId);
        updatedProduct.Should().NotBeNull();
        updatedProduct!.ProductTitle.Should().Be("Updated Laptop");
        updatedProduct.ProductDescription.Should().Be("Updated Description");
        updatedProduct.CategoryId.Should().Be(newCategory.CategoryId);
        updatedProduct.SupplierId.Should().Be(newSupplier.SupplierId);
        updatedProduct.Price.Should().Be(1999.00m);
        updatedProduct.QuantityInStock.Should().Be(12);
        updatedProduct.MinimumStock.Should().Be(10);
        updatedProduct.Location.Should().Be("Regal C3");
    }

    [Fact]
    public async Task Update_Should_Return_NotFound_When_Product_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        var dto = ProductTestData.UpdateProductDto(category.CategoryId, supplier.SupplierId);
        var service = new ProductService(context);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.Update(999, dto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent_When_Product_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        var product = ProductTestData.CreateProductEntity(category.CategoryId, supplier.SupplierId);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.Delete(product.ProductId);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        var deletedProduct = await context.Products.FindAsync(product.ProductId);
        deletedProduct.Should().BeNull();
    }

    [Fact]
    public async Task Delete_Should_Return_NotFound_When_Product_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new ProductService(context);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    private static void SeedCategoryAndSupplier(
        InventoryDashboard.Api.Data.InventoryDbContext context,
        out Category category,
        out Supplier supplier)
    {
        category = new Category
        {
            Name = "Hardware"
        };

        supplier = new Supplier
        {
            CompanyName = "Tech Supplier AG",
            Email = "info@techsupplier.ch",
            PhoneNumber = "0441234567",
            Website = "https://techsupplier.ch",
            ContactPerson = "Max Muster",
            BillingAddress = new Address
            {
                StreetAddress = "Bahnhofstrasse 1",
                City = "Zürich",
                PostalCode = "8001",
                Country = "Schweiz"
            }
        };

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        context.SaveChanges();
    }
}