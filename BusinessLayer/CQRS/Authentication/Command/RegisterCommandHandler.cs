using MediatR;
using Shared.DTO.Authentication.Request;
using Shared.DTO.User.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Authentication.Command;

public sealed record RegisterCommand(RegisterRequest RegisterRequest) : IRequest<UserResponse>;

/// <summary>
/// <c>RegisterCommand</c> komutunu işleyen bir işleyici sınıfıdır.
/// Kullanıcı kayıt isteğini alır ve <see cref="IAuthService"/> aracılığıyla kayıt işlemini gerçekleştirir.
/// </summary>
internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserResponse>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<UserResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request.RegisterRequest);
        return result;
    }
}
