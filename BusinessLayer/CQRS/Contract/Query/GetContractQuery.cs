using MediatR;
using Shared.DTO.Contract.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Contract.Query;

public sealed record GetContractQuery(Guid ContractId) : IRequest<GetContractResponse>
{
    internal class GetContractQueryHandler : IRequestHandler<GetContractQuery, GetContractResponse>
    {
        private readonly IContractRepository _contractRepository;

        public GetContractQueryHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<GetContractResponse> Handle(GetContractQuery request, CancellationToken cancellationToken)
        {
            return await _contractRepository.GetContract(request.ContractId);
        }
    }
}