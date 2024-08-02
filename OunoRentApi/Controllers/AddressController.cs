using BusinessLayer.CQRS.Address.Command;
using BusinessLayer.CQRS.Address.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Address.Request;

namespace OunoRentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly IMediator _mediator;

    public AddressController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAddressesAsync()
    {
        var result = await _mediator.Send(new GetAddressesQuery());
        return Ok(result);
    }

    [HttpGet("{addressId:guid}")]
    public async Task<IActionResult> GetAddressAsync(Guid addressId)
    {
        var result = await _mediator.Send(new GetAddressQuery(addressId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAddressAsync(CreateAddressRequest createAddressRequest)
    {
        var result = await _mediator.Send(new CreateAddressCommand(createAddressRequest));
        return Ok(result);
    }

    [HttpPut("{addressId:guid}")]
    public async Task<IActionResult> UpdateAddressAsync(Guid addressId,
        [FromBody] UpdateAddressRequest updateAddressRequest)
    {
        var result = await _mediator.Send(new UpdateAddressCommand(updateAddressRequest));
        return Ok(result);
    }

    [HttpDelete("{addressId:guid}")]
    public async Task<IActionResult> DeleteAddressAsync(Guid addressId)
    {
        var result = await _mediator.Send(new DeleteAddressCommand(addressId));
        return Ok(result);
    }
}