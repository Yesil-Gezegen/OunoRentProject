using MediatR;
using Shared.DTO.MenuItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.MenuItem.Query;

public sealed record GetMenuItemQuery(Guid MenuItemId) : IRequest<GetMenuItemResponse>;

public class GetMenuItemQueryHandler : IRequestHandler<GetMenuItemQuery, GetMenuItemResponse>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetMenuItemQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public Task<GetMenuItemResponse> Handle(GetMenuItemQuery request, CancellationToken cancellationToken)
    {
        var menuItemResponse = _menuItemRepository.GetMenuItemAsync(request.MenuItemId);
        return menuItemResponse;
    }
}
