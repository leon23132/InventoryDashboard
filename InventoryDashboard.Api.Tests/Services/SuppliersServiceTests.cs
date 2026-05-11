using FluentAssertions;
using InventoryDashboard.Api.Services;
using InventoryDashboard.Api.Tests.Helpers;
using InventoryDashboard.Api.Tests.TestData;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Api.Tests.Services;

public class SuppliersServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Supplier_With_Billing_And_Shipping_Address()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new SuppliersService(context);

        var dto = SupplierTestData.CreateSupplierDto();

        // Act
        var id = await service.CreateAsync(dto);

        // Assert
        id.Should().BeGreaterThan(0);

        var supplier = await context.Suppliers
            .Include(s => s.BillingAddress)
            .Include(s => s.ShippingAddress)
            .FirstOrDefaultAsync(s => s.SupplierId == id);

        supplier.Should().NotBeNull();
        supplier!.CompanyName.Should().Be("Muster AG");
        supplier.ContactPerson.Should().Be("Max Muster");
        supplier.BillingAddress.Should().NotBeNull();
        supplier.BillingAddress!.City.Should().Be("Zürich");
        supplier.ShippingAddress.Should().NotBeNull();
        supplier.ShippingAddress!.City.Should().Be("Winterthur");
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Supplier_Without_Shipping_Address()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new SuppliersService(context);

        var dto = SupplierTestData.CreateSupplierDtoWithoutShipping();

        // Act
        var id = await service.CreateAsync(dto);

        // Assert
        var supplier = await context.Suppliers
            .Include(s => s.BillingAddress)
            .Include(s => s.ShippingAddress)
            .FirstOrDefaultAsync(s => s.SupplierId == id);

        supplier.Should().NotBeNull();
        supplier!.CompanyName.Should().Be("Ohne Versand AG");
        supplier.BillingAddress.Should().NotBeNull();
        supplier.BillingAddress!.City.Should().Be("Bern");
        supplier.ShippingAddress.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Supplier_When_Supplier_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var supplier = SupplierTestData.CreateSupplierEntity();

        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var service = new SuppliersService(context);

        // Act
        var result = await service.GetByIdAsync(supplier.SupplierId);

        // Assert
        result.Should().NotBeNull();
        result!.SupplierId.Should().Be(supplier.SupplierId);
        result.CompanyName.Should().Be("TechTrade GmbH");
        result.BillingAddress.Should().NotBeNull();
        result.BillingAddress.City.Should().Be("Basel");
        result.ShippingAddress.Should().NotBeNull();
        result.ShippingAddress!.City.Should().Be("Luzern");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Supplier_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new SuppliersService(context);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Search_Text()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        context.Suppliers.AddRange(
            SupplierTestData.CreateSupplierEntityForSearch(
                companyName: "Muster AG",
                contactPerson: "Max Muster",
                city: "Zürich"),
            SupplierTestData.CreateSupplierEntityForSearch(
                companyName: "Tech World",
                contactPerson: "Anna Keller",
                city: "Bern")
        );

        await context.SaveChangesAsync();

        var service = new SuppliersService(context);

        // Act
        var result = await service.GetAllAsync("Muster", null, null, 1, 10);

        // Assert
        result.Should().HaveCount(1);
        result[0].CompanyName.Should().Be("Muster AG");
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_ContactPerson()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        context.Suppliers.AddRange(
            SupplierTestData.CreateSupplierEntityForSearch(
                companyName: "Alpha AG",
                contactPerson: "Max Muster",
                city: "Zürich"),
            SupplierTestData.CreateSupplierEntityForSearch(
                companyName: "Beta AG",
                contactPerson: "Anna Keller",
                city: "Bern")
        );

        await context.SaveChangesAsync();

        var service = new SuppliersService(context);

        // Act
        var result = await service.GetAllAsync(null, "Anna", null, 1, 10);

        // Assert
        result.Should().HaveCount(1);
        result[0].CompanyName.Should().Be("Beta AG");
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_City()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        context.Suppliers.AddRange(
            SupplierTestData.CreateSupplierEntityForSearch(
                companyName: "Alpha AG",
                contactPerson: "Kontakt A",
                city: "Winterthur"),
            SupplierTestData.CreateSupplierEntityForSearch(
                companyName: "Beta AG",
                contactPerson: "Kontakt B",
                city: "Bern")
        );

        await context.SaveChangesAsync();

        var service = new SuppliersService(context);

        // Act
        var result = await service.GetAllAsync(null, null, "Winterthur", 1, 10);

        // Assert
        result.Should().HaveCount(1);
        result[0].CompanyName.Should().Be("Alpha AG");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Supplier_When_Supplier_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var supplier = SupplierTestData.CreateSupplierEntity(
            companyName: "Alt AG",
            email: "alt@firma.ch",
            phoneNumber: "0440000000",
            website: "https://alt.ch",
            contactPerson: "Alt Person",
            billingStreetAddress: "Altweg 1",
            billingCity: "Bern",
            billingPostalCode: "3000");

        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var service = new SuppliersService(context);
        var dto = SupplierTestData.UpdateSupplierDto();

        // Act
        var result = await service.UpdateAsync(supplier.SupplierId, dto);

        // Assert
        result.Should().BeTrue();

        var updatedSupplier = await context.Suppliers
            .Include(s => s.BillingAddress)
            .Include(s => s.ShippingAddress)
            .FirstOrDefaultAsync(s => s.SupplierId == supplier.SupplierId);

        updatedSupplier.Should().NotBeNull();
        updatedSupplier!.CompanyName.Should().Be("Neu AG");
        updatedSupplier.Email.Should().Be("neu@firma.ch");
        updatedSupplier.PhoneNumber.Should().Be("0441112233");
        updatedSupplier.Website.Should().Be("https://neu.ch");
        updatedSupplier.ContactPerson.Should().Be("Neu Person");

        updatedSupplier.BillingAddress.Should().NotBeNull();
        updatedSupplier.BillingAddress!.StreetAddress.Should().Be("Neuweg 2");
        updatedSupplier.BillingAddress.City.Should().Be("Zürich");
        updatedSupplier.BillingAddress.PostalCode.Should().Be("8001");
        updatedSupplier.BillingAddress.Country.Should().Be("Schweiz");

        updatedSupplier.ShippingAddress.Should().NotBeNull();
        updatedSupplier.ShippingAddress!.StreetAddress.Should().Be("Lagerweg 3");
        updatedSupplier.ShippingAddress.City.Should().Be("Winterthur");
        updatedSupplier.ShippingAddress.PostalCode.Should().Be("8400");
        updatedSupplier.ShippingAddress.Country.Should().Be("Schweiz");
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_False_When_Supplier_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new SuppliersService(context);
        var dto = SupplierTestData.UpdateSupplierDto();

        // Act
        var result = await service.UpdateAsync(999, dto);

        // Assert
        result.Should().BeFalse();
        context.Suppliers.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_True_And_Remove_Supplier_When_Supplier_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var supplier = SupplierTestData.CreateSupplierEntity(
            companyName: "Delete AG",
            email: "delete@firma.ch",
            phoneNumber: "0449998877",
            website: "https://delete.ch",
            contactPerson: "Delete Person",
            billingStreetAddress: "Delete 1",
            billingCity: "Bern",
            billingPostalCode: "3000");

        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var service = new SuppliersService(context);

        // Act
        var result = await service.DeleteAsync(supplier.SupplierId);

        // Assert
        result.Should().BeTrue();

        var deletedSupplier = await context.Suppliers.FindAsync(supplier.SupplierId);
        deletedSupplier.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Supplier_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new SuppliersService(context);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }
}