using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Shared.DTO.SubCategory.Request;
using Shared.DTO.SubCategory.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.SubCategory.Command;

public sealed record CreateSubCategoryCommand(
    Guid CategoryId, CreateSubCategoryRequest CreateSubCategoryRequest) : IRequest<SubCategoryResponse>
{
    internal class CreateSubCategoryCommandHandler : IRequestHandler<CreateSubCategoryCommand, SubCategoryResponse>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public CreateSubCategoryCommandHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<SubCategoryResponse> Handle(CreateSubCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _subCategoryRepository.CreateSubCategory(
                request.CategoryId,
                request.CreateSubCategoryRequest);
        }
    }
}
