using Shared.DTO.Brand.Request;
using Shared.DTO.Brand.Response;

namespace Shared.Interface;

public interface IBrandRepository
{
    Task<BrandResponse> CreateBrand(CreateBrandRequest createBrandRequest);
    
    Task<List<GetBrandsResponse>> GetBrands();
    
    Task<GetBrandResponse> GetBrand(Guid brandId);
    
    Task<BrandResponse> UpdateBrand(UpdateBrandRequest updateBrandRequest);
    
    Task<Guid> DeleteBrand(Guid brandId);
}