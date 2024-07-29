namespace Shared.DTO.MenuItem.Request;

public sealed record UpdateMenuItemRequest(
    Guid MenuItemId,
    string Label,
    string TargetUrl,
    int OrderNumber,
    bool OnlyToMembers,
    bool IsActive
);
