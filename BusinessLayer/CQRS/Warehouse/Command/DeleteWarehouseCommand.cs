using MediatR;
using Shared.DTO.Warehouse.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Warehouse.Command;

public sealed record DeleteWarehouseCommand(Guid WarehouseId) : IRequest<Guid>
{
    internal class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, Guid>
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public DeleteWarehouseCommandHandler(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public async Task<Guid> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
        {
            return await _warehouseRepository.DeleteWarehouse(request.WarehouseId);
        }
    }
}
