namespace Shared.DTO.WarehouseConnection.Response;

public class GetWarehouseConnectionResponse
{
    public Guid WarehouseConnectionId { get; set; }

    public string WarehouseName { get; set; }

    public string ChannelName { get; set; }

    public Boolean IsActive { get; set; }
}