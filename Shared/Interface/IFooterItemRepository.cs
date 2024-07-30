using Shared.DTO.FooterItem.Request;
using Shared.DTO.FooterItem.Response;

namespace Shared.Interface;

public interface IFooterItemRepository
{
    Task<FooterItemResponse> CreateFooterItem(CreateFooterItemRequest footerItemRequest);
}