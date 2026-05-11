using InventoryDashboard.Api.Dtos.Projects;
using InventoryDashboard.Api.Entities;

namespace InventoryDashboard.Api.Tests.TestData;

public static class ProjectTestData
{
    public static CreateProjectDto CreateProjectDto(
        int productId1,
        int productId2,
        string projectName = "Office Setup",
        string? description = "Arbeitsplätze für neues Büro",
        int quantity1 = 2,
        int quantity2 = 4)
    {
        return new CreateProjectDto
        {
            ProjectName = projectName,
            Description = description,
            Products = new List<CreateProjectProductDto>
            {
                new CreateProjectProductDto
                {
                    ProductId = productId1,
                    Quantity = quantity1
                },
                new CreateProjectProductDto
                {
                    ProductId = productId2,
                    Quantity = quantity2
                }
            }
        };
    }

    public static CreateProjectDto CreateProjectDtoWithoutProducts(
        string projectName = "Leeres Projekt",
        string? description = "Ohne Produkte")
    {
        return new CreateProjectDto
        {
            ProjectName = projectName,
            Description = description,
            Products = new List<CreateProjectProductDto>()
        };
    }

    public static UpdateProjectDto UpdateProjectDto(
        int productId,
        string projectName = "Updated Project",
        string? description = "Updated Description",
        int quantity = 7)
    {
        return new UpdateProjectDto
        {
            ProjectName = projectName,
            Description = description,
            Products = new List<UpdateProjectProductDto>
            {
                new UpdateProjectProductDto
                {
                    ProductId = productId,
                    Quantity = quantity
                }
            }
        };
    }

    public static Project CreateProjectEntity(
        int productId1,
        int productId2,
        string projectName = "Office Setup",
        string? description = "Arbeitsplätze für neues Büro",
        int quantity1 = 2,
        int quantity2 = 4)
    {
        return new Project
        {
            ProjectName = projectName,
            Description = description,
            ProductProjects = new List<ProductProject>
            {
                new ProductProject
                {
                    ProductId = productId1,
                    Quantity = quantity1
                },
                new ProductProject
                {
                    ProductId = productId2,
                    Quantity = quantity2
                }
            }
        };
    }

    public static Project CreateSimpleProjectEntity(
        string projectName = "Test Projekt",
        string? description = "Test Beschreibung",
        int? productId = null,
        int quantity = 1)
    {
        var project = new Project
        {
            ProjectName = projectName,
            Description = description,
            ProductProjects = new List<ProductProject>()
        };

        if (productId.HasValue)
        {
            project.ProductProjects.Add(new ProductProject
            {
                ProductId = productId.Value,
                Quantity = quantity
            });
        }

        return project;
    }

    public static Product CreateProductEntity(
        string productTitle = "Laptop",
        int categoryId = 1,
        int supplierId = 1,
        decimal price = 1200m,
        int quantityInStock = 10,
        string location = "Lager A1")
    {
        return new Product
        {
            ProductTitle = productTitle,
            ProductDescription = $"{productTitle} Beschreibung",
            CategoryId = categoryId,
            SupplierId = supplierId,
            Price = price,
            QuantityInStock = quantityInStock,
            Location = location
        };
    }
}