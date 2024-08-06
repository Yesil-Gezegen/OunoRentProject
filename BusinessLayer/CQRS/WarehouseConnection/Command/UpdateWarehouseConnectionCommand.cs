using MediatR;
using Shared.DTO.Warehouse.Request;
using Shared.DTO.Warehouse.Response;
using Shared.DTO.WarehouseConnection.Request;
using Shared.DTO.WarehouseConnection.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.WarehouseConnection.Command;

public sealed record UpdateWarehouseConnectionCommand(
    UpdateWarehouseConnectionRequest UpdateWarehouseConnectionRequest) : IRequest<WarehouseConnectionResponse>
{
    internal class UpdateWarehouseConnectionCommandHandler : 
        IRequestHandler<UpdateWarehouseConnectionCommand, WarehouseConnectionResponse>
    {
        private readonly IWarehouseConnectionRepository _warehouseConnectionRepository;

        public UpdateWarehouseConnectionCommandHandler(IWarehouseConnectionRepository warehouseConnectionRepository)
        {
            _warehouseConnectionRepository = warehouseConnectionRepository;
        }

        public async Task<WarehouseConnectionResponse> Handle(
            UpdateWarehouseConnectionCommand request, CancellationToken cancellationToken)
        {
            return await _warehouseConnectionRepository.UpdateWarehouseConnection(request.UpdateWarehouseConnectionRequest);
        }
    }
}