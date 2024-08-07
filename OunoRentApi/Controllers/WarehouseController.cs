using BusinessLayer.CQRS.Warehouse.Command;
using BusinessLayer.CQRS.Warehouse.Query;
using BusinessLayer.CQRS.WarehouseConnection.Command;
using BusinessLayer.CQRS.WarehouseConnection.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Warehouse.Request;
using Shared.DTO.WarehouseConnection.Request;

namespace OunoRentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehouseController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseRequest request)
    {
        var warehouse = await _mediator.Send(new CreateWarehouseCommand(request));

        return Ok(warehouse);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetWarehouses()
    {
        var warehouses = await _mediator.Send(new GetWarehousesQuery());

        return Ok(warehouses);
    }
    
    [HttpGet("{warehouseId:guid}")]
    public async Task<IActionResult> GetWarehouse(Guid warehouseId)
    {
        var warehouse = await _mediator.Send(new GetWarehouseQuery(warehouseId));

        return Ok(warehouse);
    }

    
    [HttpPut("{warehouseId:guid}")]
    public async Task<IActionResult> UpdateWarehouse([FromBody] UpdateWarehouseRequest request)
    {
        var warehouse = await  _mediator.Send(new UpdateWarehouseCommand(request));

        return Ok(warehouse);
    }
    
    [HttpDelete("{warehouseId:guid}")]
    public async Task<IActionResult> DeleteWarehouse(Guid warehouseId)
    {
        var warehouse = await _mediator.Send(new DeleteWarehouseCommand(warehouseId));

        return Ok(warehouse);
    }
    
      
    [HttpPost("warehouseconnection/")]
    public async Task<IActionResult> CreateWarehouseConnection([FromBody] CreateWarehouseConnectionRequest request)
    {
        var result = await _mediator.Send(new CreateWarehouseConnectionCommand(request));
        return Ok(result);
    }

    [HttpGet("warehouseconnection/")]
    public async Task<IActionResult> GetWarehouseConnections()
    {
        var result = await _mediator.Send(new GetWarehouseConnectionsQuery());
        
        return Ok(result);
    }
    
    [HttpGet("warehouseconnection/{warehouseConnectionId:guid}")]
    public async Task<IActionResult> GetWarehouseConnection(Guid warehouseConnectionId)
    {
        var result = await _mediator.Send(new GetWarehouseConnectionQuery(warehouseConnectionId));
        
        return Ok(result);
    }
    
    [HttpPut("warehouseconnection/{warehouseConnectionId:guid}")]
    public async Task<IActionResult> UpdateWarehouseConnection([FromBody] UpdateWarehouseConnectionRequest request)
    {
        var result = await _mediator.Send(new UpdateWarehouseConnectionCommand(request));
        
        return Ok(result);
    }

    [HttpDelete("warehouseconnection/{warehouseConnectionId:guid}")]
    public async Task<IActionResult> DeleteWarehouseConnection(Guid warehouseConnectionId)
    {
        var result = await _mediator.Send(new DeleteWarehouseConnectionCommand(warehouseConnectionId));

        return Ok(result);
    }
}