using BusinessLayer.CQRS.Warehouse.Command;
using BusinessLayer.CQRS.Warehouse.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Warehouse.Request;

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
}