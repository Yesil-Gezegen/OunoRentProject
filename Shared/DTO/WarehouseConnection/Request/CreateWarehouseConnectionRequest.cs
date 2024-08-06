namespace Shared.DTO.WarehouseConnection.Request;

public sealed record CreateWarehouseConnectionRequest(
    Guid WarehouseId,
    Guid ChannelId,
    Boolean IsActive);
