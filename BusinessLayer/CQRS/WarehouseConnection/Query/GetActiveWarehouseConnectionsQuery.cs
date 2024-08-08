using MediatR;
using Shared.DTO.WarehouseConnection.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.WarehouseConnection.Query;

public sealed record GetActiveWarehouseConnectionsQuery() : IRequest<List<GetWarehouseConnectionsResponse>>
{
    internal class GetActiveWarehouseConnectionsQueryHandler :
        IRequestHandler<GetActiveWarehouseConnectionsQuery, List<GetWarehouseConnectionsResponse>>
    {
        private readonly IWarehouseConnectionRepository _warehouseConnectionRepository;

        public GetActiveWarehouseConnectionsQueryHandler(
            IWarehouseConnectionRepository warehouseConnectionRepository)
        {
            _warehouseConnectionRepository = warehouseConnectionRepository;
        }

        public async Task<List<GetWarehouseConnectionsResponse>> Handle(
            GetActiveWarehouseConnectionsQuery request, CancellationToken cancellationToken)
        {
            // Predicate doğrudan WarehouseConnection tipi için belirleniyor
            return await _warehouseConnectionRepository.GetWarehouseConnections(
                x=> x.IsActive);
        }
    }
}