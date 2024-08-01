using MediatR;
using Shared.Interface;

namespace BusinessLayer.CQRS.Contract.Command;

public sealed record DeleteContractCommand(Guid ContractId) : IRequest<Guid>
{
    internal class DeleteContractCommandHandler : IRequestHandler<DeleteContractCommand, Guid>
    {
        private readonly IContractRepository _contractRepository;

        public DeleteContractCommandHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<Guid> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
        {
            return await _contractRepository.DeleteContract(request.ContractId);
        }
    }
}