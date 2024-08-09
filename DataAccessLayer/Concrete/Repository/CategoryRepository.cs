using System.Linq.Expressions;
using Shared.DTO.Category.Response;
using Shared.Interface;
using Microsoft.EntityFrameworkCore;
using EntityLayer.Entities;
using Shared.DTO.Category.Request;
using BusinessLayer.Middlewares;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using DataAccessLayer.Concrete.Context;
using OfficeOpenXml;

namespace DataAccessLayer.Concrete.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;

    public CategoryRepository(ApplicationDbContext applicationDbContext, IMapper mapper, IImageService imageService)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _imageService = imageService;
    }

    #region GetCategories

    public async Task<List<GetCategoriesResponse>> GetCategories(
        Expression<Func<GetCategoryResponse, bool>>? predicate = null)
    {
        var categories = _applicationDbContext.Categories
            .AsNoTracking();

        if (predicate != null)
        {
            var categoriesPredicate = _mapper.MapExpression<Expression<Func<Category, bool>>>(predicate);
            categories = categories.Where(categoriesPredicate);
        }

        var categoriesList = await categories
            .Include(x => x.SubCategories)
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .Select(x => new GetCategoriesResponse
            {
                CategoryId = x.CategoryId,
                Name = x.Name,
                OrderNumber = x.OrderNumber,
                IsActive = x.IsActive,
                SubCategories = x.SubCategories.Select(y => new GetCategoriesResponse.SubCategory
                {
                    SubCategoryId = y.SubCategoryId,
                    Name = y.Name,
                    Description = y.Description,
                    Icon = y.Icon,
                    OrderNumber = y.OrderNumber,
                    IsActive = y.IsActive
                }).ToList()
            }).ToListAsync();

        var categoriesResponse = _mapper.Map<List<GetCategoriesResponse>>(categoriesList);

        return categoriesResponse;
    }

    #endregion

    #region GetCategory

    public async Task<GetCategoryResponse> GetCategory(Guid categoryId)
    {
        var category = await _applicationDbContext.Categories
                           .AsNoTracking()
                           .Where(x => x.CategoryId == categoryId)
                           .FirstOrDefaultAsync()
                       ?? throw new NotFoundException(CategoryExceptionMessages.NotFound);

        var categoryResponse = _mapper.Map<GetCategoryResponse>(category);
        return categoryResponse;
    }

    #endregion

    #region CreateCategory

    public async Task<CategoryResponse> CreateCategory(CreateCategoryRequest createCategoryRequest)
    {
        await IsExistGeneric(x => x.Name.Trim() == createCategoryRequest.Name.Trim());

        await IsExistOrderNumber(createCategoryRequest.OrderNumber);

        var category = new Category();

        category.Name = createCategoryRequest.Name.Trim();
        category.Description = createCategoryRequest.Description.Trim();
        category.Icon = await _imageService.SaveImageAsync(createCategoryRequest.Icon);
        category.OrderNumber = createCategoryRequest.OrderNumber;
        category.ImageHorizontalUrl = await _imageService.SaveImageAsync(createCategoryRequest.ImageHorizontal);
        category.ImageSquareUrl = await _imageService.SaveImageAsync(createCategoryRequest.ImageSquare);
        category.IsActive = true;

        _applicationDbContext.Categories.Add(category);

        await _applicationDbContext.SaveChangesAsync();

        var categoryResponse = _mapper.Map<CategoryResponse>(category);

        return categoryResponse;
    }

    #endregion

    #region UpdateCategory

    public async Task<CategoryResponse> UpdateCategory(UpdateCategoryRequest updateCategoryRequest)
    {
        var category = await _applicationDbContext.Categories
                           .Where(x => x.CategoryId == updateCategoryRequest.CategoryId)
                           .FirstOrDefaultAsync()
                       ?? throw new NotFoundException(CategoryExceptionMessages.NotFound);

        await IsExistWhenUpdate(updateCategoryRequest.CategoryId, updateCategoryRequest.OrderNumber, updateCategoryRequest.Name);
        
        category.Name = updateCategoryRequest.Name.Trim();
        category.Description = updateCategoryRequest.Description.Trim();
        category.OrderNumber = updateCategoryRequest.OrderNumber;
        category.IsActive = true;

        if (updateCategoryRequest.ImageHorizontal != null)
        {
            await _imageService.DeleteImageAsync(category.ImageHorizontalUrl);
            category.ImageHorizontalUrl = await _imageService.SaveImageAsync(updateCategoryRequest.ImageHorizontal);
        }

        if (updateCategoryRequest.ImageSquare != null)
        {
            await _imageService.DeleteImageAsync(category.ImageSquareUrl);
            category.ImageSquareUrl = await _imageService.SaveImageAsync(updateCategoryRequest.ImageSquare);
        }

        if (updateCategoryRequest.Icon != null)
        {
            await _imageService.DeleteImageAsync(category.Icon);
            category.Icon = await _imageService.SaveImageAsync(updateCategoryRequest.Icon);
        }

        await _applicationDbContext.SaveChangesAsync();

        var categoryResponse = _mapper.Map<CategoryResponse>(category);
        return categoryResponse;
    }

    #endregion

    #region DeleteCategory

    public async Task<Guid> DeleteCategory(Guid categoryId)
    {
        var category = await _applicationDbContext.Categories
                           .Where(x => x.CategoryId == categoryId)
                           .FirstOrDefaultAsync()
                       ?? throw new NotFoundException(CategoryExceptionMessages.NotFound);

        await IsUsedCategory(categoryId);

        await _imageService.DeleteImageAsync(category.ImageSquareUrl);

        await _imageService.DeleteImageAsync(category.ImageHorizontalUrl);

        await _imageService.DeleteImageAsync(category.Icon);
        
        _applicationDbContext.Categories.Remove(category);

        await _applicationDbContext.SaveChangesAsync();

        return category.CategoryId;
    }

    #endregion

    #region IsExistCategory

    private async Task IsExistOrderNumber(int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.Categories
            .AnyAsync(x => x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
        {
            throw new ConflictException(CategoryExceptionMessages.OrderNumberConflict);
        }
    }

    private async Task IsExistWhenUpdate(Guid categoryId, int orderNumber, string name)
    {
        var isExistOrderNumber = await _applicationDbContext.Categories
            .AnyAsync(x => x.CategoryId != categoryId && x.OrderNumber == orderNumber);
        
        var isExistCategory = await _applicationDbContext.Categories
            .AnyAsync(x=> x.CategoryId != categoryId && x.Name.Trim() == name.Trim());

        if (isExistOrderNumber)
        {
            throw new ConflictException(CategoryExceptionMessages.OrderNumberConflict);
        }

        if (isExistCategory)
        {
            throw new ConflictException("Category already exists");
        }
    }

    private async Task<bool> IsExistGeneric(Expression<Func<Category, bool>> filter)
    {
        var result = await _applicationDbContext.Categories.AnyAsync(filter);

        if (result)
            throw new ConflictException(CategoryExceptionMessages.Conflict);

        return result;
    }

    #endregion

    #region IsUsedCategory

    private async Task IsUsedCategory(Guid categoryId)
    {
        var isUsed = await _applicationDbContext.SubCategories
            .AnyAsync(x => x.CategoryId == categoryId);

        if (isUsed)
        {
            throw new IsUsedException("Category is used");
        }
    }

    #endregion
    
     public async Task<byte[]> ExportCategoriesToExcel(Expression<Func<GetCategoriesResponse, bool>>? predicate = null)
    {
        var categories = _applicationDbContext.Categories
        .AsNoTracking();

    if (predicate != null)
    {
        var categoriesPredicate = _mapper.Map<Expression<Func<Category, bool>>>(predicate);
        categories = categories.Where(categoriesPredicate);
    }

    var categoriesList = await categories
        .Include(x => x.SubCategories)
        .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
        .Select(x => new GetCategoriesResponse
        {
            CategoryId = x.CategoryId,
            Name = x.Name,
            OrderNumber = x.OrderNumber,
            SubCategories = x.SubCategories.Select(y => new GetCategoriesResponse.SubCategory
            {
                SubCategoryId = y.SubCategoryId,
                Name = y.Name,
                Description = y.Description,
                Icon = y.Icon,
                OrderNumber = y.OrderNumber,
                IsActive = y.IsActive
            }).ToList()
        }).ToListAsync();

    using (var package = new ExcelPackage())
    {
        var worksheet = package.Workbook.Worksheets.Add("Categories");

        worksheet.Cells[1, 1].Value = "CategoryId";
        worksheet.Cells[1, 2].Value = "Name";
        worksheet.Cells[1, 3].Value = "OrderNumber";
        worksheet.Cells[1, 4].Value = "SubCategoryId";
        worksheet.Cells[1, 5].Value = "SubCategory Name";
        worksheet.Cells[1, 6].Value = "Description";
        worksheet.Cells[1, 7].Value = "Icon";
        worksheet.Cells[1, 8].Value = "SubCategory OrderNumber";
        worksheet.Cells[1, 9].Value = "IsActive";

        worksheet.Row(1).Style.Font.Bold = true;

        int row = 2;
        foreach (var category in categoriesList)
        {
            foreach (var subCategory in category.SubCategories)
            {
                worksheet.Cells[row, 1].Value = category.CategoryId;
                worksheet.Cells[row, 2].Value = category.Name;
                worksheet.Cells[row, 3].Value = category.OrderNumber;
                worksheet.Cells[row, 4].Value = subCategory.SubCategoryId;
                worksheet.Cells[row, 5].Value = subCategory.Name;
                worksheet.Cells[row, 6].Value = subCategory.Description;
                worksheet.Cells[row, 7].Value = subCategory.Icon;
                worksheet.Cells[row, 8].Value = subCategory.OrderNumber;
                worksheet.Cells[row, 9].Value = subCategory.IsActive;

                row++;
            }
        }

        worksheet.Cells.AutoFitColumns();

        return await Task.FromResult(package.GetAsByteArray());
    }
    }
}