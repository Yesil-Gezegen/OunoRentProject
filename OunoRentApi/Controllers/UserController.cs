using BusinessLayer.CQRS.User.Command;
using BusinessLayer.CQRS.User.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.User.Request;

namespace OunoRentApi.Controllers;

[ApiController]
//[Authorize]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _mediator.Send(new GetUsersQuery());
        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var result = await _mediator.Send(new GetUserQuery(userId));
        return Ok(result);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var result = await _mediator.Send(new UpdateUserCommand(request));
        return Ok(result);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var result = await _mediator.Send(new DeleteUserCommand(userId));
        return Ok(result);
    }
}
