using MediatR;
using Shared.DTO.Price.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Price.Query;

public sealed record GetPriceQuery(Guid PriceId) : IRequest<GetPriceResponse>
{
    internal class GetPriceQueryHandler : IRequestHandler<GetPriceQuery, GetPriceResponse>
    {
        private readonly IPriceRepository _priceRepository;

        public GetPriceQueryHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public async Task<GetPriceResponse> Handle(GetPriceQuery request, CancellationToken cancellationToken)
        {
            return await _priceRepository.GetPrice(request.PriceId);
        }
    }
}
