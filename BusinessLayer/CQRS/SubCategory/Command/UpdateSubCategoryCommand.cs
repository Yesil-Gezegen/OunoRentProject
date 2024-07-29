using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Shared.DTO.SubCategory.Request;
using Shared.DTO.SubCategory.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.SubCategory.Command;

public sealed record UpdateSubCategoryCommand(Guid CategoryId, UpdateSubCategoryRequest UpdateSubCategoryRequest) : IRequest<SubCategoryResponse>
{
    internal class UpdateSubCategoryCommandHandler : IRequestHandler<UpdateSubCategoryCommand, SubCategoryResponse>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public UpdateSubCategoryCommandHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<SubCategoryResponse> Handle(UpdateSubCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _subCategoryRepository.UpdateSubCategory(
                request.CategoryId,
                request.UpdateSubCategoryRequest);
        }
    }
}
