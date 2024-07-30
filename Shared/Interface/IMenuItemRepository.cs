using System.Linq.Expressions;
using Shared.DTO.MenuItem.Request;
using Shared.DTO.MenuItem.Response;

namespace Shared.Interface;

public interface IMenuItemRepository
{
    Task<MenuItemResponse> CreateMenuItemAsync(CreateMenuItemRequest createMenuItemRequest);
    Task<MenuItemResponse> UpdateMenuItemAsync(UpdateMenuItemRequest updateMenuItemRequest);
    Task<Guid> DeleteMenuItemAsync(Guid id);
    Task<GetMenuItemResponse> GetMenuItemAsync(Guid id);
    Task<List<GetMenuItemsResponse>> GetMenuItemsAsync(Expression<Func<GetMenuItemResponse, bool>>? predicate = null);
}
