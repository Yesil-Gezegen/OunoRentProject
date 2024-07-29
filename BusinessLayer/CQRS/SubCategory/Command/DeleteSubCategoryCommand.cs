using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Shared.Interface;

namespace BusinessLayer.CQRS.SubCategory.Command;

public sealed record DeleteSubCategoryCommand(Guid SubCategoryId) : IRequest<Guid>
{
    internal class DeleteSubCategoryCommandHandler : IRequestHandler<DeleteSubCategoryCommand, Guid>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public DeleteSubCategoryCommandHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<Guid> Handle(DeleteSubCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _subCategoryRepository.DeleteSubCategory(request.SubCategoryId);
        }
    }
}
