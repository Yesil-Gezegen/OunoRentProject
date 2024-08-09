using System.Linq.Expressions;
using MediatR;
using Shared.DTO.Category.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Category.Command;

public sealed record ExportCategoriesToExcelCommand(Expression<Func<ExportExcelCategoryResponse, bool>>? Predicate = null) : IRequest<byte[]>
{
    internal class ExportCategoriesToExcelCommandHandler : IRequestHandler<ExportCategoriesToExcelCommand, byte[]>
    {
        private readonly ICategoryRepository _categoryRepository;

        public ExportCategoriesToExcelCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<byte[]> Handle(ExportCategoriesToExcelCommand request, CancellationToken cancellationToken)
        {
            var excelData = await _categoryRepository.ExportCategoriesToExcel(request.Predicate);

            return excelData;
        }
    }
}