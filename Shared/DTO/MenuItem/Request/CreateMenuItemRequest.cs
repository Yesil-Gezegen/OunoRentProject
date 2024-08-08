using Microsoft.AspNetCore.Http;

namespace Shared.DTO.MenuItem.Request;

public sealed record CreateMenuItemRequest(
    string Label,
    string TargetUrl,
    int OrderNumber,
    IFormFile Icon,
    bool OnlyToMembers,
    bool IsActive
);