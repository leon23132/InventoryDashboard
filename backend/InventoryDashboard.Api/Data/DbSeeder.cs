using InventoryDashboard.Api.Entities;

namespace InventoryDashboard.Api.Data
{
    public static class DbSeeder
    {
        public static void Seed(InventoryDbContext db)
        {
            // DB bereits gefüllt? → nichts machen
            if (db.Products.Any())
                return;

            // --------------------
            // 1) Addresses
            // --------------------
            var address1 = new Address
            {
                StreetAddress = "Bahnhofstrasse 1",
                City = "Zürich",
                PostalCode = "8001",
                Country = "Schweiz"
            };

            var address2 = new Address
            {
                StreetAddress = "Industriestrasse 12",
                City = "Winterthur",
                PostalCode = "8400",
                Country = "Schweiz"
            };

            db.Addresses.AddRange(address1, address2);
            db.SaveChanges();

            // --------------------
            // 2) Categories
            // --------------------
            var cat1 = new Category { Name = "Elektronik" };
            var cat2 = new Category { Name = "Büromaterial" };

            db.Categories.AddRange(cat1, cat2);
            db.SaveChanges();

            // --------------------
            // 3) Suppliers
            // --------------------
            var supplier1 = new Supplier
            {
                CompanyName = "TechSupplier AG",
                BillingAddressId = address1.AddressId,
                ShippingAddressId = address1.AddressId,
                ContactPerson = "Max Müller",
                Email = "kontakt@techsupplier.ch",
                PhoneNumber = "+41 44 123 45 67",
                Website = "https://techsupplier.ch"
            };

            var supplier2 = new Supplier
            {
                CompanyName = "OfficePro GmbH",
                BillingAddressId = address2.AddressId,
                ShippingAddressId = address2.AddressId,
                ContactPerson = "Laura Meier",
                Email = "info@officepro.ch",
                PhoneNumber = "+41 52 987 65 43",
                Website = "https://officepro.ch"
            };

            db.Suppliers.AddRange(supplier1, supplier2);
            db.SaveChanges();

            // --------------------
            // 4) Projects
            // --------------------
            var project1 = new Project
            {
                ProjectName = "Neues Büro Zürich",
                Description = "Ausstattung eines neuen Büros"
            };

            var project2 = new Project
            {
                ProjectName = "IT-Erneuerung 2025",
                Description = "Erneuerung der IT-Infrastruktur"
            };

            db.Projects.AddRange(project1, project2);
            db.SaveChanges();

            // --------------------
            // 5) Products
            // --------------------
            var product1 = new Product
            {
                ProductTitle = "Laptop Dell XPS",
                ProductDescription = "13 Zoll Business Laptop",
                CategoryId = cat1.CategoryId,
                SupplierId = supplier1.SupplierId,
                Price = 1499.00m,
                QuantityInStock = 10,
                Location = "Regal A1"
            };

            var product2 = new Product
            {
                ProductTitle = "27 Zoll Monitor",
                ProductDescription = "QHD Monitor",
                CategoryId = cat1.CategoryId,
                SupplierId = supplier1.SupplierId,
                Price = 299.00m,
                QuantityInStock = 20,
                Location = "Regal B2"
            };

            var product3 = new Product
            {
                ProductTitle = "Bürostuhl",
                ProductDescription = "Ergonomischer Bürostuhl",
                CategoryId = cat2.CategoryId,
                SupplierId = supplier2.SupplierId,
                Price = 199.90m,
                QuantityInStock = 15,
                Location = "Lager Nord"
            };

            db.Products.AddRange(product1, product2, product3);
            db.SaveChanges();

            // --------------------
            // 6) ProductProjects
            // --------------------
            db.ProductProjects.AddRange(
                new ProductProject
                {
                    ProductId = product1.ProductId,
                    ProjectId = project1.ProjectId,
                    Quantity = 5
                },
                new ProductProject
                {
                    ProductId = product2.ProductId,
                    ProjectId = project1.ProjectId,
                    Quantity = 5
                },
                new ProductProject
                {
                    ProductId = product3.ProductId,
                    ProjectId = project2.ProjectId,
                    Quantity = 10
                }
            );

            db.SaveChanges();
        }
    }
}
