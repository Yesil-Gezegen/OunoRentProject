using BusinessLayer.CQRS.Contract.Command;
using BusinessLayer.CQRS.Contract.Query;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Contract.Request;

namespace OunoRentApi.Controllers;
[ApiController]
[Route("api/[controller]")]

public class ContractController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContractController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> CreateContract(CreateContractRequest createContractRequest)
    {
        var result = await _mediator.Send(new CreateContractCommand(createContractRequest));
        
        return Ok(result);
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetContracts()
    {
        var result = await _mediator.Send(new GetContractsQuery());
        
        return Ok(result);
    }
    
    [HttpGet("{contractId:guid}")]
    public async Task<IActionResult> GetContract(Guid contractId)
    {
        var result = await _mediator.Send(new GetContractQuery(contractId));
        
        return Ok(result);
    }
    
    [HttpPut("{contractId:guid}")]
    public async Task<IActionResult> UpdateContract(UpdateContractRequest updateContractRequest)
    {
        var result = await _mediator.Send(new UpdateContractCommand(updateContractRequest));
        
        return Ok(result);
    }
    
    [HttpDelete("{contractId:guid}")]
    public async Task<IActionResult> DeleteContract(Guid contractId)
    {
        var result = await _mediator.Send(new DeleteContractCommand(contractId));
        
        return Ok(result);
    }
}