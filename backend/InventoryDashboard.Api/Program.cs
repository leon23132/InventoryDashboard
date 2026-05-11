using InventoryDashboard.Api.Data;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using InventoryDashboard.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventoryDb")));

builder.Services.AddControllers();

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<SuppliersService>();
builder.Services.AddScoped<ProjectsService>();
builder.Services.AddScoped<DashboardService>();


builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://192.168.1.247:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json

    //Scalar UI
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("InventoryDashboard API");
        options.WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    DbSeeder.Seed(db);
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { }