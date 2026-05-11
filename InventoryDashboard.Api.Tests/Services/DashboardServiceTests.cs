using FluentAssertions;
using InventoryDashboard.Api.Services;
using InventoryDashboard.Api.Tests.Helpers;
using InventoryDashboard.Api.Tests.TestData;

namespace InventoryDashboard.Api.Tests.Services;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetOverviewAsync_Should_Return_Correct_Total_Counts()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category1 = DashboardTestData.CreateHardwareCategory();
        var category2 = DashboardTestData.CreateSoftwareCategory();

        var supplier1 = DashboardTestData.CreateTechSupplier();
        var supplier2 = DashboardTestData.CreateOfficeSupplier();

        context.Categories.AddRange(category1, category2);
        context.Suppliers.AddRange(supplier1, supplier2);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            DashboardTestData.CreateLaptop(category1.CategoryId, supplier1.SupplierId, 10, 1500m),
            DashboardTestData.CreateMonitor(category1.CategoryId, supplier1.SupplierId, 3, 300m),
            DashboardTestData.CreateOfficeProduct(category2.CategoryId, supplier2.SupplierId, 20, 99m)
        );

        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var result = await service.GetOverviewAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalProducts.Should().Be(3);
        result.TotalCategories.Should().Be(2);
        result.TotalSuppliers.Should().Be(2);
    }

    [Fact]
    public async Task GetOverviewAsync_Should_Return_Correct_LowStockCount_With_Default_Threshold()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = DashboardTestData.CreateHardwareCategory();
        var supplier = DashboardTestData.CreateTechSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            DashboardTestData.CreateLaptop(category.CategoryId, supplier.SupplierId, 10, 1500m),
            DashboardTestData.CreateMonitor(category.CategoryId, supplier.SupplierId, 5, 300m),
            DashboardTestData.CreateMouse(category.CategoryId, supplier.SupplierId, 2, 50m)
        );

        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var result = await service.GetOverviewAsync();

        // Assert
        result.LowStockThreshold.Should().Be(5);
        result.LowStockCount.Should().Be(2);
    }

    [Fact]
    public async Task GetOverviewAsync_Should_Return_Correct_LowStockCount_With_Custom_Threshold()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = DashboardTestData.CreateHardwareCategory();
        var supplier = DashboardTestData.CreateTechSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            DashboardTestData.CreateLaptop(category.CategoryId, supplier.SupplierId, 10, 1500m),
            DashboardTestData.CreateMonitor(category.CategoryId, supplier.SupplierId, 5, 300m),
            DashboardTestData.CreateMouse(category.CategoryId, supplier.SupplierId, 2, 50m)
        );

        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var result = await service.GetOverviewAsync(2);

        // Assert
        result.LowStockThreshold.Should().Be(2);
        result.LowStockCount.Should().Be(1);
    }

    [Fact]
    public async Task GetOverviewAsync_Should_Return_ProductsPerCategory()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var hardware = DashboardTestData.CreateHardwareCategory();
        var software = DashboardTestData.CreateSoftwareCategory();
        var supplier = DashboardTestData.CreateTechSupplier();

        context.Categories.AddRange(hardware, software);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            DashboardTestData.CreateLaptop(hardware.CategoryId, supplier.SupplierId, 10, 1500m),
            DashboardTestData.CreateMonitor(hardware.CategoryId, supplier.SupplierId, 3, 300m),
            DashboardTestData.CreateOfficeProduct(software.CategoryId, supplier.SupplierId, 20, 99m)
        );

        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var result = await service.GetOverviewAsync();

        // Assert
        result.ProductsPerCategory.Should().HaveCount(2);

        result.ProductsPerCategory.Should().Contain(c =>
            c.CategoryId == hardware.CategoryId &&
            c.CategoryName == "Hardware" &&
            c.ProductCount == 2);

        result.ProductsPerCategory.Should().Contain(c =>
            c.CategoryId == software.CategoryId &&
            c.CategoryName == "Software" &&
            c.ProductCount == 1);
    }

    [Fact]
    public async Task GetOverviewAsync_Should_Order_ProductsPerCategory_By_ProductCount_Descending()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var hardware = DashboardTestData.CreateHardwareCategory();
        var software = DashboardTestData.CreateSoftwareCategory();
        var supplier = DashboardTestData.CreateTechSupplier();

        context.Categories.AddRange(hardware, software);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            DashboardTestData.CreateLaptop(hardware.CategoryId, supplier.SupplierId, 10, 1500m),
            DashboardTestData.CreateMonitor(hardware.CategoryId, supplier.SupplierId, 3, 300m),
            DashboardTestData.CreateMouse(hardware.CategoryId, supplier.SupplierId, 7, 50m),
            DashboardTestData.CreateOfficeProduct(software.CategoryId, supplier.SupplierId, 20, 99m)
        );

        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var result = await service.GetOverviewAsync();

        // Assert
        result.ProductsPerCategory.Should().HaveCount(2);
        result.ProductsPerCategory.First().CategoryName.Should().Be("Hardware");
        result.ProductsPerCategory.First().ProductCount.Should().Be(3);
    }

    [Fact]
    public async Task GetOverviewAsync_Should_Return_TopProductsByStock_Ordered_Correctly()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = DashboardTestData.CreateHardwareCategory();
        var supplier = DashboardTestData.CreateTechSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            DashboardTestData.CreateMouse(category.CategoryId, supplier.SupplierId, 8, 50m),
            DashboardTestData.CreateLaptop(category.CategoryId, supplier.SupplierId, 15, 1500m),
            DashboardTestData.CreateMonitor(category.CategoryId, supplier.SupplierId, 15, 300m),
            DashboardTestData.CreateKeyboard(category.CategoryId, supplier.SupplierId, 4, 80m)
        );

        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var result = await service.GetOverviewAsync();

        // Assert
        result.TopProductsByStock.Should().HaveCount(4);
        result.TopProductsByStock[0].StockQuantity.Should().Be(15);
        result.TopProductsByStock[1].StockQuantity.Should().Be(15);

        result.TopProductsByStock.Take(2).Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "Laptop", "Monitor" });

        result.TopProductsByStock[2].Name.Should().Be("Mouse");
        result.TopProductsByStock[2].StockQuantity.Should().Be(8);
        result.TopProductsByStock[3].Name.Should().Be("Keyboard");
        result.TopProductsByStock[3].StockQuantity.Should().Be(4);
    }

    [Fact]
    public async Task GetOverviewAsync_Should_Return_Max_10_TopProductsByStock()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = DashboardTestData.CreateHardwareCategory();
        var supplier = DashboardTestData.CreateTechSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        for (int i = 1; i <= 12; i++)
        {
            context.Products.Add(
                DashboardTestData.CreateCustomProduct(
                    $"Produkt {i:D2}",
                    category.CategoryId,
                    supplier.SupplierId,
                    i,
                    10m + i
                )
            );
        }

        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var result = await service.GetOverviewAsync();

        // Assert
        result.TopProductsByStock.Should().HaveCount(10);
        result.TopProductsByStock.First().StockQuantity.Should().Be(12);
        result.TopProductsByStock.Last().StockQuantity.Should().Be(3);
    }

    [Fact]
    public async Task GetOverviewAsync_Should_Return_ProductsPerSupplier()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = DashboardTestData.CreateHardwareCategory();
        var supplier1 = DashboardTestData.CreateSupplierA();
        var supplier2 = DashboardTestData.CreateSupplierB();

        context.Categories.Add(category);
        context.Suppliers.AddRange(supplier1, supplier2);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            DashboardTestData.CreateLaptop(category.CategoryId, supplier1.SupplierId, 10, 1500m),
            DashboardTestData.CreateMonitor(category.CategoryId, supplier1.SupplierId, 3, 300m),
            DashboardTestData.CreateMouse(category.CategoryId, supplier2.SupplierId, 8, 50m)
        );

        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var result = await service.GetOverviewAsync();

        // Assert
        result.ProductsPerSupplier.Should().HaveCount(2);

        result.ProductsPerSupplier.Should().Contain(s =>
            s.SupplierId == supplier1.SupplierId &&
            s.SupplierName == "Supplier A" &&
            s.ProductCount == 2);

        result.ProductsPerSupplier.Should().Contain(s =>
            s.SupplierId == supplier2.SupplierId &&
            s.SupplierName == "Supplier B" &&
            s.ProductCount == 1);
    }

    [Fact]
    public async Task GetOverviewAsync_Should_Order_ProductsPerSupplier_By_ProductCount_Descending()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = DashboardTestData.CreateHardwareCategory();
        var supplier1 = DashboardTestData.CreateSupplierA();
        var supplier2 = DashboardTestData.CreateSupplierB();

        context.Categories.Add(category);
        context.Suppliers.AddRange(supplier1, supplier2);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            DashboardTestData.CreateLaptop(category.CategoryId, supplier1.SupplierId, 10, 1500m),
            DashboardTestData.CreateMonitor(category.CategoryId, supplier1.SupplierId, 3, 300m),
            DashboardTestData.CreateMouse(category.CategoryId, supplier1.SupplierId, 8, 50m),
            DashboardTestData.CreateKeyboard(category.CategoryId, supplier2.SupplierId, 4, 80m)
        );

        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var result = await service.GetOverviewAsync();

        // Assert
        result.ProductsPerSupplier.Should().HaveCount(2);
        result.ProductsPerSupplier.First().SupplierName.Should().Be("Supplier A");
        result.ProductsPerSupplier.First().ProductCount.Should().Be(3);
    }

    [Fact]
    public async Task GetOverviewAsync_Should_Return_Empty_Collections_When_No_Data_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new DashboardService(context);

        // Act
        var result = await service.GetOverviewAsync();

        // Assert
        result.TotalProducts.Should().Be(0);
        result.TotalCategories.Should().Be(0);
        result.TotalSuppliers.Should().Be(0);
        result.LowStockCount.Should().Be(0);
        result.ProductsPerCategory.Should().BeEmpty();
        result.TopProductsByStock.Should().BeEmpty();
        result.ProductsPerSupplier.Should().BeEmpty();
        result.LowStockThreshold.Should().Be(5);
    }
}