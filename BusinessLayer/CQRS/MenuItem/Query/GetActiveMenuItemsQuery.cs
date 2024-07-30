using MediatR;
using Shared.DTO.MenuItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.MenuItem.Query;

public sealed record GetActiveMenuItemsQuery() : IRequest<List<GetMenuItemsResponse>>;

public class GetActiveMenuItemsQueryHandler : IRequestHandler<GetActiveMenuItemsQuery, List<GetMenuItemsResponse>>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetActiveMenuItemsQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<List<GetMenuItemsResponse>> Handle(GetActiveMenuItemsQuery request, CancellationToken cancellationToken)
    {
        var menuItemsResponse = await _menuItemRepository.GetMenuItemsAsync(mi => mi.IsActive);
        return menuItemsResponse;
    }
}