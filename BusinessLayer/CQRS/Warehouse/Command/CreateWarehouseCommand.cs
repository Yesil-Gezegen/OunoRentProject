using MediatR;
using Shared.DTO.Warehouse.Request;
using Shared.DTO.Warehouse.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Warehouse.Command;

public sealed record CreateWarehouseCommand(CreateWarehouseRequest CreateWarehouseRequest) : IRequest<WarehouseResponse>
{
    internal class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, WarehouseResponse>
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public CreateWarehouseCommandHandler(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public async Task<WarehouseResponse> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            return await _warehouseRepository.CreateWarehouse(request.CreateWarehouseRequest);
        }
    }
}