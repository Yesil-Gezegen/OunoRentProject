using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.SubCategory.Request;
using Shared.DTO.SubCategory.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class SubCategoryRepository : ISubCategoryRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public SubCategoryRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
 
    #region CreateSubCategory

    public async Task<SubCategoryResponse> CreateSubCategory(Guid categoryId,
        CreateSubCategoryRequest createSubCategoryRequest)
    {
        await IsExistGeneric(x=> x.Name.Trim() == createSubCategoryRequest.Name.Trim());

        await IsExistOrderNumber(createSubCategoryRequest.OrderNumber);

        var subCategory = new SubCategory();

        subCategory.CategoryId = categoryId;
        subCategory.Name = createSubCategoryRequest.Name.Trim();
        subCategory.Description = createSubCategoryRequest.Description.Trim();
        subCategory.Icon = createSubCategoryRequest.Icon.Trim();
        subCategory.OrderNumber = createSubCategoryRequest.OrderNumber;
        subCategory.IsActive = true;

        _applicationDbContext.SubCategories.Add(subCategory);

        await _applicationDbContext.SaveChangesAsync();

        var categoryResponse = _mapper.Map<SubCategoryResponse>(subCategory);

        return categoryResponse;
    }

    #endregion

    #region DeleteSubCategory

    public async Task<Guid> DeleteSubCategory(Guid subCategoryId)
    {
        var subCategory = await _applicationDbContext.SubCategories
                              .Include(x => x.Category)
                              .Where(x => x.SubCategoryId == subCategoryId)
                              .FirstOrDefaultAsync()
                          ?? throw new NotFoundException(SubCategoryExceptionMessages.NotFound);

        _applicationDbContext.SubCategories.Remove(subCategory);

        await _applicationDbContext.SaveChangesAsync();

        return subCategoryId;
    }

    #endregion

    #region GetSubCategories

    public async Task<List<GetSubCategoriesResponse>> GetSubCategories(Guid categoryId,
        Expression<Func<GetSubCategoryResponse, bool>>? predicate = null)
    {
        var subCategories = _applicationDbContext.SubCategories
            .Include(x => x.Category)
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId);
        
        if (predicate != null)
        {
            var subCategoriesPredicate = _mapper.MapExpression<Expression<Func<SubCategory, bool>>>(predicate);
            subCategories = subCategories.Where(subCategoriesPredicate);
        }

        var subCategpriesList = await subCategories
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();

        var subCategoriesResponse = _mapper.Map<List<GetSubCategoriesResponse>>(subCategpriesList);

        return subCategoriesResponse;
    }

    #endregion

    #region GetSubCategory

    public async Task<GetSubCategoryResponse> GetSubCategory(Guid categoryId, Guid subCategoryId)
    {
        var subCategory = await _applicationDbContext.SubCategories
                              .AsNoTracking()
                              .Where(x => x.CategoryId == categoryId && x.SubCategoryId == subCategoryId)
                              .FirstOrDefaultAsync()
                          ?? throw new NotFoundException(SubCategoryExceptionMessages.NotFound);

        var subCategoryResponse = _mapper.Map<GetSubCategoryResponse>(subCategory);

        return subCategoryResponse;
    }

    #endregion

    #region UpdateSubCategory

    public async Task<SubCategoryResponse> UpdateSubCategory(Guid categoryId,
        UpdateSubCategoryRequest updateSubCategoryRequest)
    {
        var subCategory = await _applicationDbContext.SubCategories
                              .Include(x => x.Category)
                              .Where(x => x.CategoryId == categoryId &&
                                          x.SubCategoryId == updateSubCategoryRequest.SubCategoryId)
                              .FirstOrDefaultAsync()
                          ?? throw new NotFoundException(SubCategoryExceptionMessages.NotFound);

        await IsExistWhenUpdate(
            updateSubCategoryRequest.SubCategoryId, updateSubCategoryRequest.OrderNumber, updateSubCategoryRequest.Name);

        subCategory.Name = updateSubCategoryRequest.Name.Trim();
        subCategory.Description = updateSubCategoryRequest.Description.Trim();
        subCategory.Icon = updateSubCategoryRequest.Icon.Trim();
        subCategory.OrderNumber = updateSubCategoryRequest.OrderNumber;

        _applicationDbContext.SubCategories.Update(subCategory);

        await _applicationDbContext.SaveChangesAsync();

        var subCategoryResponse = _mapper.Map<SubCategoryResponse>(subCategory);

        return subCategoryResponse;
    }

    #endregion

    #region IsExistSubCategory

    private async Task IsExistOrderNumber(int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.SubCategories
            .AnyAsync(x => x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
        {
            throw new ConflictException(SubCategoryExceptionMessages.OrderNumberConflict);
        }
    }

    private async Task<bool> IsExistGeneric(Expression<Func<SubCategory, bool>> filter)
    {
        var result = await _applicationDbContext.SubCategories.AnyAsync(filter);

        if (result)
            throw new ConflictException(SubCategoryExceptionMessages.Conflict);

        return result;
    }
    
    private async Task IsExistWhenUpdate(Guid subCategoryId, int orderNumber, string name)
    {
        var isExistOrderNumber = await _applicationDbContext.SubCategories
            .AnyAsync(x => x.SubCategoryId != subCategoryId && x.OrderNumber == orderNumber);
        
        var isExistSubCategory = await _applicationDbContext.SubCategories
            .AnyAsync(x=> x.SubCategoryId != subCategoryId && x.Name.Trim() == name.Trim());

        if (isExistOrderNumber)
        {
            throw new ConflictException(SubCategoryExceptionMessages.OrderNumberConflict);
        }

        if (isExistSubCategory)
        {
            throw new ConflictException("SubCategory already exists");
        }
    }

    #endregion

}
