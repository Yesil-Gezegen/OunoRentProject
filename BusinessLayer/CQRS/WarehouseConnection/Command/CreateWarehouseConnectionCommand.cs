using MediatR;
using Shared.DTO.WarehouseConnection.Request;
using Shared.DTO.WarehouseConnection.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.WarehouseConnection.Command;

public sealed record CreateWarehouseConnectionCommand(
    CreateWarehouseConnectionRequest CreateWarehouseConnectionRequest) : IRequest<WarehouseConnectionResponse>
{
    internal class CreateWarehouseConnectionCommandHandler : 
        IRequestHandler<CreateWarehouseConnectionCommand, WarehouseConnectionResponse>
    {
        private readonly IWarehouseConnectionRepository _warehouseConnectionRepository;

        public CreateWarehouseConnectionCommandHandler(IWarehouseConnectionRepository warehouseConnectionRepository)
        {
            _warehouseConnectionRepository = warehouseConnectionRepository;
        }

        public async Task<WarehouseConnectionResponse> Handle(
            CreateWarehouseConnectionCommand request, CancellationToken cancellationToken)
        {
            return await _warehouseConnectionRepository.CreateWarehouseConnection(
                request.CreateWarehouseConnectionRequest);
        }
    }
}