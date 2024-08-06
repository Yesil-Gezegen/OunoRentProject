namespace Shared.DTO.WarehouseConnection.Response;

public class GetWarehouseConnectionsResponse
{
    public Guid WarehouseConnectionId { get; set; }

    public string WarehouseName { get; set; }

    public string ChannelName { get; set; }

    public Boolean IsActive { get; set; }
}