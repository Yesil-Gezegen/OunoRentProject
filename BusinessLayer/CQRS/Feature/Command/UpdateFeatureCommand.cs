using MediatR;
using Shared.DTO.Feature.Request;
using Shared.DTO.Feature.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Feature.Command;

public sealed record UpdateFeatureCommand(UpdateFeatureRequest UpdateFeatureRequest) : IRequest<FeatureResponse>;

public class UpdateFeatureCommandHandler : IRequestHandler<UpdateFeatureCommand, FeatureResponse>
{
    private readonly IFeatureRepository _featureRepository;

    public UpdateFeatureCommandHandler(IFeatureRepository featureRepository)
    {
        _featureRepository = featureRepository;
    }
    
    public async Task<FeatureResponse> Handle(UpdateFeatureCommand request, CancellationToken cancellationToken)
    {
        var featureResponse = await _featureRepository.UpdateFeatureAsync(request.UpdateFeatureRequest);
        return featureResponse;
    }
}