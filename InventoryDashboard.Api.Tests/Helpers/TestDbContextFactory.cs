using InventoryDashboard.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Api.Tests.Helpers;

public static class TestDbContextFactory
{
    public static InventoryDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new InventoryDbContext(options);

        // sorgt dafür, dass das Schema erstellt wird
        context.Database.EnsureCreated();

        return context;
    }
}