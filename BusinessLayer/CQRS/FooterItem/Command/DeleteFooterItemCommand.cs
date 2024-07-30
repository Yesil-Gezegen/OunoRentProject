using MediatR;
using Shared.DTO.FooterItem.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FooterItem.Command;

public sealed record DeleteFooterItemCommand(Guid FooterItemId) : IRequest<Guid>
{
    internal class DeleteFooterItemCommandHandler : IRequestHandler<DeleteFooterItemCommand, Guid>
    {
        private readonly IFooterItemRepository _footerItemRepository;

        public DeleteFooterItemCommandHandler(IFooterItemRepository footerItemRepository)
        {
            _footerItemRepository = footerItemRepository;
        }

        public async Task<Guid> Handle(DeleteFooterItemCommand request, CancellationToken cancellationToken)
        {
            return await _footerItemRepository.DeleteFooterItem(request.FooterItemId);
        }
    }
}