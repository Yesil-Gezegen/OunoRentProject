using MediatR;
using Shared.DTO.FeaturedCategories.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FeaturedCategory.Query;

public sealed record GetFeaturedCategoriesQuery() : IRequest<List<GetFeaturedCategoriesResponse>>;

public class
    GetFeaturedCategoriesQueryHandler : IRequestHandler<GetFeaturedCategoriesQuery, List<GetFeaturedCategoriesResponse>>
{
    private readonly IFeaturedCategoryRepository _featuredCategoryRepository;

    public GetFeaturedCategoriesQueryHandler(IFeaturedCategoryRepository featuredCategoryRepository)
    {
        _featuredCategoryRepository = featuredCategoryRepository;
    }

    public async Task<List<GetFeaturedCategoriesResponse>> Handle(GetFeaturedCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _featuredCategoryRepository.GetFeaturedCategoriesAsync();
        return result;
    }
}