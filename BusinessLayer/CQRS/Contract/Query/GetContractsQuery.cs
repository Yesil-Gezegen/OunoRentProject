using MediatR;
using Shared.DTO.Contract.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Contract.Query;

public sealed record GetContractsQuery() : IRequest<List<GetContractsResponse>>
{
    internal class GetContractsQueryHandler : IRequestHandler<GetContractsQuery, List<GetContractsResponse>>
    {
        private readonly IContractRepository _contractRepository;

        public GetContractsQueryHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<List<GetContractsResponse>> Handle(GetContractsQuery request, CancellationToken cancellationToken)
        {
            return await _contractRepository.GetContracts();
        }
    }
}