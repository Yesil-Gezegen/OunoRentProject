using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities;

public class FeaturedCategory : AuditTrailer
{
    [Key]
    public Guid FeaturedCategoryId { get; set; }
    public int OrderNumber { get; set; }
    public bool IsActive { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public Category Category { get; set; }
}
