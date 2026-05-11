using FluentAssertions;
using InventoryDashboard.Api.Entities;
using InventoryDashboard.Api.Services;
using InventoryDashboard.Api.Tests.Helpers;
using InventoryDashboard.Api.Tests.TestData;
using Microsoft.EntityFrameworkCore;
namespace InventoryDashboard.Api.Tests.Services;

public class ProjectsServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Project_With_Products_And_Return_Id()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product1 = ProjectTestData.CreateProductEntity("Laptop", category.CategoryId, supplier.SupplierId, 1200m);
        var product2 = ProjectTestData.CreateProductEntity("Monitor", category.CategoryId, supplier.SupplierId, 300m);

        context.Products.AddRange(product1, product2);
        await context.SaveChangesAsync();

        var service = new ProjectsService(context);
        var dto = ProjectTestData.CreateProjectDto(product1.ProductId, product2.ProductId);

        // Act
        var id = await service.CreateAsync(dto);

        // Assert
        id.Should().BeGreaterThan(0);

        var project = await context.Projects
            .Include(p => p.ProductProjects)
            .FirstOrDefaultAsync(p => p.ProjectId == id);

        project.Should().NotBeNull();
        project!.ProjectName.Should().Be("Office Setup");
        project.Description.Should().Be("Arbeitsplätze für neues Büro");
        project.ProductProjects.Should().HaveCount(2);
        project.ProductProjects.Should().Contain(pp => pp.ProductId == product1.ProductId && pp.Quantity == 2);
        project.ProductProjects.Should().Contain(pp => pp.ProductId == product2.ProductId && pp.Quantity == 4);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_ArgumentException_When_No_Products_Are_Provided()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new ProjectsService(context);
        var dto = ProjectTestData.CreateProjectDtoWithoutProducts();

        // Act
        Func<Task> act = async () => await service.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("At least one product must be specified for the project.");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Project_When_Project_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product1 = ProjectTestData.CreateProductEntity("Laptop", category.CategoryId, supplier.SupplierId, 1200m);
        var product2 = ProjectTestData.CreateProductEntity("Monitor", category.CategoryId, supplier.SupplierId, 300m);

        context.Products.AddRange(product1, product2);
        await context.SaveChangesAsync();

        var project = ProjectTestData.CreateProjectEntity(product1.ProductId, product2.ProductId);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectsService(context);

        // Act
        var result = await service.GetByIdAsync(project.ProjectId);

        // Assert
        result.Should().NotBeNull();
        result.ProjectId.Should().Be(project.ProjectId);
        result.ProjectName.Should().Be("Office Setup");
        result.Description.Should().Be("Arbeitsplätze für neues Büro");
        result.Products.Should().HaveCount(2);
        result.Products.Should().Contain(p => p.ProductTitle == "Laptop" && p.Quantity == 2 && p.UnitPrice == 1200m);
        result.Products.Should().Contain(p => p.ProductTitle == "Monitor" && p.Quantity == 4 && p.UnitPrice == 300m);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_Exception_When_Project_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new ProjectsService(context);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Projects_Ordered_By_ProjectName()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product = ProjectTestData.CreateProductEntity("Laptop", category.CategoryId, supplier.SupplierId, 1200m);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        context.Projects.AddRange(
            ProjectTestData.CreateSimpleProjectEntity("Zeta Projekt", "Desc 1", product.ProductId),
            ProjectTestData.CreateSimpleProjectEntity("Alpha Projekt", "Desc 2", product.ProductId),
            ProjectTestData.CreateSimpleProjectEntity("Beta Projekt", "Desc 3", product.ProductId)
        );

        await context.SaveChangesAsync();

        var service = new ProjectsService(context);

        // Act
        var result = await service.GetAllAsync(null, 1, 10);

        // Assert
        result.Should().HaveCount(3);
        result.Select(p => p.ProjectName)
            .Should()
            .ContainInOrder("Alpha Projekt", "Beta Projekt", "Zeta Projekt");
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Search_Text()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product = ProjectTestData.CreateProductEntity("Laptop", category.CategoryId, supplier.SupplierId, 1200m);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        context.Projects.AddRange(
            ProjectTestData.CreateSimpleProjectEntity("Office Setup", "Büro Arbeitsplätze", product.ProductId),
            ProjectTestData.CreateSimpleProjectEntity("Warehouse Upgrade", "Lager Ausbau", product.ProductId),
            ProjectTestData.CreateSimpleProjectEntity("Office Extension", "Büro Erweiterung", product.ProductId)
        );

        await context.SaveChangesAsync();

        var service = new ProjectsService(context);

        // Act
        var result = await service.GetAllAsync("Office", 1, 10);

        // Assert
        result.Should().HaveCount(2);
        result.Select(p => p.ProjectName)
            .Should()
            .Contain(new[] { "Office Setup", "Office Extension" });
    }

    [Fact]
    public async Task GetAllAsync_Should_Apply_Pagination()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product = ProjectTestData.CreateProductEntity("Laptop", category.CategoryId, supplier.SupplierId, 1200m);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        context.Projects.AddRange(
            ProjectTestData.CreateSimpleProjectEntity("Alpha Projekt", "Desc", product.ProductId),
            ProjectTestData.CreateSimpleProjectEntity("Beta Projekt", "Desc", product.ProductId),
            ProjectTestData.CreateSimpleProjectEntity("Gamma Projekt", "Desc", product.ProductId)
        );

        await context.SaveChangesAsync();

        var service = new ProjectsService(context);

        // Act
        var result = await service.GetAllAsync(null, 2, 1);

        // Assert
        result.Should().HaveCount(1);
        result[0].ProjectName.Should().Be("Beta Projekt");
    }

    [Fact]
    public async Task GetAllAsync_Should_Use_Default_Page_And_PageSize_When_Invalid_Values_Are_Passed()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product = ProjectTestData.CreateProductEntity("Laptop", category.CategoryId, supplier.SupplierId, 1200m);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        for (int i = 1; i <= 15; i++)
        {
            context.Projects.Add(
                ProjectTestData.CreateSimpleProjectEntity($"Projekt {i:D2}", "Beschreibung", product.ProductId)
            );
        }

        await context.SaveChangesAsync();

        var service = new ProjectsService(context);

        // Act
        var result = await service.GetAllAsync(null, 0, 0);

        // Assert
        result.Should().HaveCount(10);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Project_When_Project_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var oldProduct = ProjectTestData.CreateProductEntity("Laptop", category.CategoryId, supplier.SupplierId, 1200m);
        var newProduct = ProjectTestData.CreateProductEntity("Monitor", category.CategoryId, supplier.SupplierId, 300m);

        context.Products.AddRange(oldProduct, newProduct);
        await context.SaveChangesAsync();

        var project = ProjectTestData.CreateSimpleProjectEntity("Altes Projekt", "Alte Beschreibung", oldProduct.ProductId);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectsService(context);
        var dto = ProjectTestData.UpdateProjectDto(newProduct.ProductId);

        // Act
        var result = await service.UpdateAsync(project.ProjectId, dto);

        // Assert
        result.Should().BeTrue();

        var updatedProject = await context.Projects
            .Include(p => p.ProductProjects)
            .FirstOrDefaultAsync(p => p.ProjectId == project.ProjectId);

        updatedProject.Should().NotBeNull();
        updatedProject!.ProjectName.Should().Be("Updated Project");
        updatedProject.Description.Should().Be("Updated Description");
        updatedProject.ProductProjects.Should().HaveCount(1);
        updatedProject.Should().NotBeNull();
        updatedProject!.ProjectName.Should().Be("Updated Project");
        updatedProject.Description.Should().Be("Updated Description");
        updatedProject.ProductProjects.Should().HaveCount(1);

        var productProject = updatedProject.ProductProjects.Single();
        productProject.ProductId.Should().Be(newProduct.ProductId);
        productProject.Quantity.Should().Be(7);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_False_When_Project_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new ProjectsService(context);
        var dto = ProjectTestData.UpdateProjectDto(1);

        // Act
        var result = await service.UpdateAsync(999, dto);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_True_And_Remove_Project_When_Project_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product = ProjectTestData.CreateProductEntity("Laptop", category.CategoryId, supplier.SupplierId, 1200m);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var project = ProjectTestData.CreateSimpleProjectEntity("Delete Project", "To be deleted", product.ProductId);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectsService(context);

        // Act
        var result = await service.DeleteAsync(project.ProjectId);

        // Assert
        result.Should().BeTrue();

        var deletedProject = await context.Projects.FindAsync(project.ProjectId);
        deletedProject.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Project_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new ProjectsService(context);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    private static Supplier CreateSupplier()
    {
        return new Supplier
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
    }
}