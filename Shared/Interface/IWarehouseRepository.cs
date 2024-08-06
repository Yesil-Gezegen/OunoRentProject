using Shared.DTO.Warehouse.Request;
using Shared.DTO.Warehouse.Response;

namespace Shared.Interface;

public interface IWarehouseRepository
{
    Task<WarehouseResponse> CreateWarehouse(CreateWarehouseRequest createWarehouseRequest);
    
    Task<List<GetWarehousesResponse>> GetWarehouses();
    
    Task<GetWarehouseResponse> GetWarehouse(Guid warehouseId);
    
    Task<WarehouseResponse> UpdateWarehouse(UpdateWarehouseRequest updateWarehouseRequest);
    
    Task<Guid> DeleteWarehouse(Guid warehouseId);
}