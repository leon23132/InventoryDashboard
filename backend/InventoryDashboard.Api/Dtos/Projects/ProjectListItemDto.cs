using System.ComponentModel.DataAnnotations;
using InventoryDashboard.Api.Entities;

public class ProjectListItemDTO
{

    public int ProjectId { get; set; }

    [Required]
    [StringLength(150)]
    public string ProjectName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public List<ProjectProductDto> Products { get; set; } = new();
}