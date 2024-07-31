using System.Linq.Expressions;
using Shared.DTO.Feature.Request;
using Shared.DTO.Feature.Response;

namespace Shared.Interface;

public interface IFeatureRepository
{
    Task<FeatureResponse> CreateFeatureAsync(CreateFeatureRequest createFeatureRequest);
    Task<FeatureResponse> UpdateFeatureAsync(UpdateFeatureRequest updateFeatureRequest);
    Task<FeatureResponse> DeleteFeatureAsync(Guid featureId);
    Task<GetFeatureResponse> GetFeatureAsync(Guid featureId);
    Task<List<GetFeaturesResponse>> GetFeaturesAsync(Expression<Func<GetFeatureResponse, bool>>? predicate = null);
}