namespace Shared.DTO.FooterItem.Request;

public sealed record UpdateFooterItemRequest(
    Guid FooterItemId, string Label, int Column, int OrderNumber, string TargetUrl, Boolean IsActive);
