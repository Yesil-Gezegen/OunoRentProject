using BusinessLayer.CQRS.FooterItem.Command;
using BusinessLayer.CQRS.FooterItem.Query;
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
    
    [HttpGet("")]
    public async Task<IActionResult> GetFooterItems()
    {
        var result = await _mediator.Send(new GetFooterItemsQuery());
        return Ok(result);
    }
    
    [HttpGet("getactive")]
    public async Task<IActionResult> GetActiveFooterItems()
    {
        var result = await _mediator.Send(new GetActiveFooterItemsQuery());
        return Ok(result);
    }
    
    [HttpGet("{footerItemId:guid}")]
    public async Task<IActionResult> GetFooterItem(Guid footerItemId)
    {
        var result = await _mediator.Send(new GetFooterItemQuery(footerItemId));
        return Ok(result);
    }
    
    [HttpPut("{footerItemId:guid}")]
    public async Task<IActionResult> UpdateFooterItem(UpdateFooterItemRequest updateFooterItemRequest)
    {
        var result = await _mediator.Send(new UpdateFooterItemCommand(updateFooterItemRequest));
        return Ok(result);
    }
    
    [HttpDelete("{footerItemId:guid}")]
    public async Task<IActionResult> DeleteFooterItem(Guid footerItemId)
    {
        var result = await _mediator.Send(new DeleteFooterItemCommand(footerItemId));
        return Ok(result);
    }
}