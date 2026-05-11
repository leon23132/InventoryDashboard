using InventoryDashboard.Api.Data;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryDashboard.Api.Tests.Integration
{
    public static class TestDbHelper
    {
        public static void ResetDb(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            TestDataSeeder.Seed(db);
        }
    }
}