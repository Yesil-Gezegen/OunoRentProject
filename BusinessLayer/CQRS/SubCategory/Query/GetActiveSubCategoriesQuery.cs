using MediatR;
using Shared.DTO.Category.Response;
using Shared.DTO.SubCategory.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.SubCategory.Query;

public sealed record GetActiveSubCategoriesQuery(Guid categoryId) : IRequest<List<GetSubCategoriesResponse>>;

public class
    GetActiveSubCategoriesQueryHandler : IRequestHandler<GetActiveSubCategoriesQuery, List<GetSubCategoriesResponse>>
{
    private readonly ISubCategoryRepository _subCategoryRepository;

    public GetActiveSubCategoriesQueryHandler(ISubCategoryRepository subCategoryRepository)
    {
        _subCategoryRepository = subCategoryRepository;
    }

    public async Task<List<GetSubCategoriesResponse>> Handle(GetActiveSubCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var subCategoriesResponse =
            await _subCategoryRepository.GetSubCategories(request.categoryId, sc => sc.IsActive);
        return subCategoriesResponse;
    }
}