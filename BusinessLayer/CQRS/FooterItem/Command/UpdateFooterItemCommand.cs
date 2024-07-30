using MediatR;
using Shared.DTO.FooterItem.Request;
using Shared.DTO.FooterItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FooterItem.Command;

public sealed record UpdateFooterItemCommand(UpdateFooterItemRequest UpdateFooterItemRequest)
    : IRequest<FooterItemResponse>
{

    internal class UpdateFooterItemCommandHandler : IRequestHandler<UpdateFooterItemCommand, FooterItemResponse>
    {
        private readonly IFooterItemRepository _footerItemRepository;

        public UpdateFooterItemCommandHandler(IFooterItemRepository footerItemRepository)
        {
            _footerItemRepository = footerItemRepository;
        }

        public async Task<FooterItemResponse> Handle(UpdateFooterItemCommand request,
            CancellationToken cancellationToken)
        {
            return await _footerItemRepository.UpdateFooterItem(request.UpdateFooterItemRequest);
        }
    }
}
