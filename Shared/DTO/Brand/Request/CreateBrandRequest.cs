namespace Shared.DTO.Brand.Request;

public sealed record CreateBrandRequest(string Name, string Logo, Boolean ShowOnBrands, Boolean IsActive);
