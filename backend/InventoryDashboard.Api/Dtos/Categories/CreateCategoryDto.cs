using System.ComponentModel.DataAnnotations;

public class CreateCategoryDto
{
    [Required]
    [StringLength(80)]
    public string Name { get; set; } = null!;
}