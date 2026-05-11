using System.ComponentModel.DataAnnotations;

namespace InventoryDashboard.Api.Dtos.Projects
{
    public class CreateProjectDto
    {
        
        [Required]
        [StringLength(150)]
        public string ProjectName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        // Optional: direkt Produkte fürs Projekt angeben
        public List<CreateProjectProductDto> Products { get; set; } = new();
    }

    public class CreateProjectProductDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
