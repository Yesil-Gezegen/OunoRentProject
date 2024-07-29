using System.Linq.Expressions;
using AutoMapper;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.MenuItem.Request;
using Shared.DTO.MenuItem.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public MenuItemRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateMenuItem

    public async Task<MenuItemResponse> CreateMenuItemAsync(CreateMenuItemRequest createMenuItemRequest)
    {
        await IsExistGeneric(x => x.Label == createMenuItemRequest.Label);

        await IsExistOrderNumber(createMenuItemRequest.OrderNumber);

        var menuItem = new MenuItem
        {
            Label = createMenuItemRequest.Label,
            TargetUrl = createMenuItemRequest.TargetUrl,
            OrderNumber = createMenuItemRequest.OrderNumber,
            OnlyToMembers = createMenuItemRequest.OnlyToMembers,
            IsActive = createMenuItemRequest.IsActive,
        };

        _applicationDbContext.MenuItems.Add(menuItem);

        await _applicationDbContext.SaveChangesAsync();

        var menuItemResponse = _mapper.Map<MenuItemResponse>(menuItem);

        return menuItemResponse;
    }


    #endregion
 
    #region DeleteMenuItem

    public async Task<Guid> DeleteMenuItemAsync(Guid menuItemId)
    {
        var entity = await _applicationDbContext.MenuItems
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.MenuItemId == menuItemId)
                     ?? throw new NotFoundException("Menu item not found.");

        _applicationDbContext.MenuItems.Remove(entity);

        await _applicationDbContext.SaveChangesAsync();

        return entity.MenuItemId;
    }


    #endregion

    #region GetMenuItem

    public async Task<GetMenuItemResponse> GetMenuItemAsync(Guid menuItemId)
    {
        var menuItem = await _applicationDbContext.MenuItems
                           .AsNoTracking()
                           .FirstOrDefaultAsync(x => x.MenuItemId == menuItemId)
                       ?? throw new NotFoundException("Menu not found");

        var menuItemResponse = _mapper.Map<GetMenuItemResponse>(menuItem);
        
        return menuItemResponse;
    }


    #endregion
 
    #region GetMenuItems

    public async Task<List<GetMenuItemsResponse>> GetMenuItemsAsync()
    {
        var entity = await _applicationDbContext.MenuItems
            .AsNoTracking()
            .ToListAsync();

        var menuItemResponse = _mapper.Map<List<GetMenuItemsResponse>>(entity);

        return menuItemResponse;
    }


    #endregion
  
    #region UpdateMenuItem

    public async Task<MenuItemResponse> UpdateMenuItemAsync(UpdateMenuItemRequest updateMenuItemRequest)
    {
        await IsExistGeneric(x => x.Label == updateMenuItemRequest.Label);

        await IsExistOrderNumberWhenUpdate(updateMenuItemRequest.MenuItemId, updateMenuItemRequest.OrderNumber);

        var menuItem = await _applicationDbContext.MenuItems
                           .Where(x => x.MenuItemId == updateMenuItemRequest.MenuItemId)
                           .FirstOrDefaultAsync()
                       ?? throw new NotFoundException("Menu item not found");

        menuItem.Label = updateMenuItemRequest.Label;
        menuItem.TargetUrl = updateMenuItemRequest.TargetUrl;
        menuItem.OrderNumber = updateMenuItemRequest.OrderNumber;
        menuItem.OnlyToMembers = updateMenuItemRequest.OnlyToMembers;
        menuItem.IsActive = updateMenuItemRequest.IsActive;

        _applicationDbContext.MenuItems.Update(menuItem);

        await _applicationDbContext.SaveChangesAsync();

        var menuItemResponse = _mapper.Map<MenuItemResponse>(menuItem);

        return menuItemResponse;
    }


    #endregion
  
    #region IsExist

    private async Task<bool> IsExistGeneric(Expression<Func<MenuItem, bool>> filter)
    {
        return await _applicationDbContext.MenuItems.AnyAsync(filter);
    }

    private async Task IsExistOrderNumber(int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.MenuItems
            .AnyAsync(x => x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
            throw new ConflictException("Order number already exists");
    }
    
    private async Task IsExistOrderNumberWhenUpdate(Guid menuItemId, int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.MenuItems
            .AnyAsync(x => x.MenuItemId != menuItemId && x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
        {
            throw new ConflictException("Order number already exists");
        }
    }
    
    #endregion
    
   
    
}