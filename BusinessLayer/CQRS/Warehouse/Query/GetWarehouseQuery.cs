using MediatR;
using Shared.DTO.Warehouse.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Warehouse.Query;

public sealed record GetWarehouseQuery(Guid WarehouseId) : IRequest<GetWarehouseResponse>
{
    internal class GetWarehouseQueryHandler : IRequestHandler<GetWarehouseQuery, GetWarehouseResponse>
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public GetWarehouseQueryHandler(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public async Task<GetWarehouseResponse> Handle(GetWarehouseQuery request, CancellationToken cancellationToken)
        {
            return await _warehouseRepository.GetWarehouse(request.WarehouseId);
        }
    }
}