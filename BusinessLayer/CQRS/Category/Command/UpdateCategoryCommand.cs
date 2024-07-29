using MediatR;
using Shared.DTO.Category.Request;
using Shared.DTO.Category.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Category.Command;

public sealed record UpdateCategoryCommand(UpdateCategoryRequest Category) : IRequest<CategoryResponse>;

/// <summary>
/// <c>UpdateCategoryCommand</c> komutunu işleyen bir işleyici sınıfıdır.
/// Belirtilen kategori bilgisini güncellemek için repository üzerinden işlem yapar.
/// </summary>
internal class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.UpdateCategory(request.Category);
    }
}

