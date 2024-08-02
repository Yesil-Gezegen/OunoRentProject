using MediatR;
using Shared.DTO.Feature.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Feature.Command;

public sealed record DeleteFeatureCommand(Guid FeatureId) : IRequest<FeatureResponse>;

public class DeleteFeatureCommandHandler : IRequestHandler<DeleteFeatureCommand, FeatureResponse>
{
    private readonly IFeatureRepository _featureRepository;

    public DeleteFeatureCommandHandler(IFeatureRepository featureRepository)
    {
        _featureRepository = featureRepository;
    }
    
    public async Task<FeatureResponse> Handle(DeleteFeatureCommand request, CancellationToken cancellationToken)
    {
        var featureResponse = await _featureRepository.DeleteFeatureAsync(request.FeatureId);
        return featureResponse;
    }
}