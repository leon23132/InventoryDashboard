using InventoryDashboard.Api.Dtos.Addresses;
using InventoryDashboard.Api.Dtos.Suppliers;
using InventoryDashboard.Api.Entities;

namespace InventoryDashboard.Api.Tests.TestData;

public static class SupplierTestData
{
    private const string Country = "Schweiz";

    public static CreateSupplierDto CreateSupplierDto(
        string companyName = "Muster AG",
        string email = "info@muster.ch",
        string phoneNumber = "0441234567",
        string website = "https://muster.ch",
        string contactPerson = "Max Muster",
        string billingStreetAddress = "Bahnhofstrasse 1",
        string billingCity = "Zürich",
        string billingPostalCode = "8001",
        string shippingStreetAddress = "Lagerstrasse 5",
        string shippingCity = "Winterthur",
        string shippingPostalCode = "8400")
    {
        return new CreateSupplierDto
        {
            CompanyName = companyName,
            Email = email,
            PhoneNumber = phoneNumber,
            Website = website,
            ContactPerson = contactPerson,
            BillingAddress = new CreateAdressDto
            {
                StreetAddress = billingStreetAddress,
                City = billingCity,
                PostalCode = billingPostalCode,
                Country = Country
            },
            ShippingAddress = new CreateAdressDto
            {
                StreetAddress = shippingStreetAddress,
                City = shippingCity,
                PostalCode = shippingPostalCode,
                Country = Country
            }
        };
    }

    public static CreateSupplierDto CreateSupplierDtoWithoutShipping(
        string companyName = "Ohne Versand AG",
        string email = "info@ohneversand.ch",
        string phoneNumber = "0440000000",
        string website = "https://ohneversand.ch",
        string contactPerson = "Anna Keller",
        string billingStreetAddress = "Hauptstrasse 10",
        string billingCity = "Bern",
        string billingPostalCode = "3000")
    {
        return new CreateSupplierDto
        {
            CompanyName = companyName,
            Email = email,
            PhoneNumber = phoneNumber,
            Website = website,
            ContactPerson = contactPerson,
            BillingAddress = new CreateAdressDto
            {
                StreetAddress = billingStreetAddress,
                City = billingCity,
                PostalCode = billingPostalCode,
                Country = Country
            },
            ShippingAddress = null
        };
    }

    public static UpdateSupplierDto UpdateSupplierDto(
        string companyName = "Neu AG",
        string email = "neu@firma.ch",
        string phoneNumber = "0441112233",
        string website = "https://neu.ch",
        string contactPerson = "Neu Person",
        string billingStreetAddress = "Neuweg 2",
        string billingCity = "Zürich",
        string billingPostalCode = "8001",
        string shippingStreetAddress = "Lagerweg 3",
        string shippingCity = "Winterthur",
        string shippingPostalCode = "8400")
    {
        return new UpdateSupplierDto
        {
            CompanyName = companyName,
            Email = email,
            PhoneNumber = phoneNumber,
            Website = website,
            ContactPerson = contactPerson,
            BillingAddress = new UpdateAddressDto
            {
                StreetAddress = billingStreetAddress,
                City = billingCity,
                PostalCode = billingPostalCode,
                Country = Country
            },
            ShippingAddress = new UpdateAddressDto
            {
                StreetAddress = shippingStreetAddress,
                City = shippingCity,
                PostalCode = shippingPostalCode,
                Country = Country
            }
        };
    }

    public static Supplier CreateSupplierEntity(
        string companyName = "TechTrade GmbH",
        string email = "mail@techtrade.ch",
        string phoneNumber = "0431112233",
        string website = "https://techtrade.ch",
        string contactPerson = "Peter Meier",
        string billingStreetAddress = "Industriestrasse 2",
        string billingCity = "Basel",
        string billingPostalCode = "4000",
        string shippingStreetAddress = "Versandweg 8",
        string shippingCity = "Luzern",
        string shippingPostalCode = "6000")
    {
        return new Supplier
        {
            CompanyName = companyName,
            Email = email,
            PhoneNumber = phoneNumber,
            Website = website,
            ContactPerson = contactPerson,
            BillingAddress = new Address
            {
                StreetAddress = billingStreetAddress,
                City = billingCity,
                PostalCode = billingPostalCode,
                Country = Country
            },
            ShippingAddress = new Address
            {
                StreetAddress = shippingStreetAddress,
                City = shippingCity,
                PostalCode = shippingPostalCode,
                Country = Country
            }
        };
    }

    public static Supplier CreateSupplierEntityWithoutShipping(
        string companyName = "Ohne Versand AG",
        string email = "info@ohneversand.ch",
        string phoneNumber = "0440000000",
        string website = "https://ohneversand.ch",
        string contactPerson = "Anna Keller",
        string billingStreetAddress = "Hauptstrasse 10",
        string billingCity = "Bern",
        string billingPostalCode = "3000")
    {
        return new Supplier
        {
            CompanyName = companyName,
            Email = email,
            PhoneNumber = phoneNumber,
            Website = website,
            ContactPerson = contactPerson,
            BillingAddress = new Address
            {
                StreetAddress = billingStreetAddress,
                City = billingCity,
                PostalCode = billingPostalCode,
                Country = Country
            },
            ShippingAddress = null
        };
    }

    public static Supplier CreateSupplierEntityForSearch(
        string companyName,
        string contactPerson,
        string city)
    {
        return new Supplier
        {
            CompanyName = companyName,
            Email = $"{companyName.Replace(" ", "").ToLower()}@test.ch",
            PhoneNumber = "0441112233",
            Website = "https://test.ch",
            ContactPerson = contactPerson,
            BillingAddress = new Address
            {
                StreetAddress = "Teststrasse 1",
                City = city,
                PostalCode = "8000",
                Country = Country
            },
            ShippingAddress = null
        };
    }
}