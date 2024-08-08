using System.Linq.Expressions;
using Shared.DTO.Warehouse.Request;
using Shared.DTO.Warehouse.Response;
using Shared.DTO.WarehouseConnection.Request;
using Shared.DTO.WarehouseConnection.Response;

namespace Shared.Interface;

public interface IWarehouseConnectionRepository
{
    Task<WarehouseConnectionResponse> CreateWarehouseConnection(
        CreateWarehouseConnectionRequest createWarehouseConnectionRequest);

    Task<List<GetWarehouseConnectionsResponse>> GetWarehouseConnections(
        Expression<Func<GetWarehouseConnectionsResponse, bool>>? predicate = null);
    
    Task<GetWarehouseConnectionResponse> GetWarehouseConnection(Guid warehouseConnectionId);
    
    Task<WarehouseConnectionResponse> UpdateWarehouseConnection(
        UpdateWarehouseConnectionRequest updateWarehouseConnectionRequest);
    
    Task<Guid> DeleteWarehouseConnection(Guid warehouseConnectionId);
}