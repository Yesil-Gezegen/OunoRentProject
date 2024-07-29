using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Shared.DTO.SubCategory.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.SubCategory.Query;

public sealed record GetSubCategoriesQuery(Guid CategoryId) : IRequest<List<GetSubCategoriesResponse>>
{
    internal class GetSubCategoriesQueryHandler : IRequestHandler<GetSubCategoriesQuery, List<GetSubCategoriesResponse>>
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public GetSubCategoriesQueryHandler(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<List<GetSubCategoriesResponse>> Handle(GetSubCategoriesQuery request, CancellationToken cancellationToken)
        {
            var subCategoriesList = await _subCategoryRepository.GetSubCategories(request.CategoryId);

            return subCategoriesList;
        }
    }
}
