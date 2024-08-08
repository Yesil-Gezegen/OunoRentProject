using BusinessLayer.ActionFilters;
using BusinessLayer.CQRS.Authentication.Command;
using BusinessLayer.CQRS.Authentication.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Authentication.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        try
        {
            var token = await _mediator.Send(new LoginCommand(loginRequest));
            return Ok(token);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("register")]
    [ValidateModel]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        try
        {
            var result = await _mediator.Send(new RegisterCommand(registerRequest));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("validateToken")]
    public async Task<IActionResult> ValidateToken()
    {
        try
        {
            if (!HttpContext.Request.Headers.ContainsKey("Authorization"))
                return Unauthorized();

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var result = await _mediator.Send(
                new ValidateTokenQuery(
                    new ValidateTokenRequest { Token = token.ToString() }));

            if (result.ExpireTime == null)
                return Unauthorized();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
