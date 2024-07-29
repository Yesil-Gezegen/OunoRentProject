using Microsoft.AspNetCore.Http;
using Shared.Interface;

namespace BusinessLayer.Middlewares;

/// <summary>
/// İstekler için kayan sona erme süresi olan token'ları yönetir.
/// Gelen isteklerdeki Authorization başlığını kontrol eder ve gerekirse token'ı yeniler.
/// Yenilenmiş token'ı yanıt başlıklarına ekler.
/// </summary>
/// <param name="context">HTTP bağlamı.</param>
/// <param name="_tokenService">Token hizmeti.</param>
/// <returns>Asenkron görev.</returns>
public class SlidingExpirationMiddleware
{
    private readonly RequestDelegate _next;

    public SlidingExpirationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService _tokenService)
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var newToken = await _tokenService.RefreshTokenAsync(token);
            if (newToken != null)
            {
                context.Response.Headers.Append("New-Token", newToken);
                context.Request.Headers["Authorization"] = "Bearer " + newToken;
            }

        }

        await _next(context);
    }
}
