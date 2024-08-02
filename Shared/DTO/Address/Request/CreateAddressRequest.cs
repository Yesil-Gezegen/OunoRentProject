namespace Shared.DTO.Address.Request;

public record CreateAddressRequest(
    int Type,
    string Title,
    string City,
    string District,
    string Neighborhood,
    string AddressDetail,
    int? TaxNo,
    string? TaxOffice,
    string? CompanyName,
    Guid UserId
);