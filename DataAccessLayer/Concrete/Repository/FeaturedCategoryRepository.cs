using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.FeaturedCategories.Request;
using Shared.DTO.FeaturedCategories.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class FeaturedCategoryRepository : IFeaturedCategoryRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public FeaturedCategoryRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateFeaturedCategory

    public async Task<FeaturedCategoryResponse> CreateFeaturedCategoryAsync(
        CreateFeaturedCategoryRequest createFeaturedCategoryRequest)
    {
        await IsCategoryExist(createFeaturedCategoryRequest.CategoryId);

        await HasConflict(createFeaturedCategoryRequest);

        var featuredCategory = new FeaturedCategory()
        {
            OrderNumber = createFeaturedCategoryRequest.OrderNumber,
            IsActive = createFeaturedCategoryRequest.IsActive,
            CategoryId = createFeaturedCategoryRequest.CategoryId
        };

        await _applicationDbContext.AddAsync(featuredCategory);
        await _applicationDbContext.SaveChangesAsync();

        var featuredCategoryResponse = _mapper.Map<FeaturedCategoryResponse>(featuredCategory);
        return featuredCategoryResponse;
    }

    #endregion

    #region UpdateFeaturedCategory

    public async Task<FeaturedCategoryResponse> UpdateFeaturedCategoryAsync(
        UpdateFeaturedCategoryRequest updateFeaturedCategoryRequest)
    {
        var featuredCategory = await _applicationDbContext.FeaturedCategories.FirstOrDefaultAsync(fc =>
                                   fc.FeaturedCategoryId == updateFeaturedCategoryRequest.FeaturedCategoryId) ??
                               throw new NotFoundException(CategoryExceptionMessages.NotFound);

        await IsCategoryExist(updateFeaturedCategoryRequest.CategoryId);

        await HasConflict(updateFeaturedCategoryRequest);

        featuredCategory.CategoryId = updateFeaturedCategoryRequest.CategoryId;
        featuredCategory.OrderNumber = updateFeaturedCategoryRequest.OrderNumber;
        featuredCategory.IsActive = updateFeaturedCategoryRequest.IsActive;

        await _applicationDbContext.SaveChangesAsync();

        var featuredCategoryResponse = _mapper.Map<FeaturedCategoryResponse>(featuredCategory);
        return featuredCategoryResponse;
    }

    #endregion

    #region DeleteFeaturedCategories

    public async Task<Guid> DeleteFeaturedCategoryAsync(Guid featuredCategoryId)
    {
        var featuredCategory =
            await _applicationDbContext.FeaturedCategories.FirstOrDefaultAsync(fc =>
                fc.FeaturedCategoryId == featuredCategoryId) ??
            throw new NotFoundException(CategoryExceptionMessages.NotFound);

        _applicationDbContext.FeaturedCategories.Remove(featuredCategory);
        await _applicationDbContext.SaveChangesAsync();

        return featuredCategory.FeaturedCategoryId;
    }

    #endregion

    #region GetFeaturedCategory

    public async Task<GetFeaturedCategoryResponse> GetFeaturedCategoryAsync(Guid id)
    {
        var featuredCategory =
            await _applicationDbContext.FeaturedCategories
                .Include(fc => fc.Category)
                .FirstOrDefaultAsync(fc => fc.FeaturedCategoryId == id) ??
            throw new NotFoundException(CategoryExceptionMessages.NotFound);

        var featuredCategoryResponse = _mapper.Map<GetFeaturedCategoryResponse>(featuredCategory);
        return featuredCategoryResponse;
    }

    #endregion

    #region GetFeaturedCategories

    public async Task<List<GetFeaturedCategoriesResponse>> GetFeaturedCategoriesAsync(
        Expression<Func<GetFeaturedCategoryResponse, bool>>? predicate = null)
    {
        var featuredCategories = _applicationDbContext
            .FeaturedCategories
            .AsNoTracking();
        
        if (predicate != null)
        {
            var featuredCategoryPredicate = _mapper.MapExpression<Expression<Func<FeaturedCategory, bool>>>(predicate);
            featuredCategories = featuredCategories.Where(featuredCategoryPredicate);
        }

        var featuredCategoriesList = await featuredCategories
            .Include(x => x.Category)
            .ToListAsync();
        
        var featuredCategoriesResponse = _mapper.Map<List<GetFeaturedCategoriesResponse>>(featuredCategoriesList);
        return featuredCategoriesResponse;
    }

    #endregion

    #region IsExist

    private async Task<bool> IsExistAsync(Expression<Func<FeaturedCategory, bool>> filter)
    {
        return await _applicationDbContext.FeaturedCategories.AnyAsync(filter);
    }

    private async Task IsCategoryExist(Guid categoryId)
    {
        var isCategpryExist = await _applicationDbContext.Categories.AsNoTracking()
            .AnyAsync(c => c.CategoryId == categoryId);

        if (!isCategpryExist)
            throw new NotFoundException(CategoryExceptionMessages.NotFound);
    }

    private async Task HasConflict(CreateFeaturedCategoryRequest createFeaturedCategoryRequest)
    {
        var isCategoryExist = await _applicationDbContext.FeaturedCategories.AnyAsync(fc =>
            fc.CategoryId == createFeaturedCategoryRequest.CategoryId);

        var isOrderNumberExist = await _applicationDbContext.FeaturedCategories.AnyAsync(fc =>
            fc.OrderNumber == createFeaturedCategoryRequest.OrderNumber);

        if (isCategoryExist)
            throw new ConflictException(FeaturedCategoryExceptionMessages.CategoryConflict);

        if (isOrderNumberExist)
            throw new ConflictException(FeaturedCategoryExceptionMessages.OrderNumberConflict);
    }

    private async Task HasConflict(UpdateFeaturedCategoryRequest updateFeaturedCategoryRequest)
    {
        var isCategoryExist = await _applicationDbContext.FeaturedCategories
            .AnyAsync(fc =>
                fc.CategoryId == updateFeaturedCategoryRequest.CategoryId &&
                fc.FeaturedCategoryId != updateFeaturedCategoryRequest.FeaturedCategoryId);

        var isOrderNumberExist = await _applicationDbContext.FeaturedCategories
            .AnyAsync(fc =>
                fc.OrderNumber == updateFeaturedCategoryRequest.OrderNumber &&
                fc.FeaturedCategoryId != updateFeaturedCategoryRequest.FeaturedCategoryId);

        if (isOrderNumberExist)
            throw new ConflictException(FeaturedCategoryExceptionMessages.OrderNumberConflict);

        if (isCategoryExist)
            throw new ConflictException(FeaturedCategoryExceptionMessages.CategoryConflict);
    }

    #endregion
}