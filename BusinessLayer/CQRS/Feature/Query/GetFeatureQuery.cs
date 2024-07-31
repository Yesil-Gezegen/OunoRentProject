using MediatR;
using Shared.DTO.Feature.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Feature.Query;

public sealed record GetFeatureQuery(Guid FeatureId) : IRequest<GetFeatureResponse>;

public class GetFeatureQueryHandler : IRequestHandler<GetFeatureQuery, GetFeatureResponse>
{
    private readonly IFeatureRepository _featureRepository;

    public GetFeatureQueryHandler(IFeatureRepository featureRepository)
    {
        _featureRepository = featureRepository;
    }
    
    public async Task<GetFeatureResponse> Handle(GetFeatureQuery request, CancellationToken cancellationToken)
    {
        var featureResponse = await _featureRepository.GetFeatureAsync(request.FeatureId);
        return featureResponse;
    }
}

