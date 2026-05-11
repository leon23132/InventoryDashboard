using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace InventoryDashboard.Api.Tests.Integration
{
    public class CategoriesApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CategoriesApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            TestDbHelper.ResetDb(factory.Services);
        }


        [Fact]
        public async Task GetAll_Should_Return_Ok()
        {
            var response = await _client.GetAsync("/api/categories");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var items = await response.Content.ReadFromJsonAsync<List<CategoryListItemDto>>();
            items.Should().NotBeNull();
            items!.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetById_Should_Return_Category_When_Exists()
        {
            var response = await _client.GetAsync("/api/categories/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var category = await response.Content.ReadFromJsonAsync<CategoryDetailDto>();

            category.Should().NotBeNull();
            category!.CategoryId.Should().Be(1);
            category.Name.Should().Be("Elektronik");
        }

        [Fact]
        public async Task GetById_Should_Return_NotFound_When_NotExists()
        {
            var response = await _client.GetAsync("/api/categories/9999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task Create_Should_Return_Created()
        {
            var dto = new
            {
                name = "Neue Kategorie"
            };

            var response = await _client.PostAsJsonAsync("/api/categories", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            result.Should().ContainKey("id");
        }


        [Fact]
        public async Task Create_Should_Return_BadRequest_When_Invalid()
        {
            var dto = new
            {
                name = "" // invalid (Required + StringLength)
            };

            var response = await _client.PostAsJsonAsync("/api/categories", dto);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Update_Should_Return_NoContent_When_Exists()
        {
            var dto = new
            {
                categoryId = 1,
                name = "Updated Category"
            };

            var response = await _client.PutAsJsonAsync("/api/categories/1", dto);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Update_Should_Return_NotFound_When_NotExists()
        {
            var dto = new
            {
                categoryId = 9999,
                name = "Updated Category"
            };

            var response = await _client.PutAsJsonAsync("/api/categories/9999", dto);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task Delete_Should_Return_NoContent_When_Exists()
        {
            var response = await _client.DeleteAsync("/api/categories/2");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_Should_Return_NotFound_When_NotExists()
        {
            var response = await _client.DeleteAsync("/api/categories/9999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}