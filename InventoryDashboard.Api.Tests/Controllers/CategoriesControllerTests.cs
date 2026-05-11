using FluentAssertions;
using InventoryDashboard.Api.Controllers;
using InventoryDashboard.Api.Tests.Helpers;
using InventoryDashboard.Api.Tests.TestData;
using Microsoft.AspNetCore.Mvc;
using InventoryDashboard.Api.Services;


namespace InventoryDashboard.Api.Tests.Controllers;

public class CategoriesControllerTests
{
    [Fact]
    public async Task GetAll_Should_Return_Ok_With_Category_List()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        context.Categories.AddRange(
            CategoryTestData.CreateCategory("Hardware"),
            CategoryTestData.CreateCategory("Software")
        );
        await context.SaveChangesAsync();

        var service = new CategoryService(context);
        var controller = new CategoriesController(service);

        // Act
        var result = await controller.GetAll(null, 1, 10);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var items = okResult!.Value as List<CategoryListItemDto>;
        items.Should().NotBeNull();
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Category_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = CategoryTestData.CreateCategory();
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);
        var controller = new CategoriesController(service);

        // Act
        var result = await controller.GetById(category.CategoryId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var dto = okResult!.Value as CategoryDetailDto;
        dto.Should().NotBeNull();
        dto!.CategoryId.Should().Be(category.CategoryId);
        dto.Name.Should().Be(category.Name);
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Category_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new CategoryService(context);
        var controller = new CategoriesController(service);

        // Act
        var result = await controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction_When_Category_Is_Created()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new CategoryService(context);
        var controller = new CategoriesController(service);

        var dto = CategoryTestData.CreateCategoryDto();

        // Act
        var result = await controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(CategoriesController.GetById));
        createdResult.RouteValues.Should().ContainKey("id");

        context.Categories.Should().ContainSingle(c => c.Name == dto.Name);
    }

    [Fact]
    public async Task Update_Should_Return_NoContent_When_Category_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = CategoryTestData.CreateCategory("Alt");
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);
        var controller = new CategoriesController(service);

        var dto = CategoryTestData.UpdateCategoryDto(category.CategoryId, "Neu");

        // Act
        var result = await controller.Update(category.CategoryId, dto);

        // Assert
        result.Result.Should().BeOfType<NoContentResult>();

        var updatedCategory = await context.Categories.FindAsync(category.CategoryId);
        updatedCategory.Should().NotBeNull();
        updatedCategory!.Name.Should().Be("Neu");
    }

    [Fact]
    public async Task Update_Should_Return_NotFound_When_Category_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new CategoryService(context);
        var controller = new CategoriesController(service);

        var dto = CategoryTestData.UpdateCategoryDto(999, "Neu");

        // Act
        var result = await controller.Update(999, dto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent_When_Category_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = CategoryTestData.CreateCategory();
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);
        var controller = new CategoriesController(service);

        // Act
        var result = await controller.Delete(category.CategoryId);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        var deletedCategory = await context.Categories.FindAsync(category.CategoryId);
        deletedCategory.Should().BeNull();
    }

    [Fact]
    public async Task Delete_Should_Return_NotFound_When_Category_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new CategoryService(context);
        var controller = new CategoriesController(service);

        // Act
        var result = await controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}