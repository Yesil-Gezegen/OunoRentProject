using MediatR;
using Shared.DTO.Address.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Address.Command;

public sealed record DeleteAddressCommand(Guid addressId) : IRequest<AddressResponse>;

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, AddressResponse>
{
    private readonly IAddressRepository _addressRepository;

    public DeleteAddressCommandHandler(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }
    
    public async Task<AddressResponse> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var addressResponse = await _addressRepository.DeleteAddressAsync(request.addressId);
        return addressResponse;
    }
}