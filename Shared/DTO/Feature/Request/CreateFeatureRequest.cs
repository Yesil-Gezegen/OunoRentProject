namespace Shared.DTO.Feature.Request;

public record CreateFeatureRequest(
    string FeatureName,
    string FeatureType,
    Guid CategoryId,
    Guid SubCategoryId,
    bool IsActive
);