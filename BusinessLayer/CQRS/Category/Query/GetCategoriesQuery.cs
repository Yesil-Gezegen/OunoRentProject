using MediatR;
using Shared.DTO.Category.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Category.Query;

public sealed record GetCategoriesQuery() : IRequest<List<GetCategoriesResponse>>;

/// <summary>
/// <c>GetCategoriesQuery</c> sorgusunu işleyen bir işleyici sınıfıdır.
/// Kategori bilgilerini almak için repository üzerinden işlem yapar.
/// </summary>
internal class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<GetCategoriesResponse>>
{
    ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<GetCategoriesResponse>>
     Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetCategories();

        return categories;
    }
}
