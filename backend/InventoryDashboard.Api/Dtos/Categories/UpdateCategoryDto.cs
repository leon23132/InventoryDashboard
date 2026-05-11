using System.ComponentModel.DataAnnotations;

public class UpdateCategoryDto
{
    [Required]
    public int CategoryId { get; set; }
    [Required]
    [StringLength(80)]
    public string Name { get; set; } = null!;
}