namespace Shared.DTO.Category.Request;

public sealed record CreateCategoryRequest(
string Name, string Description, string Icon, int OrderNumber, string ImageHorizontalUrl, string ImageSquareUrl);