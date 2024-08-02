using MediatR;
using Shared.DTO.UserContracts.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.UserContract.Query;

public sealed record GetUserContractQuery(Guid userContractId) : IRequest<GetUserContractResponse>;

public class GetUserContractQueryHandler : IRequestHandler<GetUserContractQuery, GetUserContractResponse>
{
    private readonly IUserContractRepository _userContractRepository;

    public GetUserContractQueryHandler(IUserContractRepository userContractRepository)
    {
        _userContractRepository = userContractRepository;
    }
    
    public async Task<GetUserContractResponse> Handle(GetUserContractQuery request, CancellationToken cancellationToken)
    {
        var userContractResponse = await _userContractRepository.GetUserContractAsync(request.userContractId);
        return userContractResponse;
    }
}