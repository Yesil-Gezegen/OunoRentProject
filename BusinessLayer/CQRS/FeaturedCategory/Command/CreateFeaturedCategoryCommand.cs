using MediatR;
using Shared.DTO.FeaturedCategories.Request;
using Shared.DTO.FeaturedCategories.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FeaturedCategory.Command;

public sealed record CreateFeaturedCategoryCommand(CreateFeaturedCategoryRequest CreateFeaturedCategoryRequest)
    : IRequest<FeaturedCategoryResponse>;

public class
    CreateFeaturedCategoryCommandHandler : IRequestHandler<CreateFeaturedCategoryCommand, FeaturedCategoryResponse>
{
    private readonly IFeaturedCategoryRepository _featuredCategoryRepository;

    public CreateFeaturedCategoryCommandHandler(IFeaturedCategoryRepository featuredCategoryRepository)
    {
        _featuredCategoryRepository = featuredCategoryRepository;
    }
    
    public async Task<FeaturedCategoryResponse> Handle(CreateFeaturedCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var result =
            await _featuredCategoryRepository.CreateFeaturedCategoryAsync(request.CreateFeaturedCategoryRequest);
        return result;
    }
}