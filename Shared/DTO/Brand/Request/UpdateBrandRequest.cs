namespace Shared.DTO.Brand.Request;

public sealed record UpdateBrandRequest(
    Guid BrandId, string Name, string Logo, Boolean ShowOnBrands, Boolean IsActive);
