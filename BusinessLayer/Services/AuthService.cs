using BusinessLayer.Middlewares;
using BusinessLayer.Utilities;
using Shared.DTO.Authentication.Request;
using Shared.DTO.User.Request;
using Shared.DTO.User.Response;
using Shared.Interface;

namespace BusinessLayer.Services;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public AuthService(ITokenService tokenService, IUserRepository userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task<string> LoginAsync(LoginRequest loginRequest)
    {
        var isUserExist = await _userRepository.IsExistAsync(loginRequest.Email);

        if (!isUserExist)
            throw new NotFoundException(AuthenticationExceptionMessage.UserNotFound);

        var user = await _userRepository.GetUserByEmail(loginRequest.Email);
        var isVerify = PasswordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash);

        if (!isVerify)
            throw new BadRequestException(AuthenticationExceptionMessage.WrongPassword);

        var token = await _tokenService.GenerateTokenAsync(user);
        return token;
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest registerRequest)
    {
        var isUserExist = await _userRepository.IsExistAsync(registerRequest.Email);

        if (isUserExist)
            throw new ConflictException(AuthenticationExceptionMessage.EmailAlreadyExist);

        var hashPassword = PasswordHasher.HashPassword(registerRequest.Password);
        var createUserRequest = new CreateUserRequest(registerRequest.Email, hashPassword);

        var result = await _userRepository.CreateUser(createUserRequest);
        return result;
    }

    public DateTime? ValidateToken(string token)
    {
        var result = _tokenService.GetPrincipal(token);

        if (result != null)
        {
            var expireTime = _tokenService.ValidateTokenExpiry(token);

            if (expireTime != null)
                return expireTime;
        }
        return null;
    }
}