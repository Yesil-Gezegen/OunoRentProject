namespace Shared.DTO.Warehouse.Request;

public sealed record CreateWarehouseRequest(string Name, int LogoWarehouseId, bool IsActive);
