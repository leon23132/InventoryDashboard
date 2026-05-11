using FluentAssertions;
using InventoryDashboard.Api.Controllers;
using InventoryDashboard.Api.Dtos.Projects;
using InventoryDashboard.Api.Entities;
using InventoryDashboard.Api.Services;
using InventoryDashboard.Api.Tests.Helpers;
using InventoryDashboard.Api.Tests.TestData;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.Api.Tests.Controllers;

public class ProjectsControllerTests
{
    [Fact]
    public async Task GetAll_Should_Return_Ok_With_Projects()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product = ProjectTestData.CreateProductEntity(
            productTitle: "Laptop",
            categoryId: category.CategoryId,
            supplierId: supplier.SupplierId,
            price: 1200m);

        context.Products.Add(product);
        await context.SaveChangesAsync();

        context.Projects.AddRange(
            ProjectTestData.CreateSimpleProjectEntity("Projekt Alpha", "Desc 1", product.ProductId),
            ProjectTestData.CreateSimpleProjectEntity("Projekt Beta", "Desc 2", product.ProductId)
        );
        await context.SaveChangesAsync();

        var service = new ProjectsService(context);
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.GetAll(null, 1, 10);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var items = okResult!.Value as List<ProjectListItemDTO>;
        items.Should().NotBeNull();
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Project_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product1 = ProjectTestData.CreateProductEntity(
            productTitle: "Laptop",
            categoryId: category.CategoryId,
            supplierId: supplier.SupplierId,
            price: 1200m);

        var product2 = ProjectTestData.CreateProductEntity(
            productTitle: "Monitor",
            categoryId: category.CategoryId,
            supplierId: supplier.SupplierId,
            price: 300m);

        context.Products.AddRange(product1, product2);
        await context.SaveChangesAsync();

        var project = ProjectTestData.CreateProjectEntity(product1.ProductId, product2.ProductId);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectsService(context);
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.GetById(project.ProjectId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var dto = okResult!.Value as ProjectDetailDto;
        dto.Should().NotBeNull();
        dto!.ProjectId.Should().Be(project.ProjectId);
        dto.ProjectName.Should().Be("Office Setup");
        dto.Products.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_Should_Throw_Exception_When_Project_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new ProjectsService(context);
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction_When_Project_Is_Created()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product1 = ProjectTestData.CreateProductEntity(
            productTitle: "Laptop",
            categoryId: category.CategoryId,
            supplierId: supplier.SupplierId,
            price: 1200m);

        var product2 = ProjectTestData.CreateProductEntity(
            productTitle: "Monitor",
            categoryId: category.CategoryId,
            supplierId: supplier.SupplierId,
            price: 300m);

        context.Products.AddRange(product1, product2);
        await context.SaveChangesAsync();

        var service = new ProjectsService(context);
        var controller = new ProjectsController(service);

        var dto = ProjectTestData.CreateProjectDto(product1.ProductId, product2.ProductId);

        // Act
        var result = await controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(ProjectsController.GetById));
        createdResult.RouteValues.Should().ContainKey("id");

        context.Projects.Should().ContainSingle(p => p.ProjectName == dto.ProjectName);
    }

    [Fact]
    public async Task Update_Should_Return_NoContent_When_Project_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var oldProduct = ProjectTestData.CreateProductEntity(
            productTitle: "Laptop",
            categoryId: category.CategoryId,
            supplierId: supplier.SupplierId,
            price: 1200m);

        var newProduct = ProjectTestData.CreateProductEntity(
            productTitle: "Monitor",
            categoryId: category.CategoryId,
            supplierId: supplier.SupplierId,
            price: 300m);

        context.Products.AddRange(oldProduct, newProduct);
        await context.SaveChangesAsync();

        var project = ProjectTestData.CreateSimpleProjectEntity(
            projectName: "Altes Projekt",
            description: "Alte Beschreibung",
            productId: oldProduct.ProductId);

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectsService(context);
        var controller = new ProjectsController(service);

        var dto = ProjectTestData.UpdateProjectDto(
            productId: newProduct.ProductId,
            projectName: "Neues Projekt");

        // Act
        var result = await controller.Update(project.ProjectId, dto);

        // Assert
        result.Result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_Should_Return_NotFound_When_Project_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product = ProjectTestData.CreateProductEntity(
            productTitle: "Laptop",
            categoryId: category.CategoryId,
            supplierId: supplier.SupplierId,
            price: 1200m);

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProjectsService(context);
        var controller = new ProjectsController(service);

        var dto = ProjectTestData.UpdateProjectDto(
            productId: product.ProductId,
            projectName: "Neues Projekt");

        // Act
        var result = await controller.Update(999, dto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent_When_Project_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = new Category { Name = "Hardware" };
        var supplier = CreateSupplier();

        context.Categories.Add(category);
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();

        var product = ProjectTestData.CreateProductEntity(
            productTitle: "Laptop",
            categoryId: category.CategoryId,
            supplierId: supplier.SupplierId,
            price: 1200m);

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var project = ProjectTestData.CreateSimpleProjectEntity(
            projectName: "Delete Project",
            description: "To be deleted",
            productId: product.ProductId);

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectsService(context);
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.Delete(project.ProjectId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_Should_Return_NotFound_When_Project_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new ProjectsService(context);
        var controller = new ProjectsController(service);

        // Act
        var result = await controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
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