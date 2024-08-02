using MediatR;
using Shared.DTO.Price.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Price.Query;

public sealed record GetPricesQuery : IRequest<List<GetPricesResponse>>
{
    internal class GetPricesQueryHandler : IRequestHandler<GetPricesQuery, List<GetPricesResponse>>
    {
        private readonly IPriceRepository _priceRepository;

        public GetPricesQueryHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public async Task<List<GetPricesResponse>> Handle(GetPricesQuery request, CancellationToken cancellationToken)
        {
            return await _priceRepository.GetPrices();
        }
    }
}
