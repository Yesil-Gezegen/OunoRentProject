using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Shared.DTO.Blog.Request;

public record CreateBlogRequest(
    string Title,
    string Body,
    string Tags,
    string Slug,
    int OrderNumber,
    Guid SubCategoryId,
    IFormFile LargeImage,
    IFormFile SmallImage,
    Boolean IsActive
);