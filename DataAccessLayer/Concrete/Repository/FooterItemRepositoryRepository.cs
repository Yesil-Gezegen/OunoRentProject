using System.Linq.Expressions;
using AutoMapper;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.FooterItem.Request;
using Shared.DTO.FooterItem.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class FooterItemRepository : IFooterItemRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    private readonly IMapper _mapper;
    
    public FooterItemRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateFooterItem

    public async Task<FooterItemResponse> CreateFooterItem(CreateFooterItemRequest footerItemRequest)
    {
        await IsExistGeneric(x=> x.Label.Trim() == footerItemRequest.Label.Trim());

        await IsExistOrderNumber(footerItemRequest.OrderNumber);
        
        var footerItem = new FooterItem();
        
        footerItem.Label = footerItemRequest.Label.Trim();
        footerItem.Column = footerItemRequest.Column.Trim();
        footerItem.OrderNumber = footerItemRequest.OrderNumber;
        footerItem.TargetUrl = footerItemRequest.TargetUrl.Trim();
        footerItem.IsActive = footerItemRequest.IsActive;
        
        _applicationDbContext.FooterItems.Add(footerItem);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return _mapper.Map<FooterItemResponse>(footerItem);
    }


    #endregion

    #region IsExist

    private async Task IsExistOrderNumber(int orderNumber)
    {
        var footerItemOrderNumber = await _applicationDbContext.FooterItems
            .AnyAsync(x => x.OrderNumber == orderNumber);

        if (footerItemOrderNumber)
            throw new ConflictException(FooterItemExceptionMessages.OrderNumberConflict);
    }
    
    private async Task<bool> IsExistGeneric(Expression<Func<FooterItem, bool>> filter)
    {
        var result = await _applicationDbContext.FooterItems.AnyAsync(filter);

        if (result)
            throw new ConflictException(BlogExceptionMessages.Conflict);

        return result;
    }

    #endregion
 
}