using MediatR;
using Shared.DTO.Feature.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Feature.Query;

public sealed record GetActiveFeaturesQuery : IRequest<List<GetFeaturesResponse>>;

public class GetActiveFeaturesQueryHandler : IRequestHandler<GetActiveFeaturesQuery, List<GetFeaturesResponse>>
{
    private readonly IFeatureRepository _featureRepository;

    public GetActiveFeaturesQueryHandler(IFeatureRepository featureRepository)
    {
        _featureRepository = featureRepository;
    }

    public async Task<List<GetFeaturesResponse>> Handle(GetActiveFeaturesQuery request,
        CancellationToken cancellationToken)
    {
        var featuresResponse = await _featureRepository.GetFeaturesAsync(f => f.IsActive);
        return featuresResponse;
    }
}