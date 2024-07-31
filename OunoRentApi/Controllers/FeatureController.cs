using BusinessLayer.CQRS.Feature.Command;
using BusinessLayer.CQRS.Feature.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Feature.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeatureController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeatureController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetFeaturesAsync()
    {
        var result = await _mediator.Send(new GetFeaturesQuery());
        return Ok(result);
    }

    [HttpGet("GetActive")]
    public async Task<IActionResult> GetActiveFeaturesAsync()
    {
        var result = await _mediator.Send(new GetActiveFeaturesQuery());
        return Ok(result);
    }

    [HttpGet("{featureId:guid}")]
    public async Task<IActionResult> GetFeatureAsync(Guid featureId)
    {
        var result = await _mediator.Send(new GetFeatureQuery(featureId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFeatureAsync(CreateFeatureRequest createFeatureRequest)
    {
        var result = await _mediator.Send(new CreateFeatureCommand(createFeatureRequest));
        return Ok(result);
    }

    [HttpPut("{featureId:guid}")]
    public async Task<IActionResult> UpdateFeatureAsync(Guid featureId, UpdateFeatureRequest updateFeatureRequest)
    {
        var result = await _mediator.Send(new UpdateFeatureCommand(updateFeatureRequest));
        return Ok(result);
    }

    [HttpDelete("{featureId:guid}")]
    public async Task<IActionResult> DeleteFeatureAsync(Guid featureId)
    {
        var result = await _mediator.Send(new DeleteFeatureCommand(featureId));
        return Ok(result);
    }
}