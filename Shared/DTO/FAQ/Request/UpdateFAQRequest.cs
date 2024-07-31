namespace Shared.DTO.FAQ.Request;

public record UpdateFAQRequest(
    Guid FAQId,
    string Label,
    string Text,
    int OrderNumber,
    bool IsActive
);