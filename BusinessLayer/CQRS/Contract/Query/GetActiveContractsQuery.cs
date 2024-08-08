using MediatR;
using Shared.DTO.Contract.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Contract.Query;

public sealed record GetActiveContractsQuery() : IRequest<List<GetContractsResponse>>
{
    internal class GetActiveContractsQueryHandler : IRequestHandler<GetActiveContractsQuery, List<GetContractsResponse>>
    {
        private readonly IContractRepository _contractRepository;

        public GetActiveContractsQueryHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async  Task<List<GetContractsResponse>> Handle(
            GetActiveContractsQuery request, CancellationToken cancellationToken)
        {
            return await _contractRepository.GetContracts(c => c.IsActive);
        }
    }
}