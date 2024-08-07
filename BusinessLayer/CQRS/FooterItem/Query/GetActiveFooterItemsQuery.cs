using MediatR;
using Shared.DTO.FooterItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FooterItem.Query;

public sealed record GetActiveFooterItemsQuery() : IRequest<List<GetFooterItemsResponse>>
{
    internal class GetActiveFooterItemsQueryHandler : IRequestHandler<GetActiveFooterItemsQuery, List<GetFooterItemsResponse>>
    {
        private readonly IFooterItemRepository _footerItemRepository;

        public GetActiveFooterItemsQueryHandler(IFooterItemRepository footerItemRepository)
        {
            _footerItemRepository = footerItemRepository;
        }

        public async Task<List<GetFooterItemsResponse>> Handle(GetActiveFooterItemsQuery request, CancellationToken cancellationToken)
        {
            return await _footerItemRepository.GetFooterItems(x => x.IsActive);
        }
    }
}