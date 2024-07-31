using MediatR;
using Shared.DTO.Brand.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Brand.Query;

public sealed record GetBrandsQuery : IRequest<List<GetBrandsResponse>>
{
    internal class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, List<GetBrandsResponse>>
    {
        private readonly IBrandRepository _brandRepository;

        public GetBrandsQueryHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<List<GetBrandsResponse>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            return await _brandRepository.GetBrands();
        }
    }
}