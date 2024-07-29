using BusinessLayer.CQRS.MenuItem.Command;
using BusinessLayer.CQRS.MenuItem.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.MenuItem.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetMenuItems()
    {
        var result = await _mediator.Send(new GetMenuItemsQuery());
        return Ok(result);
    }

    [HttpGet("{menuItemId:guid}")]
    public async Task<ActionResult> GetMenuItem(Guid menuItemId)
    {
        var result = await _mediator.Send(new GetMenuItemQuery(menuItemId));
        return Ok(result);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateMenuItem(CreateMenuItemRequest createMenuItemRequest)
    {
        var result = await _mediator.Send(new CreateMenuItemCommand(createMenuItemRequest));
        return Ok(result);
    }

    [HttpPut("{menuItemId:guid}")]
    public async Task<IActionResult> UpdateMenuItem([FromBody] UpdateMenuItemRequest updateMenuItemRequest)
    {
        var result = await _mediator.Send(new UpdateMenuItemCommand(updateMenuItemRequest));
        return Ok(result);
    }

    [HttpDelete("{menuItemId:guid}")]
    public async Task<IActionResult> DeleteMenuItem(Guid menuItemId)
    {
        var result = await _mediator.Send(new DeleteMenuItemommand(menuItemId));
        return Ok(result);
    }
}
