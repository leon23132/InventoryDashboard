using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InventoryDashboard.Api.Dtos.Dashboard;

namespace InventoryDashboard.Api.Tests.Integration
{
    public class DashboardApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public DashboardApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            TestDbHelper.ResetDb(factory.Services);
            
        }

   
        [Fact]
        public async Task GetOverview_Should_Return_Correct_Data()
        {
            var response = await _client.GetAsync("/api/dashboard/overview");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<DashboardOverviewDto>();

            dto.Should().NotBeNull();

            // Totals
            dto!.TotalProducts.Should().Be(3);
            dto.TotalCategories.Should().Be(2);
            dto.TotalSuppliers.Should().Be(2);

            // Low stock (<=5)
            dto.LowStockThreshold.Should().Be(5);
            dto.LowStockCount.Should().Be(1);

            // Collections
            dto.ProductsPerCategory.Should().NotBeEmpty();
            dto.ProductsPerSupplier.Should().NotBeEmpty();
            dto.TopProductsByStock.Should().NotBeEmpty();
        }


        [Fact]
        public async Task GetOverview_Should_Respect_Threshold()
        {
            var response = await _client.GetAsync("/api/dashboard/overview?lowStockThreshold=50");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<DashboardOverviewDto>();

            dto.Should().NotBeNull();

            // Jetzt zählen mehr Produkte als low stock
            dto!.LowStockThreshold.Should().Be(50);
            dto.LowStockCount.Should().BeGreaterThan(1);
        }


        [Fact]
        public async Task GetOverview_Should_Return_BadRequest_When_Invalid_Threshold()
        {
            var response = await _client.GetAsync("/api/dashboard/overview?lowStockThreshold=-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}