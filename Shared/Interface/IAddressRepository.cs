using System.Linq.Expressions;
using Shared.DTO.Address.Request;
using Shared.DTO.Address.Response;

namespace Shared.Interface;

public interface IAddressRepository
{
    Task<AddressResponse> CreateAddressAsync(CreateAddressRequest createAddressRequest);
    Task<AddressResponse> UpdateAddressAsync(UpdateAddressRequest updateAddressRequest);
    Task<AddressResponse> DeleteAddressAsync(Guid addressId);
    Task<List<GetAddressesResponse>> GetAddressesAsync(Expression<Func<GetAddressResponse, bool>>? predicate = null);
    Task<GetAddressResponse> GetAddressAsync(Guid addressId);
}