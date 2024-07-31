using BusinessLayer.CQRS.FAQ.Command;
using BusinessLayer.CQRS.FAQ.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.FAQ.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FAQController : Controller
{
    private readonly IMediator _mediator;

    public FAQController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFAQsAsync()
    {
        var result = await _mediator.Send(new GetFAQsQuery());
        return Ok(result);
    }

    [HttpGet("GetActive")]
    public async Task<IActionResult> GetActiveFAQsAsync()
    {
        var result = await _mediator.Send(new GetActiveFAQsQuery());
        return Ok(result);
    }

    [HttpGet("{faqId:guid}")]
    public async Task<IActionResult> GetFAQAsync(Guid faqId)
    {
        var result = await _mediator.Send(new GetFAQQuery(faqId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFAQAsync(CreateFAQRequest createFaqRequest)
    {
        var result = await _mediator.Send(new CreateFAQCommand(createFaqRequest));
        return Ok(result);
    }

    [HttpPut("{faqId:guid}")]
    public async Task<IActionResult> UpdateFAQAsync(Guid faqId, UpdateFAQRequest updateFaqRequest)
    {
        var result = await _mediator.Send(new UpdateFAQCommand(updateFaqRequest));
        return Ok(result);
    }

    [HttpDelete("{faqId:guid}")]
    public async Task<IActionResult> DeleteFAQAsync(Guid faqId)
    {
        var result = await _mediator.Send(new DeleteFAQCommand(faqId));
        return Ok(result);
    }
}