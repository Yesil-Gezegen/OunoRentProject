using System.Linq.Expressions;
using Shared.DTO.UserContracts.Request;
using Shared.DTO.UserContracts.Response;

namespace Shared.Interface;

public interface IUserContractRepository
{
    Task<UserContractResponse> CraateUserContractAsync(CreateUserContractRequest createUserContractRequest);
    Task<GetUserContractResponse> GetUserContractAsync(Guid userContractId);
    Task<List<GetUserContractsResponse>> GetUserContractsAsync(
        Expression<Func<GetUserContractResponse, bool>>? predicate = null);
}