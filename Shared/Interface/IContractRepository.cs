using Shared.DTO.Contract.Request;
using Shared.DTO.Contract.Response;

namespace Shared.Interface;

public interface IContractRepository
{
    Task<ContractResponse> CreateContract(CreateContractRequest createContractRequest);
    
    Task<ContractResponse> UpdateContract(UpdateContractRequest updateContractRequest);
    
    Task<List<GetContractsResponse>> GetContracts();
    
    Task<GetContractResponse> GetContract(Guid contractId);
    
    Task<Guid> DeleteContract(Guid contractId);
}