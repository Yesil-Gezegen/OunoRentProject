using BusinessLayer.CQRS.UserContract.Command;
using BusinessLayer.CQRS.UserContract.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.UserContracts.Request;

namespace OunoRentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserContractController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserContractController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUserContractsAsync()
    {
        var result = await _mediator.Send(new GetUserContractsQuery());
        return Ok(result);
    }

    [HttpGet("{userContractId:guid}")]
    public async Task<IActionResult> GetUserContractAsync(Guid userContractId)
    {
        var result = await _mediator.Send(new GetUserContractQuery(userContractId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserContractAsync(CreateUserContractRequest createUserContractRequest)
    {
        var result = await _mediator.Send(new CreateUserContractCommand(createUserContractRequest));
        return Ok(result);
    }
}