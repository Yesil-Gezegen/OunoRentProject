using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Feature.Request;
using Shared.DTO.Feature.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class FeatureRepository : IFeatureRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public FeatureRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateFeature

    public async Task<FeatureResponse> CreateFeatureAsync(CreateFeatureRequest createFeatureRequest)
    {
        await HasConflict(createFeatureRequest.CategoryId, createFeatureRequest.SubCategoryId);
        
        var feature = new Feature();

        feature.FeatureName = createFeatureRequest.FeatureName.Trim();
        feature.FeatureType = createFeatureRequest.FeatureType.Trim();
        feature.CategoryId = createFeatureRequest.CategoryId;
        feature.SubCategoryId = createFeatureRequest.SubCategoryId;
        feature.IsActive = createFeatureRequest.IsActive;

        await _applicationDbContext.Features.AddAsync(feature);
        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<FeatureResponse>(feature);
    }

    #endregion

    #region UpdateFeature

    public async Task<FeatureResponse> UpdateFeatureAsync(UpdateFeatureRequest updateFeatureRequest)
    {
        await HasConflict(updateFeatureRequest.CategoryId, updateFeatureRequest.SubCategoryId);
        
        var feature =
            await _applicationDbContext.Features.FirstOrDefaultAsync(f =>
                f.FeatureId == updateFeatureRequest.FeatureId) ??
            throw new NotFoundException(FeatureExceptionMessages.NotFound);

        feature.FeatureName = updateFeatureRequest.FeatureName.Trim();
        feature.FeatureType = updateFeatureRequest.FeatureType.Trim();
        feature.CategoryId = updateFeatureRequest.CategoryId;
        feature.SubCategoryId = updateFeatureRequest.SubCategoryId;
        feature.IsActive = updateFeatureRequest.IsActive;

        _applicationDbContext.Features.Update(feature);
        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<FeatureResponse>(feature);
    }

    #endregion

    #region DeleteFeature

    public async Task<FeatureResponse> DeleteFeatureAsync(Guid featureId)
    {
        var feature = await _applicationDbContext.Features.FirstOrDefaultAsync(f => f.FeatureId == featureId) ??
                      throw new NotFoundException(FeatureExceptionMessages.NotFound);

        _applicationDbContext.Features.Remove(feature);
        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<FeatureResponse>(feature);
    }

    #endregion

    #region GetFeature

    public async Task<GetFeatureResponse> GetFeatureAsync(Guid featureId)
    {
        var feature = await _applicationDbContext.Features
                          .AsNoTracking()
                          .Include(f => f.Category)
                          .Include(f => f.SubCategory)
                          .FirstOrDefaultAsync(f => f.FeatureId == featureId) ??
                      throw new NotFoundException(FeatureExceptionMessages.NotFound);

        return _mapper.Map<GetFeatureResponse>(feature);
    }

    #endregion

    #region GetFeatures

    public async Task<List<GetFeaturesResponse>> GetFeaturesAsync(
        Expression<Func<GetFeatureResponse, bool>>? predicate = null)
    {
        var features = _applicationDbContext.Features
            .Include(f => f.Category)
            .Include(f => f.SubCategory)
            .AsNoTracking();

        if (predicate != null)
        {
            var featurePredicate = _mapper.MapExpression<Expression<Func<Feature, bool>>>(predicate);
            features = features.Where(featurePredicate);
        }

        var featureList = await features.ToListAsync(); 

        return _mapper.Map<List<GetFeaturesResponse>>(featureList);
    }

    #endregion

    #region IsExist

    private async Task<bool> IsExist(Expression<Func<Feature, bool>> expression)
    {
        return await _applicationDbContext.Features.AnyAsync(expression);
    }

    #endregion

    #region HasConflict

    private async Task HasConflict(Guid categoryId, Guid subCategoryId)
    {
        var isCategoryExist =
            await _applicationDbContext.Categories.AnyAsync(c => c.CategoryId == categoryId);

        var isSubCategoryExist = await _applicationDbContext.SubCategories.AnyAsync(sc =>
            sc.SubCategoryId == subCategoryId && sc.CategoryId == categoryId);

        if (!isCategoryExist)
            throw new NotFoundException(CategoryExceptionMessages.NotFound);

        if (!isSubCategoryExist)
            throw new NotFoundException(SubCategoryExceptionMessages.NotFound);
    }

    #endregion
}