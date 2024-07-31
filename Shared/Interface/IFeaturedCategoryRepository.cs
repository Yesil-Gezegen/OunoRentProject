using System.Linq.Expressions;
using Shared.DTO.Category.Response;
using Shared.DTO.FeaturedCategories.Request;
using Shared.DTO.FeaturedCategories.Response;

namespace Shared.Interface;

public interface IFeaturedCategoryRepository
{
    Task<FeaturedCategoryResponse> CreateFeaturedCategoryAsync(
        CreateFeaturedCategoryRequest createFeaturedCategoryRequest);

    Task<FeaturedCategoryResponse> UpdateFeaturedCategoryAsync(
        UpdateFeaturedCategoryRequest updateFeaturedCategoryRequest);

    Task<Guid> DeleteFeaturedCategoryAsync(Guid featuredCategoryId);

    Task<GetFeaturedCategoryResponse> GetFeaturedCategoryAsync(Guid id);

    Task<List<GetFeaturedCategoriesResponse>> GetFeaturedCategoriesAsync(
        Expression<Func<GetFeaturedCategoryResponse, bool>>? predicate = null);
}