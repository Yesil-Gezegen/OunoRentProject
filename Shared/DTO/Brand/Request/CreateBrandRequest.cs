using Microsoft.AspNetCore.Http;

namespace Shared.DTO.Brand.Request;

public sealed record CreateBrandRequest(string Name, IFormFile Logo, Boolean ShowOnBrands, Boolean IsActive);