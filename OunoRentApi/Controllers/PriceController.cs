using BusinessLayer.CQRS.Price.Command;
using BusinessLayer.CQRS.Price.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Price.Request;

namespace OunoRentApi.Controllers;
[ApiController]
[Route("api/[controller]")]

public class PriceController : ControllerBase
{
    private readonly IMediator _mediator;

    public PriceController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<ActionResult> CreatePrice([FromBody] CreatePriceRequest createPriceRequest)
    {
        var result = await _mediator.Send(new CreatePriceCommand(createPriceRequest));
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult> GetPrices()
    {
        var result = await _mediator.Send(new GetPricesQuery());
        return Ok(result);
    }
    
    [HttpGet("{priceId:guid}")]
    public async Task<ActionResult> GetPrice(Guid priceId)
    {
        var result = await _mediator.Send(new GetPriceQuery(priceId));
        return Ok(result);
    }
    
    [HttpPut("{priceId:guid}")]
    public async Task<ActionResult> UpdatePrice([FromBody] UpdatePriceRequest updatePriceRequest)
    {
        var result = await _mediator.Send(new UpdatePriceCommand(updatePriceRequest));
        return Ok(result);
    }
    
    [HttpDelete("{priceId:guid}")]
    public async Task<ActionResult> DeletePrice(Guid priceId)
    {
        var result = await _mediator.Send(new DeletePriceCommand(priceId));
        return Ok(result);
    }
}