using MediatR;
using Shared.DTO.MenuItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.MenuItem.Query;

public sealed record GetMenuItemsQuery() : IRequest<List<GetMenuItemsResponse>>;

public class GetMenuItemsQueryHandler : IRequestHandler<GetMenuItemsQuery, List<GetMenuItemsResponse>>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetMenuItemsQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<List<GetMenuItemsResponse>> Handle(GetMenuItemsQuery request, CancellationToken cancellationToken)
    {
        var menuItemResponse = await _menuItemRepository.GetMenuItemsAsync();
        return menuItemResponse;
    }
}
