namespace Shared.DTO.Warehouse.Request;

public sealed record UpdateWarehouseRequest(Guid WarehouseId, string Name, int LogoWarehouseId, bool IsActive);
