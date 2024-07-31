using Shared.DTO.Category.Response;
using Shared.DTO.SubCategory.Response;

namespace Shared.DTO.Feature.Response;

public record GetFeatureResponse
{
    public Guid FeatureId { get; set; }
    public string FeatureName { get; set; }
    public string FeatureType { get; set; }
    public bool IsActive { get; set; }
    public GetCategoryResponse Category { get; set; }
    public GetSubCategoryResponse SubCategory { get; set; }
}