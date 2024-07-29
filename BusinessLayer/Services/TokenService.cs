using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.DTO.Authentication.Response;
using Shared.Interface;

namespace BusinessLayer.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public TokenService(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    public async Task<string> GenerateTokenAsync(UserDetailsResponse userDetailResponse)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims(userDetailResponse);
        var tokenOptions = GetTokenOptions(signingCredentials, claims);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return token;
    }

    public async Task<string?> RefreshTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var jwtSettings = _configuration.GetSection("JWT");

        if (!double.TryParse(jwtSettings["SlidingExpireTime"], out double slidingExpireMinute))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return null;

        var tokenExpireTime = jwtToken
            .ValidTo
            .ToLocalTime();
        var refreshTokenExpireTime = tokenExpireTime
            .AddMinutes(slidingExpireMinute);

        if (DateTime.Now < tokenExpireTime
            || DateTime.Now > refreshTokenExpireTime)
        {
            return null;
        }

        var principal = GetPrincipalFromExpiredToken(token);
        var userEmail = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userEmail))
            return null;

        var user = await _userRepository.GetUserByEmail(userEmail);
        if (user == null)
            return null;

        var newToken = await GenerateTokenAsync(user);
        return newToken;
    }

    public ClaimsPrincipal? GetPrincipal(string token)
    {
        try
        {
            var validationParameters = GetValidationParameters();
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            return null;
        }
    }

    public DateTime? ValidateTokenExpiry(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return null;

        var tokenExpireTime = jwtToken
            .ValidTo
            .ToLocalTime();

        return tokenExpireTime;
    }

    /// <summary>
    /// Extracts a <see cref="ClaimsPrincipal"/> from a given JWT token without validating its expiration or signature.
    /// </summary>
    /// <param name="token">The JWT token to extract the principal from.</param>
    /// <returns>
    /// A <see cref="ClaimsPrincipal"/> representing the claims in the token, or <c>null</c> if the token is invalid.
    /// </returns>
    /// <remarks>
    /// This method is useful for scenarios where you need to access the claims in an expired token.
    /// Note that this method does not perform any validation on the token's signature or expiration date.
    /// Use this method with caution and ensure that you only use it in trusted environments.
    /// </remarks>
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return null;

        var claims = jwtToken.Claims;
        var identity = new ClaimsIdentity(claims, "jwt");
        var principal = new ClaimsPrincipal(identity);

        return principal;
    }

    /// <summary>
    /// JWT token doğrulama parametrelerini döner.
    /// </summary>
    /// <returns>JWT token doğrulama parametrelerini içeren TokenValidationParameters nesnesi.</returns>
    private TokenValidationParameters GetValidationParameters()
    {
        var jwtSettings = _configuration.GetSection("JWT");
        var key = jwtSettings["Key"];

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        };

        return tokenValidationParameters;
    }

    /// <summary>
    /// JWT token oluşturma seçeneklerini döner.
    /// </summary>
    /// <param name="signingCredentials">Token imzalama bilgilerini içeren SigningCredentials nesnesi.</param>
    /// <param name="claims">JWT token'a eklenecek claim listesi.</param>
    /// <returns>Oluşturulan JWT token seçeneklerini içeren SecurityToken nesnesi.</returns>
    private SecurityToken GetTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JWT");

        var tokenOptions = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireTime"])),
            signingCredentials: signingCredentials
        );

        return tokenOptions;
    }

    /// <summary>
    /// Verilen kullanıcı detaylarına göre claim listesini oluşturur.
    /// </summary>
    /// <param name="loginUserResponse">Kullanıcı giriş detaylarını içeren nesne.</param>
    /// <returns>Kullanıcıya ait claim'leri içeren bir liste döner.</returns>
    private async Task<List<Claim>> GetClaims(UserDetailsResponse loginUserResponse)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, loginUserResponse.Email)
        };

        return claims;
    }

    /// <summary>
    /// JWT token imzalama bilgilerini oluşturur ve döner.
    /// </summary>
    /// <returns>JWT token imzalama bilgilerini içeren SigningCredentials nesnesi.</returns>
    private SigningCredentials GetSigningCredentials()
    {
        var jwtSettings = _configuration.GetSection("JWT");
        var key = jwtSettings["Key"];
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    }
}