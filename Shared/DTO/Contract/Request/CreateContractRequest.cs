namespace Shared.DTO.Contract.Request;

public sealed record CreateContractRequest(
    string Name,
    int Version,
    int PreviousVersion,
    string Body,
    int Type,
    DateTime CreateDate,
    string RequiresAt,
    bool IsActive);
