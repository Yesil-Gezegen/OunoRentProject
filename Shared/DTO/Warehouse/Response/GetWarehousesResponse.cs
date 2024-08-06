namespace Shared.DTO.Warehouse.Response;

public class GetWarehousesResponse
{
    public Guid WarehouseId { get; set; }

    public string Name { get; set; }

    public int LogoWarehouseId { get; set; }

    public Boolean IsActive { get; set; }
}