using MediatR;
using Shared.DTO.Address.Request;
using Shared.DTO.Address.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Address.Command;

public sealed record UpdateAddressCommand(UpdateAddressRequest UpdateAddressRequest) : IRequest<AddressResponse>;

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, AddressResponse>
{
    private readonly IAddressRepository _addressRepository;

    public UpdateAddressCommandHandler(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }
    
    public async Task<AddressResponse> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var updateResponse = await _addressRepository.UpdateAddressAsync(request.UpdateAddressRequest);
        return updateResponse;
    }
}