using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InventoryDashboard.Api.Dtos.Projects;

namespace InventoryDashboard.Api.Tests.Integration
{
    public class ProjectsApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProjectsApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            TestDbHelper.ResetDb(factory.Services);
        }


        [Fact]
        public async Task GetAll_Should_Return_Ok()
        {
            var response = await _client.GetAsync("/api/projects");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var items = await response.Content.ReadFromJsonAsync<List<ProjectListItemDTO>>();
            items.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_Should_Return_Project_When_Exists()
        {
            var response = await _client.GetAsync("/api/projects/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var project = await response.Content.ReadFromJsonAsync<ProjectDetailDto>();

            project.Should().NotBeNull();
            project!.ProjectId.Should().Be(1);
            project.ProjectName.Should().Be("Test Project");
            project.Products.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetById_Should_Return_NotFound_When_NotExists()
        {
            var response = await _client.GetAsync("/api/projects/9999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_Should_Return_Created()
        {
            var dto = new
            {
                projectName = "New Project",
                description = "Test",
                products = new[]
                {
                    new
                    {
                        productId = 1,
                        quantity = 2
                    }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/projects", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            result.Should().ContainKey("id");
        }


        [Fact]
        public async Task Create_Should_Return_Error_When_NoProducts()
        {
            var dto = new
            {
                projectName = "Invalid Project",
                products = new object[] { }
            };

            var response = await _client.PostAsJsonAsync("/api/projects", dto);

            // ❗ wegen throw ArgumentException
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Update_Should_Return_NoContent_When_Exists()
        {
            var dto = new
            {
                projectName = "Updated Project",
                description = "Updated",
                products = new[]
                {
                    new
                    {
                        productId = 1,
                        quantity = 5
                    }
                }
            };

            var response = await _client.PutAsJsonAsync("/api/projects/1", dto);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }


        [Fact]
        public async Task Update_Should_Return_NotFound_When_NotExists()
        {
            var dto = new
            {
                projectName = "Updated",
                products = new[]
                {
                    new
                    {
                        productId = 1,
                        quantity = 1
                    }
                }
            };

            var response = await _client.PutAsJsonAsync("/api/projects/9999", dto);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task Delete_Should_Return_NoContent_When_Exists()
        {
            var response = await _client.DeleteAsync("/api/projects/1");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }


        [Fact]
        public async Task Delete_Should_Return_NotFound_When_NotExists()
        {
            var response = await _client.DeleteAsync("/api/projects/9999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}