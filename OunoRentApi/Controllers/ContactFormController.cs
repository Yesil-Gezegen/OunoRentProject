using BusinessLayer.CQRS.ContactForm.Command;
using BusinessLayer.CQRS.ContactForm.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.ContactForm.Request;

namespace OunoRentApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ContactFormController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactFormController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> CreateContactForm(CreateContactFormRequest createContactFormRequest)
    {
        var result = await _mediator.Send(new CreateContactFormCommand(createContactFormRequest));
        return Ok(result);
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetContactForms()
    {
        var result = await _mediator.Send(new GetContactFormsQuery());
        return Ok(result);
    }
    
    [HttpGet("{contactFormId:guid}")]
    public async Task<IActionResult> GetContactForm(Guid contactFormId)
    {
        var result = await _mediator.Send(new GetContactFormQuery(contactFormId));
        return Ok(result);
    }
    
    [HttpDelete("{contactFormId:guid}")]
    public async Task<IActionResult> DeleteContactForm(Guid contactFormId)
    {
        var result = await _mediator.Send(new DeleteContactFormCommand(contactFormId));
        return Ok(result);
    }
}