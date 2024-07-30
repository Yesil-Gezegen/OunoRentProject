using MediatR;
using Shared.DTO.FeaturedCategories.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FeaturedCategory.Query;

public sealed record GetActiveFeaturedCategoriesQuery() : IRequest<List<GetFeaturedCategoriesResponse>>;

public class
    GetActiveFeaturedCategoriesQueryHandler : IRequestHandler<GetActiveFeaturedCategoriesQuery,
    List<GetFeaturedCategoriesResponse>>
{
    private readonly IFeaturedCategoryRepository _featuredCategoryRepository;

    public GetActiveFeaturedCategoriesQueryHandler(IFeaturedCategoryRepository featuredCategoryRepository)
    {
        _featuredCategoryRepository = featuredCategoryRepository;
    }
    
    public async Task<List<GetFeaturedCategoriesResponse>> Handle(GetActiveFeaturedCategoriesQuery request, CancellationToken cancellationToken)
    {
        var featuredCategoriesResponse =
            await _featuredCategoryRepository.GetFeaturedCategoriesAsync(fc => fc.IsActive);
        return featuredCategoriesResponse;
    }
}