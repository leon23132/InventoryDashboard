using System.ComponentModel.DataAnnotations;

public class ProjectProductDto
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    public string ProductTitle { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal TotalPrice => UnitPrice * Quantity;

}
