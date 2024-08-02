using BusinessLayer.CQRS.Category.Command;
using BusinessLayer.CQRS.Category.Query;
using BusinessLayer.CQRS.SubCategory.Command;
using BusinessLayer.CQRS.SubCategory.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Category.Request;
using Shared.DTO.Category.Response;
using Shared.DTO.SubCategory.Request;

namespace OunoRentApi.Controllers.CategoryController;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _mediator.Send(new GetCategoriesQuery());
        return Ok(categories);
    }

    [HttpGet("GetActive")]
    public async Task<IActionResult> GetActiveCategories()
    {
        var categories = await _mediator.Send(new GetActiveCategoriesQuery());
        return Ok(categories);
    }

    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetCategory(Guid categoryId)
    {
        var category = await _mediator.Send(new GetCategoryQuery(categoryId));
        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryRequest request)
    {
        var category = await _mediator.Send(new CreateCategoryCommand(request));
        return Ok(category);
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategoryRequest request)
    {
        var category = await _mediator.Send(new UpdateCategoryCommand(request));
        return Ok(category);
    }

    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        var category = await _mediator.Send(new DeleteCategoryCommand(categoryId));
        return Ok(category);
    }

    [HttpPost("{categoryId:guid}/subcategory")]
    public async Task<IActionResult> CreateSubCategory(Guid categoryId,
        [FromForm] CreateSubCategoryRequest createSubCategoryRequest)
    {
        var category = await _mediator.Send(new CreateSubCategoryCommand(
            CategoryId: categoryId, CreateSubCategoryRequest: createSubCategoryRequest));

        return Ok(category);
    }

    [HttpGet("{categoryId:guid}/subcategory/{subCategoryId:guid}")]
    public async Task<IActionResult> GetSubCategory(Guid categoryId, Guid subCategoryId)
    {
        var category = await _mediator.Send(new GetSubCategoryQuery(
            CategoryId: categoryId, SubCategoryId: subCategoryId));

        return Ok(category);
    }

    [HttpGet("{categoryId:guid}/subcategory")]
    public async Task<IActionResult> GetSubCategories(Guid categoryId)
    {
        var category = await _mediator.Send(new GetSubCategoriesQuery(categoryId));

        return Ok(category);
    }

    [HttpGet("{categoryId:guid}/subcategory/getActive")]
    public async Task<IActionResult> GetActiveSubCategories(Guid categoryId)
    {
        var category = await _mediator.Send(new GetActiveSubCategoriesQuery(categoryId));

        return Ok(category);
    }

    [HttpPut("{categoryId:guid}/subcategory/{subCategoryId:guid}")]
    public async Task<IActionResult> UpdateSubCategory(Guid categoryId,
        [FromForm] UpdateSubCategoryRequest updateSubCategoryRequest)
    {
        var category = await _mediator.Send(new UpdateSubCategoryCommand(
            CategoryId: categoryId, UpdateSubCategoryRequest: updateSubCategoryRequest));

        return Ok(category);
    }

    [HttpDelete("subcategory/{subCategoryId:guid}")]
    public async Task<IActionResult> DeleteSubCategory(Guid subCategoryId)
    {
        var category = await _mediator.Send(new DeleteSubCategoryCommand(subCategoryId));

        return Ok(category);
    }
}