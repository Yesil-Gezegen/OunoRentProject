using MediatR;
using Shared.Interface;

namespace BusinessLayer.CQRS.WarehouseConnection.Command;

public sealed record DeleteWarehouseConnectionCommand(Guid WarehouseConnectionId) : IRequest<Guid>
{
    internal class DeleteWarehouseConnectionCommandHandler : IRequestHandler<DeleteWarehouseConnectionCommand, Guid>
    {
        private readonly IWarehouseConnectionRepository _warehouseConnectionRepository;

        public DeleteWarehouseConnectionCommandHandler(IWarehouseConnectionRepository warehouseConnectionRepository)
        {
            _warehouseConnectionRepository = warehouseConnectionRepository;
        }
        
        public async Task<Guid> Handle(DeleteWarehouseConnectionCommand request, CancellationToken cancellationToken)
        {
            return await _warehouseConnectionRepository.DeleteWarehouseConnection(request.WarehouseConnectionId);
        }
    }
}