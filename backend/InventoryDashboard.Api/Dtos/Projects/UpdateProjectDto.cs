using System.ComponentModel.DataAnnotations;

namespace InventoryDashboard.Api.Dtos.Projects
{
    public class UpdateProjectDto
    {
        [Required]
        [StringLength(150)]
        public string ProjectName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        // Wenn du beim Update die Produktliste ersetzen willst:
        public List<UpdateProjectProductDto> Products { get; set; } = new();
    }

    public class UpdateProjectProductDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
