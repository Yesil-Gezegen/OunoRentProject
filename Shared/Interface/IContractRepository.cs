using System.Linq.Expressions;
using Shared.DTO.Contract.Request;
using Shared.DTO.Contract.Response;

namespace Shared.Interface;

public interface IContractRepository
{
    Task<ContractResponse> CreateContract(CreateContractRequest createContractRequest);
    
    Task<List<GetContractsResponse>> GetContracts(Expression<Func<GetContractsResponse, bool>>? predicate = null);
    
    Task<GetContractResponse> GetContract(Guid contractId); 
}