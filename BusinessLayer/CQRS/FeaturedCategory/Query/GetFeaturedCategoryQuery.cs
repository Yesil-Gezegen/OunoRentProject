using MediatR;
using Shared.DTO.FeaturedCategories.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FeaturedCategory.Query;

public sealed record GetFeaturedCategoryQuery(Guid FeaturedCategoryId) : IRequest<GetFeaturedCategoryResponse>;

public class GetFeaturedCategoryQueryHandler : IRequestHandler<GetFeaturedCategoryQuery, GetFeaturedCategoryResponse>
{
    private readonly IFeaturedCategoryRepository _featuredCategoryRepository;

    public GetFeaturedCategoryQueryHandler(IFeaturedCategoryRepository featuredCategoryRepository)
    {
        _featuredCategoryRepository = featuredCategoryRepository;
    }

    public async Task<GetFeaturedCategoryResponse> Handle(GetFeaturedCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _featuredCategoryRepository.GetFeaturedCategoryAsync(request.FeaturedCategoryId);
        return result;
    }
}