using MediatR;
using Shared.DTO.Address.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Address.Query;

public sealed record GetAddressQuery(Guid addressId) : IRequest<GetAddressResponse>;

public class GetAddressQueryHandler : IRequestHandler<GetAddressQuery, GetAddressResponse>
{
    private readonly IAddressRepository _addressRepository;

    public GetAddressQueryHandler(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }
    
    public async Task<GetAddressResponse> Handle(GetAddressQuery request, CancellationToken cancellationToken)
    {
        var addressResponse = await _addressRepository.GetAddressAsync(request.addressId);
        return addressResponse;
    }
}