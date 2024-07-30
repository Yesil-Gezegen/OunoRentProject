using BusinessLayer.CQRS.FooterItem.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.FooterItem.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FooterItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public FooterItemController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> CreateFooterItem(CreateFooterItemRequest createFooterItemRequest)
    {
        var result = await _mediator.Send(new CreateFooterItemCommand(createFooterItemRequest));
        return Ok(result);
    }
}