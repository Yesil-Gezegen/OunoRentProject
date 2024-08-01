using AutoMapper;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Price.Request;
using Shared.DTO.Price.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class PriceRepository : IPriceRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public PriceRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    
    public async Task<PriceResponse> CreatePrice(CreatePriceRequest createPriceRequest)
    {
        var price = new Price();
        
        price.Barcode = createPriceRequest.Barcode;
        price.LogoPrice = createPriceRequest.LogoPrice;
        
        _applicationDbContext.Prices.Add(price);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return _mapper.Map<PriceResponse>(price);
    }

    public async Task<List<GetPricesResponse>> GerPrices()
    {
        var priceList = await _applicationDbContext.Prices
            .AsNoTracking()
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();

        return _mapper.Map<List<GetPricesResponse>>(priceList);
    }

    public async Task<GetPriceResponse> GetPrice(Guid priceId)
    {
        var price = await _applicationDbContext.Prices
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.PriceId == priceId)
                    ?? throw new NotFoundException(PriceExceptionMessages.NotFound);

        return _mapper.Map<GetPriceResponse>(price);
    }

    public async Task<PriceResponse> UpdatePrice(UpdatePriceRequest updatePriceRequest)
    {
        var price = await _applicationDbContext.Prices
            .FirstOrDefaultAsync(x=> x.PriceId == updatePriceRequest.PriceId)
                    ?? throw new NotFoundException(PriceExceptionMessages.NotFound);
        
        price.LogoPrice = updatePriceRequest.LogoPrice;

        _applicationDbContext.Prices.Update(price);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return _mapper.Map<PriceResponse>(price);
    }

    public async Task<Guid> DeletePrice(Guid priceId)
    {
        var price = await _applicationDbContext.Prices
                        .FirstOrDefaultAsync(x => x.PriceId == priceId)
                    ?? throw new NotFoundException(PriceExceptionMessages.NotFound);

        _applicationDbContext.Prices.Remove(price);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return priceId;
    }
}