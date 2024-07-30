using Shared.DTO.FooterItem.Request;
using Shared.DTO.FooterItem.Response;

namespace Shared.Interface;

public interface IFooterItemRepository
{
    Task<FooterItemResponse> CreateFooterItem(CreateFooterItemRequest footerItemRequest);
    
    Task<List<GetFooterItemsResponse>> GetFooterItems();
    
    Task<GetFooterItemResponse> GetFooterItem(Guid footerItemId);
    
    Task<FooterItemResponse> UpdateFooterItem(UpdateFooterItemRequest updateFooterItemRequest);
    
    Task<Guid> DeleteFooterItem(Guid footerItemId);
}