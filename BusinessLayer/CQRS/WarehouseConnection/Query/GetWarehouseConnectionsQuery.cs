using MediatR;
using Shared.DTO.WarehouseConnection.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.WarehouseConnection.Query;

public sealed record GetWarehouseConnectionsQuery : IRequest<List<GetWarehouseConnectionsResponse>>
{
    internal class GetWarehouseConnectionsQueryHandler : IRequestHandler<GetWarehouseConnectionsQuery, List<GetWarehouseConnectionsResponse>>
    {
        private readonly IWarehouseConnectionRepository _warehouseConnectionRepository;

        public GetWarehouseConnectionsQueryHandler(IWarehouseConnectionRepository warehouseConnectionRepository)
        {
            _warehouseConnectionRepository = warehouseConnectionRepository;
        }

        public async Task<List<GetWarehouseConnectionsResponse>> Handle(GetWarehouseConnectionsQuery request, CancellationToken cancellationToken)
        {
            return await _warehouseConnectionRepository.GetWarehouseConnections();
        }
    }
}