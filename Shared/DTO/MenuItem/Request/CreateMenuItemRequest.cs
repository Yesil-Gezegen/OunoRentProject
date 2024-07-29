namespace Shared.DTO.MenuItem.Request;

public sealed record CreateMenuItemRequest(
    string Label,
    string TargetUrl,
    int OrderNumber,
    bool OnlyToMembers,
    bool IsActive
);