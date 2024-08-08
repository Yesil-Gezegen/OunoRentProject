using MediatR;
using Shared.DTO.UserContracts.Request;
using Shared.DTO.UserContracts.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.UserContract.Command;

public sealed record CreateUserContractCommand(CreateUserContractRequest CreateUserContractRequest)
    : IRequest<UserContractResponse>;
    
public class  CreateUserContractCommandHandler : IRequestHandler<CreateUserContractCommand, UserContractResponse>
{
    private readonly IUserContractRepository _userContractRepository;

    public CreateUserContractCommandHandler(IUserContractRepository userContractRepository)
    {
        _userContractRepository = userContractRepository;
    }
    
    public async Task<UserContractResponse> Handle(CreateUserContractCommand request, CancellationToken cancellationToken)
    {
        var userContractResponse =
            await _userContractRepository.CreateUserContractAsync(request.CreateUserContractRequest);
        return userContractResponse;
    }
}