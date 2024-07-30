using Shared.DTO.Category.Response;

namespace Shared.DTO.FeaturedCategories.Response;

public record GetFeaturedCategoriesResponse
{
    public Guid FeaturedCategoryId { get; set; }
    public int OrderNumber { get; set; }
    public bool IsActive { get; set; }
    public GetCategoriesResponse GetCategoryResponse { get; set; }
}