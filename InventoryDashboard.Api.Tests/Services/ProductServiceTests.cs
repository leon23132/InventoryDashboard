using FluentAssertions;
using InventoryDashboard.Api.Entities;
using InventoryDashboard.Api.Services;
using InventoryDashboard.Api.Tests.Helpers;
using InventoryDashboard.Api.Tests.TestData;
namespace InventoryDashboard.Api.Tests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Product_And_Return_Id()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        var service = new ProductService(context);
        var dto = ProductTestData.CreateProductDto(category.CategoryId, supplier.SupplierId);

        // Act
        var id = await service.CreateAsync(dto);

        // Assert
        id.Should().BeGreaterThan(0);

        var product = await context.Products.FindAsync(id);
        product.Should().NotBeNull();
        product!.ProductTitle.Should().Be("Laptop");
        product.ProductDescription.Should().Be("Business Laptop");
        product.CategoryId.Should().Be(category.CategoryId);
        product.SupplierId.Should().Be(supplier.SupplierId);
        product.Price.Should().Be(1499.90m);
        product.QuantityInStock.Should().Be(5);
        product.Location.Should().Be("Regal A1");
        product.MinimumStock.Should().Be(10);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Product_When_Product_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        var product = ProductTestData.CreateProductEntity(category.CategoryId, supplier.SupplierId);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.GetByIdAsync(product.ProductId);

        // Assert
        result.Should().NotBeNull();
        result!.ProductId.Should().Be(product.ProductId);
        result.ProductTitle.Should().Be("Monitor");
        result.ProductDescription.Should().Be("27 Zoll Monitor");
        result.CategoryId.Should().Be(category.CategoryId);
        result.CategoryName.Should().Be(category.Name);
        result.SupplierId.Should().Be(supplier.SupplierId);
        result.SupplierName.Should().Be(supplier.CompanyName);
        result.Price.Should().Be(299.50m);
        result.QuantityInStock.Should().Be(8);
        result.MinimumStock.Should().Be(5);
        result.Location.Should().Be("Regal B2");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_Exception_When_Product_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new ProductService(context);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Products_Ordered_By_Title()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        context.Products.AddRange(
            ProductTestData.CreateProductEntityForList("Zebra Maus", "Beschreibung 1", category.CategoryId, supplier.SupplierId, 20m, 10, 5, "A1"),
            ProductTestData.CreateProductEntityForList("Laptop", "Beschreibung 2", category.CategoryId, supplier.SupplierId, 1200m, 3, 10, "A2"),
            ProductTestData.CreateProductEntityForList("Monitor", "Beschreibung 3", category.CategoryId, supplier.SupplierId, 300m, 6, 5, "A3")
        );

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.GetAllAsync(null, null, null, 1, 10);

        // Assert
        result.Should().HaveCount(3);
        result.Select(p => p.ProductTitle)
            .Should()
            .ContainInOrder("Laptop", "Monitor", "Zebra Maus");
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Search_Text_In_Title_Or_Description()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        context.Products.AddRange(
            ProductTestData.CreateProductEntityForList("Laptop", "Business Gerät", category.CategoryId, supplier.SupplierId, 1200m, 3, 10, "A1"),
            ProductTestData.CreateProductEntityForList("Monitor", "Gaming Bildschirm", category.CategoryId, supplier.SupplierId, 300m, 5, 5, "A2"),
            ProductTestData.CreateProductEntityForList("Maus", "Kabellose Business Maus", category.CategoryId, supplier.SupplierId, 50m, 8, 5, "A3")
        );

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.GetAllAsync("Business", null, null, 1, 10);

        // Assert
        result.Should().HaveCount(2);
        result.Select(p => p.ProductTitle)
            .Should()
            .Contain(new[] { "Laptop", "Maus" });
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_CategoryId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category1 = new Category { Name = "Hardware" };
        var category2 = new Category { Name = "Software" };
        var supplier = new Supplier
        {
            CompanyName = "Tech Supplier AG",
            Email = "info@tech.ch",
            PhoneNumber = "0441234567",
            Website = "https://tech.ch",
            ContactPerson = "Max Muster",
            BillingAddress = new Address
            {
                StreetAddress = "Teststrasse 1",
                City = "Zürich",
                PostalCode = "8000",
                Country = "Schweiz"
            }
        };

        context.Categories.AddRange(category1, category2);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            ProductTestData.CreateProductEntityForList("Laptop", "Hardware Produkt", category1.CategoryId, supplier.SupplierId, 1000m, 2, 10, "A1"),
            ProductTestData.CreateProductEntityForList("Office", "Software Produkt", category2.CategoryId, supplier.SupplierId, 200m, 10, 5, "A2")
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.GetAllAsync(null, category1.CategoryId, null, 1, 10);

        // Assert
        result.Should().HaveCount(1);
        result[0].ProductTitle.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_SupplierId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };

        var supplier1 = new Supplier
        {
            CompanyName = "Supplier A",
            Email = "a@test.ch",
            PhoneNumber = "0441111111",
            Website = "https://a.ch",
            ContactPerson = "Person A",
            BillingAddress = new Address
            {
                StreetAddress = "Strasse 1",
                City = "Zürich",
                PostalCode = "8000",
                Country = "Schweiz"
            }
        };

        var supplier2 = new Supplier
        {
            CompanyName = "Supplier B",
            Email = "b@test.ch",
            PhoneNumber = "0442222222",
            Website = "https://b.ch",
            ContactPerson = "Person B",
            BillingAddress = new Address
            {
                StreetAddress = "Strasse 2",
                City = "Winterthur",
                PostalCode = "8400",
                Country = "Schweiz"
            }
        };

        context.Categories.Add(category);
        context.Suppliers.AddRange(supplier1, supplier2);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            ProductTestData.CreateProductEntityForList("Laptop", "Produkt A", category.CategoryId, supplier1.SupplierId, 1000m, 2, 10, "A1"),
            ProductTestData.CreateProductEntityForList("Monitor", "Produkt B", category.CategoryId, supplier2.SupplierId, 300m, 4, 5, "A2")
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.GetAllAsync(null, null, supplier2.SupplierId, 1, 10);

        // Assert
        result.Should().HaveCount(1);
        result[0].ProductTitle.Should().Be("Monitor");
    }

    [Fact]
    public async Task GetAllAsync_Should_Apply_Pagination()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        context.Products.AddRange(
            ProductTestData.CreateProductEntityForList("A Produkt", "Desc", category.CategoryId, supplier.SupplierId, 10m, 1, 5, "A1"),
            ProductTestData.CreateProductEntityForList("B Produkt", "Desc", category.CategoryId, supplier.SupplierId, 20m, 2, 5, "A2"),
            ProductTestData.CreateProductEntityForList("C Produkt", "Desc", category.CategoryId, supplier.SupplierId, 30m, 3, 5, "A3")
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.GetAllAsync(null, null, null, 2, 1);

        // Assert
        result.Should().HaveCount(1);
        result[0].ProductTitle.Should().Be("B Produkt");
    }

    [Fact]
    public async Task GetAllAsync_Should_Use_Default_Page_And_PageSize_When_Invalid_Values_Are_Passed()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        for (int i = 1; i <= 15; i++)
        {
            context.Products.Add(
                ProductTestData.CreateProductEntityForList(
                    $"Produkt {i:D2}",
                    "Beschreibung",
                    category.CategoryId,
                    supplier.SupplierId,
                    10m + i,
                    i,
                    5,
                    $"R{i}"
                )
            );
        }

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.GetAllAsync(null, null, null, 0, 0);

        // Assert
        result.Should().HaveCount(10);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Product_When_Product_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var categoryOld = new Category { Name = "Hardware" };
        var categoryNew = new Category { Name = "Software" };

        var supplierOld = new Supplier
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

        var supplierNew = new Supplier
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

        context.Categories.AddRange(categoryOld, categoryNew);
        context.Suppliers.AddRange(supplierOld, supplierNew);
        await context.SaveChangesAsync();

        var product = ProductTestData.CreateProductEntity(categoryOld.CategoryId, supplierOld.SupplierId);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);
        var dto = ProductTestData.UpdateProductDto(categoryNew.CategoryId, supplierNew.SupplierId);

        // Act
        var result = await service.UpdateAsync(product.ProductId, dto);

        // Assert
        result.Should().BeTrue();

        var updatedProduct = await context.Products.FindAsync(product.ProductId);
        updatedProduct.Should().NotBeNull();
        updatedProduct!.ProductTitle.Should().Be("Updated Laptop");
        updatedProduct.ProductDescription.Should().Be("Updated Description");
        updatedProduct.CategoryId.Should().Be(categoryNew.CategoryId);
        updatedProduct.SupplierId.Should().Be(supplierNew.SupplierId);
        updatedProduct.Price.Should().Be(1999.00m);
        updatedProduct.QuantityInStock.Should().Be(12);
        updatedProduct.MinimumStock.Should().Be(10);
        updatedProduct.Location.Should().Be("Regal C3");
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_False_When_Product_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        var service = new ProductService(context);
        var dto = ProductTestData.UpdateProductDto(category.CategoryId, supplier.SupplierId);

        // Act
        var result = await service.UpdateAsync(999, dto);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_True_And_Remove_Product_When_Product_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        var product = ProductTestData.CreateProductEntity(category.CategoryId, supplier.SupplierId);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.DeleteAsync(product.ProductId);

        // Assert
        result.Should().BeTrue();

        var deletedProduct = await context.Products.FindAsync(product.ProductId);
        deletedProduct.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Product_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new ProductService(context);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAll_Should_Return_All_Products_As_ProductDto()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        SeedCategoryAndSupplier(context, out var category, out var supplier);

        context.Products.AddRange(
            ProductTestData.CreateProductEntityForList("Laptop", "Desc 1", category.CategoryId, supplier.SupplierId, 1000m, 2, 10, "A1"),
            ProductTestData.CreateProductEntityForList("Monitor", "Desc 2", category.CategoryId, supplier.SupplierId, 300m, 4, 5, "A2")
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = service.GetAll();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Title == "Laptop");
        result.Should().Contain(p => p.Title == "Monitor");
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