using Microsoft.AspNetCore.Http;

namespace Shared.DTO.Brand.Request;

public sealed record UpdateBrandRequest(
    Guid BrandId, string Name, IFormFile? Logo, Boolean ShowOnBrands, Boolean IsActive);
