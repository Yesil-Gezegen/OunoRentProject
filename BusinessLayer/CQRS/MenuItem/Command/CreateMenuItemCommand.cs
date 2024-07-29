using MediatR;
using Shared.DTO.MenuItem.Request;
using Shared.DTO.MenuItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.MenuItem.Command;

public sealed record CreateMenuItemCommand(CreateMenuItemRequest CreateMenuItemRequest) : IRequest<MenuItemResponse>;

public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, MenuItemResponse>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public CreateMenuItemCommandHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<MenuItemResponse> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItemResponse = await _menuItemRepository.CreateMenuItemAsync(request.CreateMenuItemRequest);
        return menuItemResponse;
    }
}
