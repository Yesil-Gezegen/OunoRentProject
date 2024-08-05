namespace Shared.DTO.Contract.Request;

public sealed record CreateContractRequest(
    string Name,
    string Body,
    int Type,
    string RequiresAt,
    bool IsActive);
