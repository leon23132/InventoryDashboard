using System.ComponentModel.DataAnnotations;

namespace InventoryDashboard.Api.Dtos.Projects
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }

        [Required]
        [StringLength(150)]
        public string ProjectName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
