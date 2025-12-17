using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryDashboard.Api.Entities
{
    public class Project
    {
         [Key]
    public int ProjectId { get; set; }

    [Required]
    [StringLength(150)]
    public string ProjectName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public ICollection<ProductProject> ProductProjects { get; set; } = new List<ProductProject>();
    }
}