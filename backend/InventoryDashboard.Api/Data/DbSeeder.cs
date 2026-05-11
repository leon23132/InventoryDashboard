using InventoryDashboard.Api.Entities;

namespace InventoryDashboard.Api.Data
{
    public static class DbSeeder
    {
        public static void Seed(InventoryDbContext db)
        {
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

            var address3 = new Address
            {
                StreetAddress = "Marktgasse 8",
                City = "Bern",
                PostalCode = "3000",
                Country = "Schweiz"
            };

            var address4 = new Address
            {
                StreetAddress = "Seestrasse 45",
                City = "Luzern",
                PostalCode = "6003",
                Country = "Schweiz"
            };

            var address5 = new Address
            {
                StreetAddress = "Hauptstrasse 99",
                City = "St. Gallen",
                PostalCode = "9000",
                Country = "Schweiz"
            };

            db.Addresses.AddRange(address1, address2, address3, address4, address5);
            db.SaveChanges();

            // --------------------
            // 2) Categories
            // --------------------
            var cat1 = new Category { Name = "Elektronik" };
            var cat2 = new Category { Name = "Büromaterial" };
            var cat3 = new Category { Name = "Netzwerk" };
            var cat4 = new Category { Name = "Möbel" };
            var cat5 = new Category { Name = "Speicher" };

            db.Categories.AddRange(cat1, cat2, cat3, cat4, cat5);
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

            var supplier3 = new Supplier
            {
                CompanyName = "NetCom Solutions AG",
                BillingAddressId = address3.AddressId,
                ShippingAddressId = address3.AddressId,
                ContactPerson = "Daniel Frei",
                Email = "sales@netcom.ch",
                PhoneNumber = "+41 31 555 12 34",
                Website = "https://netcom.ch"
            };

            var supplier4 = new Supplier
            {
                CompanyName = "SwissFurniture AG",
                BillingAddressId = address4.AddressId,
                ShippingAddressId = address4.AddressId,
                ContactPerson = "Sandra Keller",
                Email = "kontakt@swissfurniture.ch",
                PhoneNumber = "+41 41 222 33 44",
                Website = "https://swissfurniture.ch"
            };

            var supplier5 = new Supplier
            {
                CompanyName = "StorageWorld GmbH",
                BillingAddressId = address5.AddressId,
                ShippingAddressId = address5.AddressId,
                ContactPerson = "Marco Schmid",
                Email = "info@storageworld.ch",
                PhoneNumber = "+41 71 888 77 66",
                Website = "https://storageworld.ch"
            };

            db.Suppliers.AddRange(supplier1, supplier2, supplier3, supplier4, supplier5);
            db.SaveChanges();

            // --------------------
            // 4) Projects
            // --------------------
            var project1 = new Project { ProjectName = "Neues Büro Zürich", Description = "Ausstattung eines neuen Büros" };
            var project2 = new Project { ProjectName = "IT-Erneuerung 2025", Description = "Erneuerung der IT-Infrastruktur" };
            var project3 = new Project { ProjectName = "Filiale Bern", Description = "Neue Filiale einrichten" };
            var project4 = new Project { ProjectName = "Home Office Setup", Description = "Ausstattung für Home Office" };
            var project5 = new Project { ProjectName = "Gaming Area", Description = "Gaming Setup" };

            db.Projects.AddRange(project1, project2, project3, project4, project5);
            db.SaveChanges();

            // --------------------
            // 5) Products
            // --------------------
            var product1 = new Product { ProductTitle = "Laptop Dell XPS", ProductDescription = "Business Laptop", CategoryId = cat1.CategoryId, SupplierId = supplier1.SupplierId, Price = 1499, QuantityInStock = 10, Location = "A1" };
            var product2 = new Product { ProductTitle = "MacBook Pro", ProductDescription = "Apple Laptop", CategoryId = cat1.CategoryId, SupplierId = supplier1.SupplierId, Price = 2499, QuantityInStock = 5, Location = "A2" };
            var product3 = new Product { ProductTitle = "Gaming PC", ProductDescription = "High-end PC", CategoryId = cat1.CategoryId, SupplierId = supplier1.SupplierId, Price = 1999, QuantityInStock = 3, Location = "A3" };
            var product4 = new Product { ProductTitle = "Monitor 27 Zoll", ProductDescription = "QHD Display", CategoryId = cat1.CategoryId, SupplierId = supplier1.SupplierId, Price = 299, QuantityInStock = 20, Location = "A4" };
            var product5 = new Product { ProductTitle = "Keyboard", ProductDescription = "Mechanical Keyboard", CategoryId = cat1.CategoryId, SupplierId = supplier2.SupplierId, Price = 120, QuantityInStock = 15, Location = "A5" };

            var product6 = new Product { ProductTitle = "Mouse", ProductDescription = "Wireless Mouse", CategoryId = cat1.CategoryId, SupplierId = supplier2.SupplierId, Price = 60, QuantityInStock = 25, Location = "A6" };
            var product7 = new Product { ProductTitle = "Headset", ProductDescription = "Noise Cancelling", CategoryId = cat1.CategoryId, SupplierId = supplier2.SupplierId, Price = 180, QuantityInStock = 8, Location = "A7" };
            var product8 = new Product { ProductTitle = "Webcam", ProductDescription = "Full HD Webcam", CategoryId = cat1.CategoryId, SupplierId = supplier2.SupplierId, Price = 90, QuantityInStock = 12, Location = "A8" };
            var product9 = new Product { ProductTitle = "Docking Station", ProductDescription = "USB-C Dock", CategoryId = cat1.CategoryId, SupplierId = supplier1.SupplierId, Price = 150, QuantityInStock = 7, Location = "A9" };
            var product10 = new Product { ProductTitle = "USB Hub", ProductDescription = "Adapter", CategoryId = cat1.CategoryId, SupplierId = supplier1.SupplierId, Price = 50, QuantityInStock = 30, Location = "A10" };

            var product11 = new Product { ProductTitle = "Bürostuhl", ProductDescription = "Ergonomisch", CategoryId = cat2.CategoryId, SupplierId = supplier2.SupplierId, Price = 200, QuantityInStock = 15, Location = "B1" };
            var product12 = new Product { ProductTitle = "Schreibtisch", ProductDescription = "Höhenverstellbar", CategoryId = cat4.CategoryId, SupplierId = supplier4.SupplierId, Price = 500, QuantityInStock = 4, Location = "B2" };
            var product13 = new Product { ProductTitle = "Lampe", ProductDescription = "LED", CategoryId = cat2.CategoryId, SupplierId = supplier2.SupplierId, Price = 80, QuantityInStock = 18, Location = "B3" };
            var product14 = new Product { ProductTitle = "Drucker", ProductDescription = "Laser", CategoryId = cat2.CategoryId, SupplierId = supplier2.SupplierId, Price = 250, QuantityInStock = 6, Location = "B4" };
            var product15 = new Product { ProductTitle = "Scanner", ProductDescription = "Dokument", CategoryId = cat2.CategoryId, SupplierId = supplier2.SupplierId, Price = 180, QuantityInStock = 5, Location = "B5" };

            var product16 = new Product { ProductTitle = "Tablet", ProductDescription = "10 Zoll", CategoryId = cat1.CategoryId, SupplierId = supplier1.SupplierId, Price = 350, QuantityInStock = 9, Location = "C1" };
            var product17 = new Product { ProductTitle = "Smartphone", ProductDescription = "Android", CategoryId = cat1.CategoryId, SupplierId = supplier1.SupplierId, Price = 900, QuantityInStock = 11, Location = "C2" };
            var product18 = new Product { ProductTitle = "SSD 1TB", ProductDescription = "Storage", CategoryId = cat5.CategoryId, SupplierId = supplier5.SupplierId, Price = 160, QuantityInStock = 20, Location = "C3" };
            var product19 = new Product { ProductTitle = "Router", ProductDescription = "WiFi 6", CategoryId = cat3.CategoryId, SupplierId = supplier3.SupplierId, Price = 130, QuantityInStock = 14, Location = "C4" };
            var product20 = new Product { ProductTitle = "Switch", ProductDescription = "8-Port", CategoryId = cat3.CategoryId, SupplierId = supplier3.SupplierId, Price = 70, QuantityInStock = 22, Location = "C5" };

            var product21 = new Product { ProductTitle = "NAS Server", ProductDescription = "Netzwerkspeicher", CategoryId = cat3.CategoryId, SupplierId = supplier3.SupplierId, Price = 799, QuantityInStock = 4, Location = "D1" };
            var product22 = new Product { ProductTitle = "Access Point", ProductDescription = "WLAN Access Point", CategoryId = cat3.CategoryId, SupplierId = supplier3.SupplierId, Price = 189, QuantityInStock = 9, Location = "D2" };
            var product23 = new Product { ProductTitle = "Aktenschrank", ProductDescription = "Metall Aktenschrank", CategoryId = cat4.CategoryId, SupplierId = supplier4.SupplierId, Price = 320, QuantityInStock = 7, Location = "D3" };
            var product24 = new Product { ProductTitle = "Besprechungstisch", ProductDescription = "Tisch für 8 Personen", CategoryId = cat4.CategoryId, SupplierId = supplier4.SupplierId, Price = 890, QuantityInStock = 2, Location = "D4" };
            var product25 = new Product { ProductTitle = "Externe HDD 4TB", ProductDescription = "Backup Speicher", CategoryId = cat5.CategoryId, SupplierId = supplier5.SupplierId, Price = 140, QuantityInStock = 16, Location = "D5" };
            var product26 = new Product { ProductTitle = "USB Stick 128GB", ProductDescription = "Mobiler Speicher", CategoryId = cat5.CategoryId, SupplierId = supplier5.SupplierId, Price = 25, QuantityInStock = 40, Location = "D6" };

            db.Products.AddRange(
                product1, product2, product3, product4, product5,
                product6, product7, product8, product9, product10,
                product11, product12, product13, product14, product15,
                product16, product17, product18, product19, product20,
                product21, product22, product23, product24, product25, product26
            );

            db.SaveChanges();

            // --------------------
            // 6) ProductProjects
            // --------------------
            db.ProductProjects.AddRange(
                new ProductProject { ProductId = product1.ProductId, ProjectId = project1.ProjectId, Quantity = 5 },
                new ProductProject { ProductId = product2.ProductId, ProjectId = project1.ProjectId, Quantity = 5 },
                new ProductProject { ProductId = product3.ProductId, ProjectId = project2.ProjectId, Quantity = 10 },
                new ProductProject { ProductId = product11.ProductId, ProjectId = project3.ProjectId, Quantity = 8 },
                new ProductProject { ProductId = product12.ProductId, ProjectId = project3.ProjectId, Quantity = 4 },
                new ProductProject { ProductId = product16.ProductId, ProjectId = project4.ProjectId, Quantity = 3 },
                new ProductProject { ProductId = product17.ProductId, ProjectId = project4.ProjectId, Quantity = 6 },
                new ProductProject { ProductId = product5.ProductId, ProjectId = project5.ProjectId, Quantity = 2 },
                new ProductProject { ProductId = product19.ProductId, ProjectId = project2.ProjectId, Quantity = 6 },
                new ProductProject { ProductId = product21.ProductId, ProjectId = project2.ProjectId, Quantity = 2 },
                new ProductProject { ProductId = product23.ProductId, ProjectId = project1.ProjectId, Quantity = 3 },
                new ProductProject { ProductId = product24.ProductId, ProjectId = project3.ProjectId, Quantity = 1 },
                new ProductProject { ProductId = product25.ProductId, ProjectId = project4.ProjectId, Quantity = 5 }
            );

            db.SaveChanges();
        }
    }
}