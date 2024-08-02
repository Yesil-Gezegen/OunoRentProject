using Microsoft.AspNetCore.Http;

namespace Shared.DTO.Category.Request;

public sealed record UpdateCategoryRequest(
    Guid CategoryId,
    string Name,
    string Description,
    int OrderNumber,
    IFormFile? Icon,
    IFormFile? ImageHorizontal,
    IFormFile? ImageSquare
);