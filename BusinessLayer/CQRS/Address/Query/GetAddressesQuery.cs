using MediatR;
using Shared.DTO.Address.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Address.Query;

public sealed record GetAddressesQuery() : IRequest<List<GetAddressesResponse>>;

public class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, List<GetAddressesResponse>>
{
    private readonly IAddressRepository _addressRepository;

    public GetAddressesQueryHandler(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<List<GetAddressesResponse>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        var addressResponse = await _addressRepository.GetAddressesAsync();
        return addressResponse;
    }
}