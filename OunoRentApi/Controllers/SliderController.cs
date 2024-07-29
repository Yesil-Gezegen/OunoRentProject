using BusinessLayer.CQRS.Slider.Command;
using BusinessLayer.CQRS.Slider.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Slider.Request;

namespace OunoRentApi.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SliderController : ControllerBase
{
    private readonly IMediator _mediator;

    public SliderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSlider([FromBody] CreateSliderRequest request)
    {
        var slider = await _mediator.Send(new CreateSliderCommand(request));

        return Ok(slider);
    }

    [HttpGet]
    public async Task<IActionResult> GetSliders()
    {
        var sliders = await _mediator.Send(new GetSlidersQuery());

        return Ok(sliders);
    }

    [HttpGet("{sliderId:guid}")]
    public async Task<IActionResult> GetSlider(Guid sliderId)
    {
        var slider = await _mediator.Send(new GetSliderQuery(sliderId));

        return Ok(slider);
    }

    [HttpPut("{sliderId:guid}")]
    public async Task<IActionResult> UpdateSlider([FromBody] UpdateSliderRequest request)
    {
        var slider = await _mediator.Send(new UpdateSliderCommand(request));

        return Ok(slider);
    }

    [HttpDelete("{sliderId:guid}")]
    public async Task<IActionResult> DeleteSlider(Guid sliderId)
    {
        var slider = await _mediator.Send(new DeleteSliderCommand(sliderId));

        return Ok(slider);
    }
}
