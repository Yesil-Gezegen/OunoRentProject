using MediatR;
using Shared.DTO.Category.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Category.Query;

public sealed record GetCategoryQuery(Guid CategoryId) : IRequest<GetCategoryResponse>;

/// <summary>
/// <c>GetCategoryQuery</c> sorgusunu işleyen bir işleyici sınıfıdır.
/// Belirli bir kategori bilgisini almak için repository üzerinden işlem yapar.
/// </summary>
internal class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, GetCategoryResponse>
{
    ICategoryRepository _categoryRepository;

    public GetCategoryQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<GetCategoryResponse> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetCategory(request.CategoryId);

        return category;
    }
}
