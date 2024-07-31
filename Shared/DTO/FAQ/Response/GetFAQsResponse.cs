namespace Shared.DTO.FAQ.Response;

public record GetFAQsResponse
{
    public Guid FAQId { get; set; }
    public string Label { get; set; }
    public int OrderNumber { get; set; }
    public bool IsActive { get; set; }
}