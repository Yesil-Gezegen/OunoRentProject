using MediatR;
using Shared.DTO.MenuItem.Request;
using Shared.DTO.MenuItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.MenuItem.Command;

public sealed record UpdateMenuItemCommand(UpdateMenuItemRequest UpdateMenuItemRequest) : IRequest<MenuItemResponse>;

public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, MenuItemResponse>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public UpdateMenuItemCommandHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<MenuItemResponse> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItemResponse = await _menuItemRepository.UpdateMenuItemAsync(request.UpdateMenuItemRequest);
        return menuItemResponse;
    }
}
