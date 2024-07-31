using MediatR;
using Shared.DTO.Brand.Request;
using Shared.DTO.Brand.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Brand.Command;

public sealed record CreateBrandCommand(CreateBrandRequest CreateBrandRequest) : IRequest<BrandResponse>
{
    internal class CreateBrandCommandHandler: IRequestHandler<CreateBrandCommand, BrandResponse>
    {
        private readonly IBrandRepository _brandRepository;

        public CreateBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<BrandResponse> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            return await _brandRepository.CreateBrand(request.CreateBrandRequest);
        }
    }
}