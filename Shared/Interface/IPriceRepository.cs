using Shared.DTO.Price.Request;
using Shared.DTO.Price.Response;

namespace Shared.Interface;

public interface IPriceRepository
{
    Task<PriceResponse> CreatePrice(CreatePriceRequest createPriceRequest);
    
    Task<List<GetPricesResponse>> GerPrices();
    
    Task<GetPriceResponse> GetPrice(Guid priceId);
    
    Task<PriceResponse> UpdatePrice(UpdatePriceRequest updatePriceRequest);
    
    Task<Guid> DeletePrice(Guid priceId);
}