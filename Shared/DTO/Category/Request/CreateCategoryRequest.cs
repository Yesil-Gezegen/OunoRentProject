using System.Text.Json.Serialization;

namespace Shared.DTO.Category.Request;

public sealed record CreateCategoryRequest(
string Name, string Description, string Icon, int OrderNumber, [property: JsonIgnore]string ImageHorizontalUrl, string ImageSquareUrl);