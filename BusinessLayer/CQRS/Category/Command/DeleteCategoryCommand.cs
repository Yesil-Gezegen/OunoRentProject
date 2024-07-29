using MediatR;
using Shared.Interface;

namespace BusinessLayer.CQRS.Category.Command;

public sealed record DeleteCategoryCommand(Guid CategoryId) : IRequest<Guid>;

/// <summary>
/// <c>DeleteCategoryCommand</c> komutunu işleyen bir işleyici sınıfıdır.
/// Belirtilen kategori ID'sine göre kategori silme işlemini gerçekleştirir.
/// </summary>
internal class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Guid> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.DeleteCategory(request.CategoryId);
    }
}

