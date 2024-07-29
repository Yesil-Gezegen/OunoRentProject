using MediatR;
using Shared.DTO.Authentication.Request;
using Shared.DTO.Authentication.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Authentication.Query;

public sealed record ValidateTokenQuery(ValidateTokenRequest ValidateTokenRequest) : IRequest<ValidateTokenResponse>;

public class ValidateTokenQueryHandler : IRequestHandler<ValidateTokenQuery, ValidateTokenResponse>
{
    private readonly IAuthService _authService;

    public ValidateTokenQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<ValidateTokenResponse> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        var result = _authService.ValidateToken(request.ValidateTokenRequest.Token);
        var validateTokenResponse = new ValidateTokenResponse()
        {
            ExpireTime = result
        };

        return Task.FromResult(validateTokenResponse);
    }
}
