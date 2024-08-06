using BusinessLayer.CQRS.WarehouseConnection.Command;
using BusinessLayer.CQRS.WarehouseConnection.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.WarehouseConnection.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseConnectionController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehouseConnectionController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateWarehouseConnection([FromBody] CreateWarehouseConnectionRequest request)
    {
        var result = await _mediator.Send(new CreateWarehouseConnectionCommand(request));
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetWarehouseConnections()
    {
        var result = await _mediator.Send(new GetWarehouseConnectionsQuery());
        
        return Ok(result);
    }
    
    [HttpGet("{warehouseConnectionId:guid}")]
    public async Task<IActionResult> GetWarehouseConnection(Guid warehouseConnectionId)
    {
        var result = await _mediator.Send(new GetWarehouseConnectionQuery(warehouseConnectionId));
        
        return Ok(result);
    }
    
    [HttpPut("{warehouseConnectionId:guid}")]
    public async Task<IActionResult> UpdateWarehouseConnection([FromBody] UpdateWarehouseConnectionRequest request)
    {
        var result = await _mediator.Send(new UpdateWarehouseConnectionCommand(request));
        
        return Ok(result);
    }

    [HttpDelete("{warehouseConnectionId:guid}")]
    public async Task<IActionResult> DeleteWarehouseConnection(Guid warehouseConnectionId)
    {
        var result = await _mediator.Send(new DeleteWarehouseConnectionCommand(warehouseConnectionId));

        return Ok(result);
    }
}