using Shared.DTO.Contract.Request;
using Shared.DTO.Contract.Response;

namespace Shared.Interface;

public interface IContractRepository
{
    Task<ContractResponse> CreateContract(CreateContractRequest createContractRequest);
    
    Task<List<GetContractsResponse>> GetContracts();
    
    Task<GetContractResponse> GetContract(Guid contractId); 
}