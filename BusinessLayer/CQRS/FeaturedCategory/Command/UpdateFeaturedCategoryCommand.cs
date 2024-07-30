using MediatR;
using Shared.DTO.FeaturedCategories.Request;
using Shared.DTO.FeaturedCategories.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FeaturedCategory.Command;

public sealed record UpdateFeaturedCategoryCommand(UpdateFeaturedCategoryRequest UpdateFeaturedCategoryRequest)
    : IRequest<FeaturedCategoryResponse>;

public class
    UpdateFeaturedCategoryCommandHandler : IRequestHandler<UpdateFeaturedCategoryCommand, FeaturedCategoryResponse>
{
    private readonly IFeaturedCategoryRepository _featuredCategoryRepository;

    public UpdateFeaturedCategoryCommandHandler(IFeaturedCategoryRepository featuredCategoryRepository)
    {
        _featuredCategoryRepository = featuredCategoryRepository;
    }
    
    public async Task<FeaturedCategoryResponse> Handle(UpdateFeaturedCategoryCommand request, CancellationToken cancellationToken)
    {
        var result =
            await _featuredCategoryRepository.UpdateFeaturedCategoryAsync(request.UpdateFeaturedCategoryRequest);
        return result;
    }
}