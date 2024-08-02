namespace Shared.DTO.UserContracts.Request;

public record CreateUserContractRequest(
    Guid UserId,
    Guid ContractId,
    string FileName
);