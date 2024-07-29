using MediatR;
using Shared.DTO.User.Request;
using Shared.DTO.User.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.User.Command;

public sealed record CreateUserCommand(CreateUserRequest Request) : IRequest<UserResponse>;

/// <summary>
/// <c>CreateUserCommand</c> komutunu işleyen bir işleyici sınıfıdır.
/// Kullanıcı oluşturma isteğini alır ve <see cref="IUserRepository"/> aracılığıyla kullanıcı oluşturma işlemini gerçekleştirir.
/// </summary>
internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.CreateUser(request.Request);
    }
}

