using Shared.DTO.User.Response;

namespace Shared.DTO.Address.Response;

public record GetAddressResponse
{
    public Guid AddressId { get; set; }
    public int Type { get; set; }
    public string Title { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Neighborhood { get; set; }
    public string AddressDetail { get; set; }
    public int TaxNo { get; set; }
    public string TaxOffice { get; set; }
    public string CompanyName { get; set; }
    
    public GetUserResponse User { get; set; }
};