namespace Shared.DTO.Warehouse.Response;

public class GetWarehouseResponse
{
    public Guid WarehouseId { get; set; }

    public string Name { get; set; }

    public int LogoWarehouseId { get; set; }

    public Boolean IsActive { get; set; }
}