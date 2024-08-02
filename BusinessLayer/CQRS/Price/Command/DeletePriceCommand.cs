using MediatR;
using Shared.Interface;

namespace BusinessLayer.CQRS.Price.Command;

public sealed record DeletePriceCommand(Guid PriceId) : IRequest<Guid>
{
    internal class DeletePriceCommandHandler : IRequestHandler<DeletePriceCommand, Guid>
    {
        private readonly IPriceRepository _priceRepository;

        public DeletePriceCommandHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public async Task<Guid> Handle(DeletePriceCommand request, CancellationToken cancellationToken)
        {
            return await _priceRepository.DeletePrice(request.PriceId);
        }
    }
}