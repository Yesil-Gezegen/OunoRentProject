using MediatR;
using Shared.DTO.Category.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Category.Query;

public sealed record GetActiveCategoriesQuery() : IRequest<List<GetCategoriesResponse>>;

public class GetActiveCategoriesQueryHandler : IRequestHandler<GetActiveCategoriesQuery, List<GetCategoriesResponse>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetActiveCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<GetCategoriesResponse>> Handle(GetActiveCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categoriesResponse = await _categoryRepository.GetCategories(c => c.IsActive);

        return categoriesResponse;
    }
}