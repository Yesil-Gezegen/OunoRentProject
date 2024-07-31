using AutoMapper;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Brand.Request;
using Shared.DTO.Brand.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class BrandRepository : IBrandRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public BrandRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<BrandResponse> CreateBrand(CreateBrandRequest createBrandRequest)
    {
        var brand = new Brand();
        
        brand.Name = createBrandRequest.Name.Trim();
        brand.Logo = createBrandRequest.Logo.Trim();
        brand.ShowOnBrands = createBrandRequest.ShowOnBrands;
        brand.IsActive = createBrandRequest.IsActive;
        
         _applicationDbContext.Brands.Add(brand);
         
         await _applicationDbContext.SaveChangesAsync();
         
         var brandResponse = _mapper.Map<BrandResponse>(brand);

         return brandResponse;
    }

    public async Task<List<GetBrandsResponse>> GetBrands()
    {
        var brandList = await _applicationDbContext.Brands
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();

        var categoriesResponse = _mapper.Map<List<GetBrandsResponse>>(brandList);
        
        return categoriesResponse;
    }

    public async Task<GetBrandResponse> GetBrand(Guid brandId)
    {
        var brand = await _applicationDbContext.Brands
            .FirstOrDefaultAsync(x => x.BrandId == brandId);

        var brandResponse = _mapper.Map<GetBrandResponse>(brand);

        return brandResponse;
    }

    public async Task<BrandResponse> UpdateBrand(UpdateBrandRequest updateBrandRequest)
    {
        var brand = await _applicationDbContext.Brands
                        .FirstOrDefaultAsync(x => x.BrandId == updateBrandRequest.BrandId)
                    ?? throw new NotFoundException(BrandExceptionMessages.NotFound);

        brand.Name = updateBrandRequest.Name.Trim();
        brand.Logo = updateBrandRequest.Logo.Trim();
        brand.ShowOnBrands = updateBrandRequest.ShowOnBrands;
        brand.IsActive = updateBrandRequest.IsActive;

        await _applicationDbContext.SaveChangesAsync();

        var brandResponse = _mapper.Map<BrandResponse>(brand);

        return brandResponse;
    }

    public async Task<Guid> DeleteBrand(Guid brandId)
    {
        var brand = await _applicationDbContext.Brands
                        .FirstOrDefaultAsync(x => x.BrandId == brandId)
                    ?? throw new NotFoundException(BrandExceptionMessages.NotFound);

        _applicationDbContext.Brands.Remove(brand);

        await _applicationDbContext.SaveChangesAsync();

        return brandId;
    }
}