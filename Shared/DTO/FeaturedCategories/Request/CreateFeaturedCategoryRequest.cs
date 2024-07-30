namespace Shared.DTO.FeaturedCategories.Request;

public record CreateFeaturedCategoryRequest(
    Guid CategoryId,
    int OrderNumber,
    bool IsActive
    );