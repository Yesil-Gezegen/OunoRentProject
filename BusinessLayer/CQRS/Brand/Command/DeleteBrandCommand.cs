using MediatR;
using Shared.Interface;

namespace BusinessLayer.CQRS.Brand.Command;

public sealed record DeleteBrandCommand(Guid BrandId) : IRequest<Guid>
{
    internal class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Guid>
    {
        private readonly IBrandRepository _brandRepository;

        public DeleteBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<Guid> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            return await _brandRepository.DeleteBrand(request.BrandId);
        }
    }
}