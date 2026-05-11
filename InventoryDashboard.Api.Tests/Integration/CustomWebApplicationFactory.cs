using InventoryDashboard.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryDashboard.Api.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly InMemoryDatabaseRoot _dbRoot = new();
        private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptors = services.ToList();

                foreach (var d in descriptors)
                {
                    if (d.ServiceType.FullName != null &&
                        d.ServiceType.FullName.Contains("Microsoft.EntityFrameworkCore"))
                    {
                        services.Remove(d);
                    }
                }

                services.AddDbContext<InventoryDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName, _dbRoot);
                });
            });
        }
    }
}