using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities;

public class Category : AuditTrailer
{
    [Key]
    public Guid CategoryId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Icon { get; set; }

    public int OrderNumber { get; set; }

    public string ImageHorizontalUrl { get; set; }

    public string ImageSquareUrl { get; set; }

    public Boolean IsActive { get; set; }

    // Relationship

    public ICollection<SubCategory> SubCategories { get; set; }
}