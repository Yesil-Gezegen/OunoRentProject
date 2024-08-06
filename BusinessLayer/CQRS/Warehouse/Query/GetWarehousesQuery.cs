using MediatR;
using Shared.DTO.Warehouse.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Warehouse.Query;

public sealed record GetWarehousesQuery : IRequest<List<GetWarehousesResponse>>
{
    internal class GetWarehousesQueryHandler : IRequestHandler<GetWarehousesQuery, List<GetWarehousesResponse>>
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public GetWarehousesQueryHandler(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public async Task<List<GetWarehousesResponse>> Handle(GetWarehousesQuery request, CancellationToken cancellationToken)
        {
            return await _warehouseRepository.GetWarehouses();
        }
    }
}