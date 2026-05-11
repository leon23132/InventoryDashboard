using InventoryDashboard.Api.Entities;

namespace InventoryDashboard.Api.Tests.TestData;

public static class DashboardTestData
{
    private const string Country = "Schweiz";

    public static Category CreateHardwareCategory()
    {
        return new Category
        {
            Name = "Hardware"
        };
    }

    public static Category CreateSoftwareCategory()
    {
        return new Category
        {
            Name = "Software"
        };
    }

    public static Supplier CreateSupplierA()
    {
        return new Supplier
        {
            CompanyName = "Supplier A",
            Email = "a@test.ch",
            PhoneNumber = "0441234567",
            Website = "https://supplier-a.ch",
            ContactPerson = "Max Muster",
            BillingAddress = new Address
            {
                StreetAddress = "Bahnhofstrasse 1",
                City = "Zürich",
                PostalCode = "8001",
                Country = Country
            }
        };
    }

    public static Supplier CreateSupplierB()
    {
        return new Supplier
        {
            CompanyName = "Supplier B",
            Email = "b@test.ch",
            PhoneNumber = "0447654321",
            Website = "https://supplier-b.ch",
            ContactPerson = "Anna Keller",
            BillingAddress = new Address
            {
                StreetAddress = "Marktgasse 10",
                City = "Bern",
                PostalCode = "3000",
                Country = Country
            }
        };
    }

    public static Supplier CreateTechSupplier()
    {
        return new Supplier
        {
            CompanyName = "Tech Supplier AG",
            Email = "info@tech.ch",
            PhoneNumber = "0441234567",
            Website = "https://tech.ch",
            ContactPerson = "Max Muster",
            BillingAddress = new Address
            {
                StreetAddress = "Bahnhofstrasse 1",
                City = "Zürich",
                PostalCode = "8001",
                Country = Country
            }
        };
    }

    public static Supplier CreateOfficeSupplier()
    {
        return new Supplier
        {
            CompanyName = "Office Supplier AG",
            Email = "info@office.ch",
            PhoneNumber = "0445556677",
            Website = "https://office.ch",
            ContactPerson = "Sandra Meier",
            BillingAddress = new Address
            {
                StreetAddress = "Büroweg 5",
                City = "Winterthur",
                PostalCode = "8400",
                Country = Country
            }
        };
    }

    public static Product CreateLaptop(int categoryId, int supplierId, int quantityInStock = 10, decimal price = 1500m)
    {
        return new Product
        {
            ProductTitle = "Laptop",
            ProductDescription = "Laptop Beschreibung",
            CategoryId = categoryId,
            SupplierId = supplierId,
            QuantityInStock = quantityInStock,
            Price = price,
            Location = "Regal A1"
        };
    }

    public static Product CreateMonitor(int categoryId, int supplierId, int quantityInStock = 5, decimal price = 300m)
    {
        return new Product
        {
            ProductTitle = "Monitor",
            ProductDescription = "Monitor Beschreibung",
            CategoryId = categoryId,
            SupplierId = supplierId,
            QuantityInStock = quantityInStock,
            Price = price,
            Location = "Regal A2"
        };
    }

    public static Product CreateMouse(int categoryId, int supplierId, int quantityInStock = 2, decimal price = 50m)
    {
        return new Product
        {
            ProductTitle = "Mouse",
            ProductDescription = "Mouse Beschreibung",
            CategoryId = categoryId,
            SupplierId = supplierId,
            QuantityInStock = quantityInStock,
            Price = price,
            Location = "Regal B1"
        };
    }

    public static Product CreateKeyboard(int categoryId, int supplierId, int quantityInStock = 4, decimal price = 80m)
    {
        return new Product
        {
            ProductTitle = "Keyboard",
            ProductDescription = "Keyboard Beschreibung",
            CategoryId = categoryId,
            SupplierId = supplierId,
            QuantityInStock = quantityInStock,
            Price = price,
            Location = "Regal B2"
        };
    }

    public static Product CreateOfficeProduct(int categoryId, int supplierId, int quantityInStock = 20, decimal price = 99m)
    {
        return new Product
        {
            ProductTitle = "Office 365",
            ProductDescription = "Office 365 Beschreibung",
            CategoryId = categoryId,
            SupplierId = supplierId,
            QuantityInStock = quantityInStock,
            Price = price,
            Location = "Regal C1"
        };
    }

    public static Product CreateCustomProduct(
        string title,
        int categoryId,
        int supplierId,
        int quantityInStock,
        decimal price)
    {
        return new Product
        {
            ProductTitle = title,
            ProductDescription = $"{title} Beschreibung",
            CategoryId = categoryId,
            SupplierId = supplierId,
            QuantityInStock = quantityInStock,
            Price = price,
            Location = "Regal X1"
        };
    }
}