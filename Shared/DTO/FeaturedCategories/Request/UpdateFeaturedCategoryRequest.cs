namespace Shared.DTO.FeaturedCategories.Request;

public record UpdateFeaturedCategoryRequest(
    Guid FeaturedCategoryId,
    Guid CategoryId,
    int OrderNumber,
    bool IsActive
    );