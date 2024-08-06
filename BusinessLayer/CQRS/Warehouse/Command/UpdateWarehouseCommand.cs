using MediatR;
using Shared.DTO.Warehouse.Request;
using Shared.DTO.Warehouse.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Warehouse.Command;

public sealed record UpdateWarehouseCommand(UpdateWarehouseRequest UpdateWarehouseRequest) : IRequest<WarehouseResponse>
{
    internal class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, WarehouseResponse>
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public UpdateWarehouseCommandHandler(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public async Task<WarehouseResponse> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            return await _warehouseRepository.UpdateWarehouse(request.UpdateWarehouseRequest);
        }
    }
}