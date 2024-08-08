using BusinessLayer.CQRS.Brand.Command;
using BusinessLayer.CQRS.Brand.Query;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Brand.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateBrand([FromForm] CreateBrandRequest createBrandRequest)
    {
        var result = await _mediator.Send(new CreateBrandCommand(createBrandRequest));

        return Ok(result);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetBrands()
    {
        var result = await _mediator.Send(new GetBrandsQuery());

        return Ok(result);
    }
    
    [HttpGet("getActive")]
    public async Task<IActionResult> GetActiveBrands()
    {
        var result = await _mediator.Send(new GetActiveBrandsQuery());

        return Ok(result);
    }

    [HttpGet("{brandId:guid}")]
    public async Task<IActionResult> GetBrand(Guid brandId)
    {
        var result = await _mediator.Send(new GetBrandQuery(brandId));

        return Ok(result);
    }

    [HttpPut("{brandId:guid}")]
    public async Task<IActionResult> UpdateBrand([FromForm] UpdateBrandRequest updateBrandRequest)
    {
        var result = await _mediator.Send(new UpdateBrandCommand(updateBrandRequest));

        return Ok(result);
    }

    [HttpDelete("{brandId:guid}")]
    public async Task<IActionResult> DeleteBrand(Guid brandId)
    {
        var result = await _mediator.Send(new DeleteBrandCommand(brandId));

        return Ok(result);
    }
}