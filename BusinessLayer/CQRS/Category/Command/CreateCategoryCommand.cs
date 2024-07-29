using MediatR;
using Shared.DTO.Category.Request;
using Shared.DTO.Category.Response;
using Shared.Interface;
using AutoMapper;

namespace BusinessLayer.CQRS.Category.Command;

public sealed record CreateCategoryCommand(CreateCategoryRequest Category) : IRequest<CategoryResponse>;

/// <summary>
/// <c>CreateCategoryCommand</c> komutunu işleyen bir işleyici sınıfıdır.
/// Yeni bir kategori oluşturma işlemini gerçekleştirir ve sonucu <see cref="CategoryResponse"/> olarak döndürür.
/// </summary>
internal class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.CreateCategory(request.Category);

        var categoryResponse = _mapper.Map<CategoryResponse>(category);

        return categoryResponse;
    }
}