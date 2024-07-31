using MediatR;
using Shared.DTO.Feature.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Feature.Query;

public sealed record GetFeaturesQuery : IRequest<List<GetFeaturesResponse>>;

public class GetFeaturesQueryHandler : IRequestHandler<GetFeaturesQuery, List<GetFeaturesResponse>>
{
    private readonly IFeatureRepository _featureRepository;

    public GetFeaturesQueryHandler(IFeatureRepository featureRepository)
    {
        _featureRepository = featureRepository;
    }
    
    public async Task<List<GetFeaturesResponse>> Handle(GetFeaturesQuery request, CancellationToken cancellationToken)
    {
        var featureResponse = await _featureRepository.GetFeaturesAsync();
        return featureResponse;
    }
}
