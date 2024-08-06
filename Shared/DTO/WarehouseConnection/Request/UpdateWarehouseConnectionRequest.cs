namespace Shared.DTO.WarehouseConnection.Request;

public sealed record UpdateWarehouseConnectionRequest(
    Guid WarehouseConnectionId,
    Guid WarehouseId,
    Guid ChannelId,
    Boolean IsActive);
