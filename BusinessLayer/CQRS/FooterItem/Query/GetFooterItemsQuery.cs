using MediatR;
using Shared.DTO.FooterItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FooterItem.Query;

public sealed record GetFooterItemsQuery : IRequest<List<GetFooterItemsResponse>>
{
    internal class GetFooterItemsQueryHandler : IRequestHandler<GetFooterItemsQuery, List<GetFooterItemsResponse>>
    {
         private readonly IFooterItemRepository _footerItemRepository;

         public GetFooterItemsQueryHandler(IFooterItemRepository footerItemRepository)
         {
             _footerItemRepository = footerItemRepository;
         }
         
         public async Task<List<GetFooterItemsResponse>> Handle(GetFooterItemsQuery request, CancellationToken cancellationToken)
         {
             var footerItemList = await _footerItemRepository.GetFooterItems();

             return footerItemList;
         }
    }  
}