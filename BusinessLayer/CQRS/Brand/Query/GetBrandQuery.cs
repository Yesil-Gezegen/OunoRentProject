using MediatR;
using Shared.DTO.Brand.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Brand.Query;

public sealed record GetBrandQuery(Guid BrandId): IRequest<GetBrandResponse>
{
    internal class GetBrandQueryHandler : IRequestHandler<GetBrandQuery, GetBrandResponse>
    {
        private readonly IBrandRepository _brandRepository;

        public GetBrandQueryHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<GetBrandResponse> Handle(GetBrandQuery request, CancellationToken cancellationToken)
        {
            return await _brandRepository.GetBrand(request.BrandId);
        }
    }
}