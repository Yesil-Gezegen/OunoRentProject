namespace Shared.DTO.FooterItem.Response;

public class GetFooterItemsResponse
{
    public Guid FooterItemId { get; set; }

    public string Label { get; set; }

    public string Column { get; set; }

    public int OrderNumber { get; set; }

    public string TargetUrl { get; set; }

    public Boolean IsActive { get; set; }
}