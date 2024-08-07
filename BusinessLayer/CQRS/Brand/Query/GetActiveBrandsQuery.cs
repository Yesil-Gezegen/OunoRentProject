using MediatR;
using Shared.DTO.Brand.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Brand.Query;

public sealed record GetActiveBrandsQuery() : IRequest<List<GetBrandsResponse>>
{
    public class GetActiveBrandsQueryHandler : IRequestHandler<GetActiveBrandsQuery, List<GetBrandsResponse>>
    {
        private readonly IBrandRepository _brandRepository;

        public GetActiveBrandsQueryHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<List<GetBrandsResponse>> Handle(GetActiveBrandsQuery request, CancellationToken cancellationToken)
        { 
            return await _brandRepository.GetBrands(brand => brand.IsActive);
        }
    }
}