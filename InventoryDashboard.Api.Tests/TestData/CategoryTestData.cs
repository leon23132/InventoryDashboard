using InventoryDashboard.Api.Entities;

namespace InventoryDashboard.Api.Tests.TestData;

public static class CategoryTestData
{
    public static Category CreateCategory(string name = "Hardware")
    {
        return new Category
        {
            Name = name
        };
    }

    public static CreateCategoryDto CreateCategoryDto(string name = "Hardware")
    {
        return new CreateCategoryDto
        {
            Name = name
        };
    }

    public static UpdateCategoryDto UpdateCategoryDto(int categoryId, string name = "Updated Hardware")
    {
        return new UpdateCategoryDto
        {
            CategoryId = categoryId,
            Name = name
        };
    }
}