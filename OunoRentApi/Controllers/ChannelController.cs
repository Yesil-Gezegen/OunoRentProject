using BusinessLayer.CQRS.Channel.Command;
using BusinessLayer.CQRS.Channel.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Channel.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChannelController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChannelController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
    public async Task<IActionResult> CreateChannel([FromForm] CreateChannelRequest request)
    {
        var channel = await _mediator.Send(new CreateChannelCommand(request));
        
        return Ok(channel);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetChannels()
    {
        var channels =  await _mediator.Send(new GetChannelsQuery());
        
        return Ok(channels);
    }
    
    [HttpGet("{channelId:guid}")]
    public async Task<IActionResult> GetChannel(Guid channelId)
    {
        var channel = await _mediator.Send(new GetChannelQuery(channelId));
        
        return Ok(channel);
    }
    
    [HttpPut("{channelId:guid}")]
    public async Task<IActionResult> UpdateChannel([FromForm] UpdateChannelRequest request)
    {
        var channel = await _mediator.Send(new UpdateChannelCommand(request));
        
        return Ok(channel);
    }
    
    [HttpDelete("{channelId:guid}")]
    public async Task<IActionResult> DeleteChannel(Guid channelId)
    {
        await _mediator.Send(new DeleteChannelCommand(channelId));
        
        return Ok();
    }
}