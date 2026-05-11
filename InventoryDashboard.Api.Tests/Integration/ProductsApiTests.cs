using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InventoryDashboard.Api.Dtos.Products;

namespace InventoryDashboard.Api.Tests.Integration
{
    public class ProductsApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductsApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            TestDbHelper.ResetDb(factory.Services);
        }


        [Fact]
        public async Task GetAll_Should_Return_Ok()
        {
            var response = await _client.GetAsync("/api/products");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var items = await response.Content.ReadFromJsonAsync<List<ProductListItemDto>>();
            items.Should().NotBeNull();
            items!.Count.Should().BeGreaterThan(0);
        }

   
        [Fact]
        public async Task GetById_Should_Return_Product_When_Exists()
        {
            var response = await _client.GetAsync("/api/products/3");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var product = await response.Content.ReadFromJsonAsync<ProductDetailDto>();

            product.Should().NotBeNull();
            product!.ProductId.Should().Be(3);

            
            product.ProductTitle.Should().Be("Papier");
        }

  
        [Fact]
        public async Task GetById_Should_Return_NotFound_When_NotExists()
        {
            var response = await _client.GetAsync("/api/products/9999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task Create_Should_Return_Created()
        {
            var dto = new
            {
                productTitle = "New Product",
                productDescription = "Test",
                categoryId = 1,
                supplierId = 1,
                price = 50,
                quantityInStock = 5,
                location = "Regal B1"
            };

            var response = await _client.PostAsJsonAsync("/api/products", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            result.Should().ContainKey("id");
        }


        [Fact]
        public async Task Create_Should_Return_BadRequest_When_Invalid()
        {
            var dto = new
            {
                productTitle = "", // invalid
                categoryId = 1,
                supplierId = 1,
                price = -10,
                quantityInStock = -5
            };

            var response = await _client.PostAsJsonAsync("/api/products", dto);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Update_Should_Return_NoContent_When_Exists()
        {
            var dto = new
            {
                productTitle = "Updated Product",
                productDescription = "Updated",
                categoryId = 1,
                supplierId = 1,
                price = 99,
                quantityInStock = 10,
                location = "Regal C1"
            };

            var response = await _client.PutAsJsonAsync("/api/products/1", dto);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

 
        [Fact]
        public async Task Update_Should_Return_NotFound_When_NotExists()
        {
            var dto = new
            {
                productTitle = "Updated",
                productDescription = "Updated",
                categoryId = 1,
                supplierId = 1,
                price = 10,
                quantityInStock = 1,
                location = "X"
            };

            var response = await _client.PutAsJsonAsync("/api/products/9999", dto);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task Delete_Should_Return_NoContent_When_Exists()
        {
            var response = await _client.DeleteAsync("/api/products/1");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }


        [Fact]
        public async Task Delete_Should_Return_NotFound_When_NotExists()
        {
            var response = await _client.DeleteAsync("/api/products/9999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}