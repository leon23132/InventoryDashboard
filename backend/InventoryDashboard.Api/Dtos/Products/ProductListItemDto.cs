namespace InventoryDashboard.Api.Dtos.Products
{
    public class ProductListItemDto
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string? CategoryName { get; set; }
        public string? SupplierName { get; set; }

        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public int MinimumStock { get; set; }
        public string? Location { get; set; }

    }
}