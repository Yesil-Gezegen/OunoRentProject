using System.Linq.Expressions;
using Shared.DTO.Category.Response;
using Shared.Interface;
using Microsoft.EntityFrameworkCore;
using EntityLayer.Entities;
using Shared.DTO.Category.Request;
using BusinessLayer.Middlewares;
using AutoMapper;

namespace DataAccessLayer.Concrete.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public CategoryRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region GetCategories
    public async Task<List<GetCategoriesResponse>> GetCategories()
    {
        var categories = await _applicationDbContext.Categories
            .AsNoTracking()
            .Include(x => x.SubCategories)
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
        
        var categoriesResponse = _mapper.Map<List<GetCategoriesResponse>>(categories);
        
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
        ?? throw new NotFoundException("Category not found");

        var categoryResponse = _mapper.Map<GetCategoryResponse>(category);
        return categoryResponse;
    }
    #endregion

    #region CreateCategory
    public async Task<CategoryResponse> CreateCategory(CreateCategoryRequest createCategoryRequest)
    {
        await IsExistGeneric(x => x.Name == createCategoryRequest.Name);

        await IsExistOrderNumber(createCategoryRequest.OrderNumber);

        var category = new Category();

        category.Name = createCategoryRequest.Name;
        category.Description = createCategoryRequest.Description;
        category.Icon = createCategoryRequest.Icon;
        category.OrderNumber = createCategoryRequest.OrderNumber;
        category.ImageHorizontalUrl = createCategoryRequest.ImageHorizontalUrl;
        category.ImageSquareUrl = createCategoryRequest.ImageSquareUrl;
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
        ?? throw new NotFoundException("Category not found");

        await IsExistOrderNumberWhenUpdate(updateCategoryRequest.CategoryId, updateCategoryRequest.OrderNumber);

        category.Name = updateCategoryRequest.Name;
        category.Description = updateCategoryRequest.Description;
        category.Icon = updateCategoryRequest.Icon;
        category.OrderNumber = updateCategoryRequest.OrderNumber;
        category.ImageHorizontalUrl = updateCategoryRequest.ImageHorizontalUrl;
        category.ImageSquareUrl = updateCategoryRequest.ImageSquareUrl;
        category.IsActive =  true;

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
        ?? throw new NotFoundException("Category not found");

        await IsUsedCategory(categoryId);

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
            throw new ConflictException("Order number already exists");
        }
    }
    
    private async Task IsExistOrderNumberWhenUpdate(Guid categoryId, int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.Categories
            .AnyAsync(x => x.CategoryId != categoryId && x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
        {
            throw new ConflictException("Order number already exists");
        }
    }

    private async Task<bool> IsExistGeneric(Expression<Func<Category, bool>> filter)
    {
        var result = await _applicationDbContext.Categories.AnyAsync(filter);

        if (result)
            throw new ConflictException("Already exist");

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
}