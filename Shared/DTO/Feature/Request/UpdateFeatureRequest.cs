namespace Shared.DTO.Feature.Request;

public record UpdateFeatureRequest(
    Guid FeatureId,
    string FeatureName,
    string FeatureType,
    Guid CategoryId,
    Guid SubCategoryId,
    bool IsActive
);