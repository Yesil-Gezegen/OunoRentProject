using MediatR;
using Shared.DTO.UserContracts.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.UserContract.Query;

public sealed record GetUserContractsQuery : IRequest<List<GetUserContractsResponse>>;

public class GetUserContractsQueryHandler : IRequestHandler<GetUserContractsQuery, List<GetUserContractsResponse>>
{
    private readonly IUserContractRepository _userContractRepository;

    public GetUserContractsQueryHandler(IUserContractRepository userContractRepository)
    {
        _userContractRepository = userContractRepository;
    }
    
    public async Task<List<GetUserContractsResponse>> Handle(GetUserContractsQuery request, CancellationToken cancellationToken)
    {
        var userContractResponse = await _userContractRepository.GetUserContractsAsync();
        return userContractResponse;
    }
}