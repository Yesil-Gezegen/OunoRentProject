namespace Shared.DTO.FAQ.Request;

public record CreateFAQRequest(
    string Label,
    string Text,
    int OrderNumber,
    bool IsActive
);