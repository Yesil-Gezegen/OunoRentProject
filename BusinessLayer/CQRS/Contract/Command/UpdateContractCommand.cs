using MediatR;
using Shared.DTO.Contract.Request;
using Shared.DTO.Contract.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Contract.Command;

public sealed record UpdateContractCommand(UpdateContractRequest UpdateContractRequest) : IRequest<ContractResponse>
{
    internal class UpdateContractCommandHandler : IRequestHandler<UpdateContractCommand, ContractResponse>
    {
        private readonly IContractRepository _contractRepository;

        public UpdateContractCommandHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<ContractResponse> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
        {
            return await _contractRepository.UpdateContract(request.UpdateContractRequest);
        }
    }
}