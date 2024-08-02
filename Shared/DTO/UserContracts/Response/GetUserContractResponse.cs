using Shared.DTO.Contract.Response;
using Shared.DTO.User.Response;

namespace Shared.DTO.UserContracts.Response;

public record GetUserContractResponse
{
    public Guid UserContractId { get; set; }
    public string FileName { get; set; }
    public DateTime Date { get; set; }
    
    public GetUserResponse User { get; set; }
    public GetContractResponse Contract { get; set; }
}