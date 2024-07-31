using MediatR;
using Shared.DTO.Brand.Request;
using Shared.DTO.Brand.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Brand.Command;

public sealed record UpdateBrandCommand(UpdateBrandRequest UpdateBrandRequest) : IRequest<BrandResponse>
{
    internal class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, BrandResponse>
    {
        private readonly IBrandRepository _brandRepository;

        public UpdateBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<BrandResponse> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            return await _brandRepository.UpdateBrand(request.UpdateBrandRequest);
        }
    }
}