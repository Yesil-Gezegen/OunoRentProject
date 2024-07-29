using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.DTO.SubCategory.Request;
using Shared.DTO.SubCategory.Response;

namespace Shared.Interface;

public interface ISubCategoryRepository
{
    Task<SubCategoryResponse> CreateSubCategory(Guid CategoryId, CreateSubCategoryRequest createSubCategoryRequest);

    Task<GetSubCategoryResponse> GetSubCategory(Guid CategoryId, Guid SubCategoryId);

    Task<List<GetSubCategoriesResponse>> GetSubCategories(Guid CategoryId);

    Task<SubCategoryResponse> UpdateSubCategory(Guid CategoryId, UpdateSubCategoryRequest updateSubCategoryRequest);

    Task<Guid> DeleteSubCategory(Guid SubCategoryId);
}
