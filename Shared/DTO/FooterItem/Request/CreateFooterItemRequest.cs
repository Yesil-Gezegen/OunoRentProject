namespace Shared.DTO.FooterItem.Request;

public sealed record CreateFooterItemRequest(
    string Label, string Column, int OrderNumber, string TargetUrl, Boolean IsActive);
