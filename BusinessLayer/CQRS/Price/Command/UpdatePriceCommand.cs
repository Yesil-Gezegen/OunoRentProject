using MediatR;
using Shared.DTO.Price.Request;
using Shared.DTO.Price.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Price.Command;

public sealed record UpdatePriceCommand(UpdatePriceRequest UpdatePriceRequest) : IRequest<PriceResponse>
{
    internal class UpdatePriceCommandHandler : IRequestHandler<UpdatePriceCommand, PriceResponse>
    {
        private readonly IPriceRepository _priceRepository;

        public UpdatePriceCommandHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public async Task<PriceResponse> Handle(UpdatePriceCommand request, CancellationToken cancellationToken)
        {
            return await _priceRepository.UpdatePrice(request.UpdatePriceRequest);
        }
    }
}