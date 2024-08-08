using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.MenuItem.Request;
using Shared.DTO.MenuItem.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;

    public MenuItemRepository(ApplicationDbContext applicationDbContext, IMapper mapper, IImageService imageService)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _imageService = imageService;
    }

    #region CreateMenuItem

    public async Task<MenuItemResponse> CreateMenuItemAsync(CreateMenuItemRequest createMenuItemRequest)
    {
        await IsExistGeneric(x => x.Label.Trim() == createMenuItemRequest.Label.Trim());

        await IsExistOrderNumber(createMenuItemRequest.OrderNumber);

        var menuItem = new MenuItem
        {
            Label = createMenuItemRequest.Label.Trim(),
            TargetUrl = createMenuItemRequest.TargetUrl.Trim(),
            Icon = await _imageService.SaveImageAsync(createMenuItemRequest.Icon),
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
                         .FirstOrDefaultAsync(x => x.MenuItemId == menuItemId)
                     ?? throw new NotFoundException(MenuItemExceptionMessages.NotFound);

        await _imageService.DeleteImageAsync(entity.Icon);

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
                       ?? throw new NotFoundException(MenuItemExceptionMessages.NotFound);

        var menuItemResponse = _mapper.Map<GetMenuItemResponse>(menuItem);

        return menuItemResponse;
    }

    #endregion

    #region GetMenuItems

    public async Task<List<GetMenuItemsResponse>> GetMenuItemsAsync(
        Expression<Func<GetMenuItemResponse, bool>>? predicate = null)
    {
        var menuItems = _applicationDbContext.MenuItems
            .AsNoTracking();

        if (predicate != null) 
        {
            var menuItemPredicate = _mapper.MapExpression<Expression<Func<MenuItem, bool>>>(predicate);
            menuItems = menuItems.Where(menuItemPredicate);
        }

        var menuItemsList = await menuItems
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();

        var menuItemResponse = _mapper.Map<List<GetMenuItemsResponse>>(menuItemsList);

        return menuItemResponse;
    }

    #endregion

    #region UpdateMenuItem

    public async Task<MenuItemResponse> UpdateMenuItemAsync(UpdateMenuItemRequest updateMenuItemRequest)
    {
        await IsExistWhenUpdate(updateMenuItemRequest.MenuItemId, updateMenuItemRequest.OrderNumber,
            updateMenuItemRequest.Label);

        var menuItem = await _applicationDbContext.MenuItems
                           .Where(x => x.MenuItemId == updateMenuItemRequest.MenuItemId)
                           .FirstOrDefaultAsync()
                       ?? throw new NotFoundException(MenuItemExceptionMessages.NotFound);

        menuItem.Label = updateMenuItemRequest.Label;
        menuItem.TargetUrl = updateMenuItemRequest.TargetUrl;
        menuItem.OrderNumber = updateMenuItemRequest.OrderNumber;
        menuItem.OnlyToMembers = updateMenuItemRequest.OnlyToMembers;
        menuItem.IsActive = updateMenuItemRequest.IsActive;

        if (updateMenuItemRequest.Icon != null)
        {
            await _imageService.DeleteImageAsync(menuItem.Icon);
            menuItem.Icon = await _imageService.SaveImageAsync(updateMenuItemRequest.Icon);
        }

        _applicationDbContext.MenuItems.Update(menuItem);

        await _applicationDbContext.SaveChangesAsync();

        var menuItemResponse = _mapper.Map<MenuItemResponse>(menuItem);

        return menuItemResponse;
    }

    #endregion

    #region IsExist

    private async Task<bool> IsExistGeneric(Expression<Func<MenuItem, bool>> filter)
    {
        var result = await _applicationDbContext.MenuItems.AnyAsync(filter);

        if (result)
            throw new ConflictException("Menu item already exists");

        return result;
    }

    private async Task IsExistOrderNumber(int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.MenuItems
            .AnyAsync(x => x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
            throw new ConflictException(MenuItemExceptionMessages.OrderNumberConflict);
    }

    private async Task IsExistWhenUpdate(Guid menuItemId, int orderNumber, string label)
    {
        var isExistOrderNumber = await _applicationDbContext.MenuItems
            .AnyAsync(x => x.MenuItemId != menuItemId && x.OrderNumber == orderNumber);

        var isExistMenuItem = await _applicationDbContext.MenuItems
            .AnyAsync(x => x.MenuItemId != menuItemId && x.Label.Trim() == label.Trim());

        if (isExistOrderNumber)
        {
            throw new ConflictException(MenuItemExceptionMessages.OrderNumberConflict);
        }

        if (isExistMenuItem)
        {
            throw new ConflictException("Menu item already exists");
        }
    }

    #endregion
}