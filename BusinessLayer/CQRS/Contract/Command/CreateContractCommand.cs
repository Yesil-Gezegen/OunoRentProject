using MediatR;
using Shared.DTO.Contract.Request;
using Shared.DTO.Contract.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Contract.Command;

public sealed record CreateContractCommand(CreateContractRequest CreateContractRequest) : IRequest<ContractResponse>
{
    internal class CreateContractCommandHandler : IRequestHandler<CreateContractCommand , ContractResponse>
    {
        private readonly IContractRepository _contractRepository;

        public CreateContractCommandHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<ContractResponse> Handle(CreateContractCommand request, CancellationToken cancellationToken)
        {
            return await _contractRepository.CreateContract(request.CreateContractRequest);
        }
    }
}