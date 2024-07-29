namespace Shared.DTO.Category.Request;

public sealed record UpdateCategoryRequest(
    Guid CategoryId,
    string Name, string Description, string Icon, int OrderNumber, string ImageHorizontalUrl, string ImageSquareUrl
);

