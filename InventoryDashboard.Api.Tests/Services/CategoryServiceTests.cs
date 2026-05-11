using FluentAssertions;
using InventoryDashboard.Api.Services;
using InventoryDashboard.Api.Tests.Helpers;
using InventoryDashboard.Api.Tests.TestData;

namespace InventoryDashboard.Api.Tests.Services;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Category_And_Return_Id()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new CategoryService(context);

        var dto = CategoryTestData.CreateCategoryDto("Hardware");

        // Act
        var id = await service.CreateAsync(dto);

        // Assert
        id.Should().BeGreaterThan(0);

        var category = await context.Categories.FindAsync(id);
        category.Should().NotBeNull();
        category!.Name.Should().Be("Hardware");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Category_When_Category_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = CategoryTestData.CreateCategory("Software");
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act
        var result = await service.GetByIdAsync(category.CategoryId);

        // Assert
        result.Should().NotBeNull();
        result!.CategoryId.Should().Be(category.CategoryId);
        result.Name.Should().Be("Software");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Category_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new CategoryService(context);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Categories_Ordered_By_Name()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        context.Categories.AddRange(
            CategoryTestData.CreateCategory("Zubehör"),
            CategoryTestData.CreateCategory("Hardware"),
            CategoryTestData.CreateCategory("Software")
        );
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act
        var result = await service.GetAllAsync(null, 1, 10);

        // Assert
        result.Should().HaveCount(3);
        result.Select(c => c.Name).Should().ContainInOrder("Hardware", "Software", "Zubehör");
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Search_Text()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        context.Categories.AddRange(
            CategoryTestData.CreateCategory("Hardware"),
            CategoryTestData.CreateCategory("Software"),
            CategoryTestData.CreateCategory("Büromaterial")
        );
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act
        var result = await service.GetAllAsync("ware", 1, 10);

        // Assert
        result.Should().HaveCount(2);
        result.Select(c => c.Name).Should().Contain(new[] { "Hardware", "Software" });
    }

    [Fact]
    public async Task GetAllAsync_Should_Apply_Pagination()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        context.Categories.AddRange(
            CategoryTestData.CreateCategory("A"),
            CategoryTestData.CreateCategory("B"),
            CategoryTestData.CreateCategory("C")
        );
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act
        var result = await service.GetAllAsync(null, 2, 1);

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("B");
    }

    [Fact]
    public async Task GetAllAsync_Should_Use_Default_Page_And_PageSize_When_Invalid_Values_Are_Passed()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        for (int i = 1; i <= 15; i++)
        {
            context.Categories.Add(CategoryTestData.CreateCategory($"Category {i:D2}"));
        }

        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act
        var result = await service.GetAllAsync(null, 0, 0);

        // Assert
        result.Should().HaveCount(10);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_True_And_Update_Category_When_Category_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = CategoryTestData.CreateCategory("Alt");
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        var dto = CategoryTestData.UpdateCategoryDto(category.CategoryId, "Neu");

        // Act
        var result = await service.UpdateAsync(category.CategoryId, dto);

        // Assert
        result.Should().BeTrue();

        var updatedCategory = await context.Categories.FindAsync(category.CategoryId);
        updatedCategory.Should().NotBeNull();
        updatedCategory!.Name.Should().Be("Neu");
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_False_When_Category_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new CategoryService(context);

        var dto = CategoryTestData.UpdateCategoryDto(999, "Neu");

        // Act
        var result = await service.UpdateAsync(999, dto);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_True_And_Remove_Category_When_Category_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();

        var category = CategoryTestData.CreateCategory("Zu löschen");
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act
        var result = await service.DeleteAsync(category.CategoryId);

        // Assert
        result.Should().BeTrue();

        var deletedCategory = await context.Categories.FindAsync(category.CategoryId);
        deletedCategory.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Category_Does_Not_Exist()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateContext();
        var service = new CategoryService(context);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }
}