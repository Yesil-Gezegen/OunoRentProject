using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Shared.DTO.Category.Request;

public sealed record CreateCategoryRequest(
    string Name,
    string Description,
    IFormFile Icon,
    int OrderNumber,
    IFormFile ImageHorizontal,
    IFormFile ImageSquare);