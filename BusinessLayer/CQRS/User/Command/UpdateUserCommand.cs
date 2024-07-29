using MediatR;
using Shared.DTO.User.Request;
using Shared.DTO.User.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.User.Command;

public sealed record UpdateUserCommand(UpdateUserRequest user) : IRequest<UserResponse>;

/// <summary>
/// <c>UpdateUserCommand</c> komutunu işleyen bir işleyici sınıfıdır.
/// Kullanıcı güncelleme isteğini alır ve <see cref="IUserRepository"/> aracılığıyla kullanıcı güncelleme işlemini gerçekleştirir.
/// </summary>
internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.UpdateUser(request.user);
    }
}

