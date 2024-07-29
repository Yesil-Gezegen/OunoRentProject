using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Shared.DTO.SubCategory.Request;
using Shared.DTO.SubCategory.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.SubCategory.Query;

public sealed record GetSubCategoryQuery(Guid CategoryId, Guid SubCategoryId ) : IRequest<GetSubCategoryResponse>
{
    internal class GetSubCategoryQueryHandler : IRequestHandler<GetSubCategoryQuery, GetSubCategoryResponse>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public GetSubCategoryQueryHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<GetSubCategoryResponse> Handle(GetSubCategoryQuery request, CancellationToken cancellationToken)
        {
            return await _subCategoryRepository.GetSubCategory(request.CategoryId, request.SubCategoryId);
        }
    }
}
