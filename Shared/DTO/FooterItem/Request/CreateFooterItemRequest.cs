namespace Shared.DTO.FooterItem.Request;

public sealed record CreateFooterItemRequest(
    string Label, int Column, int OrderNumber, string TargetUrl, Boolean IsActive);
