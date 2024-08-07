using MediatR;
using Shared.DTO.Warehouse.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Warehouse.Query;

public sealed record GetActiveWarehousesQuery() : IRequest<List<GetWarehousesResponse>>
{
    internal class GetActiveWarehouseQueryHandler : IRequestHandler<GetActiveWarehousesQuery, List<GetWarehousesResponse>>
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public GetActiveWarehouseQueryHandler(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public async Task<List<GetWarehousesResponse>> Handle(GetActiveWarehousesQuery request, CancellationToken cancellationToken)
        {
            return await _warehouseRepository.GetWarehouses(x => x.IsActive);
        }
    }
}