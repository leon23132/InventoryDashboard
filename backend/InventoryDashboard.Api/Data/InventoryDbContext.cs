using InventoryDashboard.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Api.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    // Tabellen
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProductProject> ProductProjects => Set<ProductProject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite Primary Key für Join-Tabelle
        modelBuilder.Entity<ProductProject>()
            .HasKey(pp => new { pp.ProductId, pp.ProjectId });

        modelBuilder.Entity<ProductProject>()
            .HasOne(pp => pp.Product)
            .WithMany(p => p.ProductProjects)
            .HasForeignKey(pp => pp.ProductId);

        modelBuilder.Entity<ProductProject>()
            .HasOne(pp => pp.Project)
            .WithMany(p => p.ProductProjects)
            .HasForeignKey(pp => pp.ProjectId);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.ProductTitle);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.CategoryId);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SupplierId);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Supplier>()
            .HasIndex(s => s.CompanyName);

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.ProjectName);
    }
}
