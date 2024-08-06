namespace EntityLayer.Entities;

public class Warehouse : AuditTrailer
{
    public Guid WarehouseId { get; set; }

    public string Name { get; set; }

    public int LogoWarehouseId { get; set; }

    public Boolean IsActive { get; set; }

    public ICollection<WarehouseConnection> WarehouseConnections { get; set; }
}