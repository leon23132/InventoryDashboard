using FluentAssertions;
using InventoryDashboard.Api.Controllers;
using InventoryDashboard.Api.Dtos.Suppliers;
using InventoryDashboard.Api.Services;
using InventoryDashboard.Api.Tests.Helpers;
using InventoryDashboard.Api.Tests.TestData;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.Api.Tests.Controllers;

public class SuppliersControllerTests
{
    [Fact]
    public async Task GetAll_Should_Return_Ok_With_Suppliers()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        context.Suppliers.AddRange(
            SupplierTestData.CreateSupplierEntity(companyName: "Muster AG"),
            SupplierTestData.CreateSupplierEntity(companyName: "Tech GmbH")
        );
        await context.SaveChangesAsync();

        var service = new SuppliersService(context);
        var controller = new SuppliersController(service);

        // Act
        var result = await controller.GetAll(null, null, null, 1, 10);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var items = okResult!.Value as List<SupplierListItemDto>;
        items.Should().NotBeNull();
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Supplier_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var supplier = SupplierTestData.CreateSupplierEntity(companyName: "Muster AG");
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var service = new SuppliersService(context);
        var controller = new SuppliersController(service);

        // Act
        var result = await controller.GetById(supplier.SupplierId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var dto = okResult!.Value as SupplierDetailDto;
        dto.Should().NotBeNull();
        dto!.SupplierId.Should().Be(supplier.SupplierId);
        dto.CompanyName.Should().Be("Muster AG");
        dto.BillingAddress.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Supplier_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new SuppliersService(context);
        var controller = new SuppliersController(service);

        // Act
        var result = await controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction_When_Supplier_Is_Created()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new SuppliersService(context);
        var controller = new SuppliersController(service);

        var dto = SupplierTestData.CreateSupplierDto();

        // Act
        var result = await controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(SuppliersController.GetById));
        createdResult.RouteValues.Should().ContainKey("id");

        context.Suppliers.Should().ContainSingle(s => s.CompanyName == dto.CompanyName);
    }

    [Fact]
    public async Task Update_Should_Return_NoContent_When_Supplier_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var supplier = SupplierTestData.CreateSupplierEntity(companyName: "Alt AG");
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var service = new SuppliersService(context);
        var controller = new SuppliersController(service);

        var dto = SupplierTestData.UpdateSupplierDto(companyName: "Neu AG");

        // Act
        var result = await controller.Update(supplier.SupplierId, dto);

        // Assert
        result.Result.Should().BeOfType<NoContentResult>();

        var updatedSupplier = await context.Suppliers.FindAsync(supplier.SupplierId);
        updatedSupplier.Should().NotBeNull();
        updatedSupplier!.CompanyName.Should().Be("Neu AG");
    }

    [Fact]
    public async Task Update_Should_Return_NotFound_When_Supplier_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new SuppliersService(context);
        var controller = new SuppliersController(service);

        var dto = SupplierTestData.UpdateSupplierDto(companyName: "Neu AG");

        // Act
        var result = await controller.Update(999, dto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent_When_Supplier_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var supplier = SupplierTestData.CreateSupplierEntity(companyName: "Muster AG");
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var service = new SuppliersService(context);
        var controller = new SuppliersController(service);

        // Act
        var result = await controller.Delete(supplier.SupplierId);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        var deletedSupplier = await context.Suppliers.FindAsync(supplier.SupplierId);
        deletedSupplier.Should().BeNull();
    }

    [Fact]
    public async Task Delete_Should_Return_NotFound_When_Supplier_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new SuppliersService(context);
        var controller = new SuppliersController(service);

        // Act
        var result = await controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}