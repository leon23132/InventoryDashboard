using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InventoryDashboard.Api.Dtos.Suppliers;

namespace InventoryDashboard.Api.Tests.Integration
{
    public class SuppliersApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SuppliersApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            TestDbHelper.ResetDb(factory.Services);
        }


        [Fact]
        public async Task GetAll_Should_Return_Ok()
        {
            var response = await _client.GetAsync("/api/suppliers");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var items = await response.Content.ReadFromJsonAsync<List<SupplierListItemDto>>();
            items.Should().NotBeNull();
            items!.Count.Should().BeGreaterThan(0);
        }


        [Fact]
        public async Task GetById_Should_Return_Supplier_When_Exists()
        {
            var response = await _client.GetAsync("/api/suppliers/2");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var supplier = await response.Content.ReadFromJsonAsync<SupplierDetailDto>();

            supplier.Should().NotBeNull();
            supplier!.SupplierId.Should().Be(2);

            supplier.CompanyName.Should().Be("Office GmbH");
        }


        [Fact]
        public async Task GetById_Should_Return_NotFound_When_NotExists()
        {
            var response = await _client.GetAsync("/api/suppliers/9999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_Should_Return_Created()
        {
            var dto = new
            {
                companyName = "New Supplier",
                email = "new@test.ch",
                phoneNumber = "+41790000000",
                website = "https://test.ch",
                contactPerson = "John Doe",
                billingAddress = new
                {
                    streetAddress = "Teststrasse 1",
                    city = "Zürich",
                    postalCode = "8000",
                    country = "CH"
                },
                shippingAddress = new
                {
                    streetAddress = "Teststrasse 2",
                    city = "Zürich",
                    postalCode = "8000",
                    country = "CH"
                }
            };

            var response = await _client.PostAsJsonAsync("/api/suppliers", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            result.Should().ContainKey("id");
        }

        [Fact]
        public async Task Create_Should_Return_BadRequest_When_Invalid()
        {
            var dto = new
            {
                companyName = "", // invalid
            };

            var response = await _client.PostAsJsonAsync("/api/suppliers", dto);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Update_Should_Return_NoContent_When_Exists()
        {
            var dto = new
            {
                companyName = "Updated Supplier",
                email = "updated@test.ch",
                phoneNumber = "+41791111111",
                website = "https://updated.ch",
                contactPerson = "Updated Person",
                billingAddress = new
                {
                    streetAddress = "Neue Strasse 1",
                    city = "Bern",
                    postalCode = "3000",
                    country = "CH"
                },
                shippingAddress = new
                {
                    streetAddress = "Neue Strasse 2",
                    city = "Bern",
                    postalCode = "3000",
                    country = "CH"
                }
            };

            var response = await _client.PutAsJsonAsync("/api/suppliers/1", dto);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }


        [Fact]
        public async Task Update_Should_Return_NotFound_When_NotExists()
        {
            var dto = new
            {
                companyName = "Updated Supplier",
                email = "updated@test.ch", // 🔥 wichtig (sonst 400!)
                billingAddress = new
                {
                    streetAddress = "Test",
                    city = "Test",
                    postalCode = "0000",
                    country = "CH"
                }
            };

            var response = await _client.PutAsJsonAsync("/api/suppliers/9999", dto);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task Delete_Should_Return_NoContent_When_Exists()
        {
            var response = await _client.DeleteAsync("/api/suppliers/1");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }


        [Fact]
        public async Task Delete_Should_Return_NotFound_When_NotExists()
        {
            var response = await _client.DeleteAsync("/api/suppliers/9999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}