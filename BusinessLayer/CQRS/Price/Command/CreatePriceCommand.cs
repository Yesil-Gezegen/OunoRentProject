using MediatR;
using Shared.DTO.Price.Request;
using Shared.DTO.Price.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Price.Command;

public sealed record CreatePriceCommand(CreatePriceRequest CreatePriceRequest) : IRequest<PriceResponse>
{
    internal class CreatePriceCommandHandler : IRequestHandler<CreatePriceCommand, PriceResponse>
    {
        private readonly IPriceRepository _priceRepository;

        public CreatePriceCommandHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public async Task<PriceResponse> Handle(CreatePriceCommand request, CancellationToken cancellationToken)
        {
            return await _priceRepository.CreatePrice(request.CreatePriceRequest);
        }
    }
}