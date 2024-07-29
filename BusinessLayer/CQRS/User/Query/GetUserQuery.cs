using MediatR;
using Shared.DTO.User.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.User.Query;

public sealed record GetUserQuery(Guid UserId) : IRequest<GetUserResponse>;

internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserById(request.UserId);
    }
}


