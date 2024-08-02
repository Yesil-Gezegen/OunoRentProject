using MediatR;
using Shared.DTO.Address.Request;
using Shared.DTO.Address.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Address.Command;

public sealed record CreateAddressCommand(CreateAddressRequest CreateAddressRequest) : IRequest<AddressResponse>;

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressResponse>
{
    private readonly IAddressRepository _addressRepository;

    public CreateAddressCommandHandler(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }
    
    public async Task<AddressResponse> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var addressResponse = await _addressRepository.CreateAddressAsync(request.CreateAddressRequest);
        return addressResponse;
    }
}