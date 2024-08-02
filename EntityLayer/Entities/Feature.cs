using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities;

public class Feature
{
    [Key]
    public Guid FeatureId { get; set; }
    public string FeatureName { get; set; }
    public string FeatureType { get; set; }
    public bool IsActive { get; set; }
    
    public Guid CategoryId { get; set; }
    public Guid SubCategoryId { get; set; }
    
    public Category Category { get; set; }
    public SubCategory SubCategory { get; set; }
}