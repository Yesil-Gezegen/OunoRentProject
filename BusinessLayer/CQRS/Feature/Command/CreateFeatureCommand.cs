using MediatR;
using Shared.DTO.Feature.Request;
using Shared.DTO.Feature.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Feature.Command;

public sealed record CreateFeatureCommand(CreateFeatureRequest CreateFeatureRequest) : IRequest<FeatureResponse>;

public class CreateFeatureCommandHandler : IRequestHandler<CreateFeatureCommand, FeatureResponse>
{
    private readonly IFeatureRepository _featureRepository;

    public CreateFeatureCommandHandler(IFeatureRepository featureRepository)
    {
        _featureRepository = featureRepository;
    }
    
    public async Task<FeatureResponse> Handle(CreateFeatureCommand request, CancellationToken cancellationToken)
    {
        var featureResponse = await _featureRepository.CreateFeatureAsync(request.CreateFeatureRequest);
        return featureResponse;
    }
}