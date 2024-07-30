using BusinessLayer.CQRS.FeaturedCategory.Command;
using BusinessLayer.CQRS.FeaturedCategory.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.FeaturedCategories.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeaturedCategoryController : Controller
{
    private readonly IMediator _mediator;

    public FeaturedCategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetFeaturedCategoriesAsync()
    {
        var result = await _mediator.Send(new GetFeaturedCategoriesQuery());
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetActiveFeaturedCategoriesAsync()
    {
        var result = await _mediator.Send(new GetActiveFeaturedCategoriesQuery());
        return Ok(result);
    }
    
    [HttpGet("{featuredCategoryId:guid}")]

    public async Task<IActionResult> GetFeaturedCategoryAsync(Guid featuredCategoryId)
    {
        var result = await _mediator.Send(new GetFeaturedCategoryQuery(featuredCategoryId));
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateFeaturedCategoryAsync(
        CreateFeaturedCategoryRequest createFeaturedCategoryRequest)
    {
        var result = await _mediator.Send(new CreateFeaturedCategoryCommand(createFeaturedCategoryRequest));
        return Ok(result);
    }

    [HttpPut("{featuredCategoryId:guid}")]
    public async Task<IActionResult> UpdateFeaturedCategoryAsync(
        UpdateFeaturedCategoryRequest updateFeaturedCategoryRequest)
    {
        var result = await _mediator.Send(new UpdateFeaturedCategoryCommand(updateFeaturedCategoryRequest));
        return Ok(result);
    }

    [HttpDelete("{featuredCategoryId:guid}")]
    public async Task<IActionResult> DeleteFeaturedCategoryAsync(Guid featuredCategoryId)
    {
        var result = await _mediator.Send(new DeleteFeaturedCategoryCommand(featuredCategoryId));
        return Ok(result);
    }
}