using MediatR;
using Shared.DTO.WarehouseConnection.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.WarehouseConnection.Query;

public sealed record GetWarehouseConnectionQuery(Guid GetWarehouseConnectionId) : IRequest<GetWarehouseConnectionResponse>
{
    internal class GetWarehouseConnectionQueryHandler : IRequestHandler<GetWarehouseConnectionQuery, GetWarehouseConnectionResponse>
    {
        private readonly IWarehouseConnectionRepository _warehouseConnectionRepository;

        public GetWarehouseConnectionQueryHandler(IWarehouseConnectionRepository warehouseConnectionRepository)
        {
            _warehouseConnectionRepository = warehouseConnectionRepository;
        }

        public async Task<GetWarehouseConnectionResponse> Handle(GetWarehouseConnectionQuery request, CancellationToken cancellationToken)
        {
            return await _warehouseConnectionRepository.GetWarehouseConnection(request.GetWarehouseConnectionId);
        }
    }
}