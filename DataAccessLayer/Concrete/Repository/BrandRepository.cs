using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Brand.Request;
using Shared.DTO.Brand.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class BrandRepository : IBrandRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;
    
    public BrandRepository(ApplicationDbContext applicationDbContext, IMapper mapper, IImageService imageService)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _imageService = imageService;
    }

    #region CreateBrand

    public async Task<BrandResponse> CreateBrand(CreateBrandRequest createBrandRequest)
    {
        var brand = new Brand();
        
        brand.Name = createBrandRequest.Name.Trim();
        brand.Logo = await _imageService.SaveImageAsync(createBrandRequest.Logo);
        brand.ShowOnBrands = createBrandRequest.ShowOnBrands;
        brand.IsActive = createBrandRequest.IsActive;
        
        _applicationDbContext.Brands.Add(brand);
         
        await _applicationDbContext.SaveChangesAsync();
         
        var brandResponse = _mapper.Map<BrandResponse>(brand);

        return brandResponse;
    }

    #endregion

    #region GetBrands

    public async Task<List<GetBrandsResponse>> GetBrands(Expression<Func<GetBrandsResponse, bool>>? predicate = null)
    {
        var brands = _applicationDbContext.Brands
            .AsNoTracking();

        if (predicate != null)
        {
            var brandPredicate = _mapper.MapExpression<Expression<Func<Brand, bool>>>(predicate);
            brands = brands.Where(brandPredicate);
        }
        
        var brandList = await brands
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();
        
        var brandResponse = _mapper.Map<List<GetBrandsResponse>>(brandList);

        return brandResponse;
    }


    #endregion

    #region GetBrand

    public async Task<GetBrandResponse> GetBrand(Guid brandId)
    {
        var brand = await _applicationDbContext.Brands
            .FirstOrDefaultAsync(x => x.BrandId == brandId);

        var brandResponse = _mapper.Map<GetBrandResponse>(brand);

        return brandResponse;
    }

    #endregion

    #region UpdateBrand

    public async Task<BrandResponse> UpdateBrand(UpdateBrandRequest updateBrandRequest)
    {
        var brand = await _applicationDbContext.Brands
                        .FirstOrDefaultAsync(x => x.BrandId == updateBrandRequest.BrandId)
                    ?? throw new NotFoundException(BrandExceptionMessages.NotFound);
        
        brand.Name = updateBrandRequest.Name.Trim();
        brand.ShowOnBrands = updateBrandRequest.ShowOnBrands;
        brand.IsActive = updateBrandRequest.IsActive;
        
        if(updateBrandRequest.Logo != null)
        {
            await _imageService.DeleteImageAsync(brand.Logo);
            brand.Logo = await _imageService.SaveImageAsync(updateBrandRequest.Logo);
        }

        await _applicationDbContext.SaveChangesAsync();

        var brandResponse = _mapper.Map<BrandResponse>(brand);

        return brandResponse;
    }

    #endregion

    #region DeleteBrand

    public async Task<Guid> DeleteBrand(Guid brandId)
    {
        var brand = await _applicationDbContext.Brands
                        .FirstOrDefaultAsync(x => x.BrandId == brandId)
                    ?? throw new NotFoundException(BrandExceptionMessages.NotFound);

       await _imageService.DeleteImageAsync(brand.Logo);
        
        _applicationDbContext.Brands.Remove(brand);

        await _applicationDbContext.SaveChangesAsync();

        return brandId;
    }

    #endregion
  
}