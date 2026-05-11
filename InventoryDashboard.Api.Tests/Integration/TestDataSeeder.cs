using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.Entities;


namespace InventoryDashboard.Api.Tests.Integration;

public static class TestDataSeeder
{
    public static void Seed(InventoryDbContext db)
    {
        // Guard gegen mehrfaches Seeding
        if (db.Products.Any() || db.Categories.Any() || db.Suppliers.Any() || db.Projects.Any())
            return;

        // --------------------
        // ADDRESSES
        // --------------------
        var supplier1Billing = new Address
        {
            StreetAddress = "Teststrasse 1",
            City = "Zürich",
            PostalCode = "8000",
            Country = "CH"
        };

        var supplier1Shipping = new Address
        {
            StreetAddress = "Industriestrasse 5",
            City = "Winterthur",
            PostalCode = "8400",
            Country = "CH"
        };

        var supplier2Billing = new Address
        {
            StreetAddress = "Büroweg 10",
            City = "Bern",
            PostalCode = "3000",
            Country = "CH"
        };

        db.Addresses.AddRange(supplier1Billing, supplier1Shipping, supplier2Billing);
        db.SaveChanges();

        // --------------------
        // CATEGORIES
        // --------------------
        var category1 = new Category { Name = "Elektronik" };
        var category2 = new Category { Name = "Büro" };

        db.Categories.AddRange(category1, category2);
        db.SaveChanges();

        // --------------------
        // SUPPLIERS
        // --------------------
        var supplier1 = new Supplier
        {
            CompanyName = "Tech AG",
            Email = "tech@test.ch",
            ContactPerson = "Max Muster",
            BillingAddress = supplier1Billing,
            ShippingAddress = supplier1Shipping
        };

        var supplier2 = new Supplier
        {
            CompanyName = "Office GmbH",
            Email = "office@test.ch",
            ContactPerson = "Anna Meier",
            BillingAddress = supplier2Billing
        };

        db.Suppliers.AddRange(supplier1, supplier2);
        db.SaveChanges();

        // --------------------
        // PRODUCTS
        // --------------------
        var product1 = new Product
        {
            ProductTitle = "Laptop",
            CategoryId = category1.CategoryId,
            SupplierId = supplier1.SupplierId,
            Price = 1500,
            QuantityInStock = 5,
            MinimumStock = 10,
            Location = "A1"
        };

        var product2 = new Product
        {
            ProductTitle = "Maus",
            CategoryId = category1.CategoryId,
            SupplierId = supplier1.SupplierId,
            Price = 25,
            QuantityInStock = 50,
            MinimumStock = 20,
            Location = "B2"
        };

        var product3 = new Product
        {
            ProductTitle = "Papier",
            CategoryId = category2.CategoryId,
            SupplierId = supplier2.SupplierId,
            Price = 10,
            QuantityInStock = 100,
            MinimumStock = 30,
            Location = "C3"
        };

        db.Products.AddRange(product1, product2, product3);
        db.SaveChanges();

        // --------------------
        // PROJECTS
        // --------------------
        var project = new Project
        {
            ProjectName = "Test Project",
            Description = "Demo"
        };

        db.Projects.Add(project);
        db.SaveChanges();

        // --------------------
        // PRODUCTPROJECTS
        // --------------------
        var productProject = new ProductProject
        {
            ProjectId = project.ProjectId,
            ProductId = product1.ProductId,
            Quantity = 2
        };

        db.ProductProjects.Add(productProject);
        db.SaveChanges();
    }
}