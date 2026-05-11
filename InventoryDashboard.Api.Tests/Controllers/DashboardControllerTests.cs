using FluentAssertions;
using InventoryDashboard.Api.Controllers;
using InventoryDashboard.Api.Dtos.Dashboard;
using InventoryDashboard.Api.Services;
using InventoryDashboard.Api.Tests.Helpers;
using InventoryDashboard.Api.Tests.TestData;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.Api.Tests.Controllers;

public class DashboardControllerTests
{
    [Fact]
    public async Task GetOverview_Should_Return_Ok_With_Correct_Dashboard_Data_When_Threshold_Is_Valid()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var hardwareCategory = DashboardTestData.CreateHardwareCategory();
        var softwareCategory = DashboardTestData.CreateSoftwareCategory();

        var supplierA = DashboardTestData.CreateSupplierA();
        var supplierB = DashboardTestData.CreateSupplierB();

        context.Categories.AddRange(hardwareCategory, softwareCategory);
        context.Suppliers.AddRange(supplierA, supplierB);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            DashboardTestData.CreateLaptop(
                hardwareCategory.CategoryId,
                supplierA.SupplierId,
                quantityInStock: 10,
                price: 1500m),

            DashboardTestData.CreateMonitor(
                hardwareCategory.CategoryId,
                supplierA.SupplierId,
                quantityInStock: 3,
                price: 300m),

            DashboardTestData.CreateOfficeProduct(
                softwareCategory.CategoryId,
                supplierB.SupplierId,
                quantityInStock: 20,
                price: 99m)
        );

        await context.SaveChangesAsync();

        var service = new DashboardService(context);
        var controller = new DashboardController(service);

        // Act
        var result = await controller.GetOverview(5);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();

        var dto = okResult.Value.Should().BeOfType<DashboardOverviewDto>().Subject;

        dto.TotalProducts.Should().Be(3);
        dto.TotalCategories.Should().Be(2);
        dto.TotalSuppliers.Should().Be(2);
        dto.LowStockCount.Should().Be(1);
        dto.LowStockThreshold.Should().Be(5);

        dto.ProductsPerCategory.Should().HaveCount(2);
        dto.ProductsPerCategory.Should().Contain(x =>
            x.CategoryName == hardwareCategory.Name && x.ProductCount == 2);
        dto.ProductsPerCategory.Should().Contain(x =>
            x.CategoryName == softwareCategory.Name && x.ProductCount == 1);

        dto.ProductsPerSupplier.Should().HaveCount(2);
        dto.ProductsPerSupplier.Should().Contain(x =>
            x.SupplierName == supplierA.CompanyName && x.ProductCount == 2);
        dto.ProductsPerSupplier.Should().Contain(x =>
            x.SupplierName == supplierB.CompanyName && x.ProductCount == 1);

        dto.TopProductsByStock.Should().NotBeNull();
        dto.TopProductsByStock.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetOverview_Should_Return_BadRequest_When_Threshold_Is_Negative()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new DashboardService(context);
        var controller = new DashboardController(service);

        // Act
        var result = await controller.GetOverview(-1);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("lowStockThreshold must be >= 0");
    }
}