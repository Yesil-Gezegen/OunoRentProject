namespace EntityLayer.Entities;

public class WarehouseConnection : AuditTrailer
{
    public Guid WarehouseConnectionId { get; set; }

    public Boolean IsActive { get; set; }
    
    // Relationship
    
    public Guid WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; }

    public Guid ChannelId { get; set; }

    public Channel Channel { get; set; }
}