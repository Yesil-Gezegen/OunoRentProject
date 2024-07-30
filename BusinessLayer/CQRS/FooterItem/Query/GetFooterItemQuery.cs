using MediatR;
using Shared.DTO.FooterItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FooterItem.Query;

public sealed record GetFooterItemQuery(Guid FooterItemId) : IRequest<GetFooterItemResponse>
{
    internal class GetFooterItemQueryHandler : IRequestHandler<GetFooterItemQuery, GetFooterItemResponse>
    {
        private readonly IFooterItemRepository _footerItemRepository;

        public GetFooterItemQueryHandler(IFooterItemRepository footerItemRepository)
        {
            _footerItemRepository = footerItemRepository;
        }
        
        public async Task<GetFooterItemResponse> Handle(GetFooterItemQuery request, CancellationToken cancellationToken)
        {
            var footerItem = await _footerItemRepository.GetFooterItem(request.FooterItemId);

            return footerItem;
        }
    }  
}
