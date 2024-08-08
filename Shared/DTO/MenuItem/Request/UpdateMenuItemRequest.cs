using Microsoft.AspNetCore.Http;

namespace Shared.DTO.MenuItem.Request;

public sealed record UpdateMenuItemRequest(
    Guid MenuItemId,
    string Label,
    string TargetUrl,
    IFormFile? Icon,
    int OrderNumber,
    bool OnlyToMembers,
    bool IsActive
);
