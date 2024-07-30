using BusinessLayer.CQRS.Blog.Command;
using BusinessLayer.CQRS.Blog.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Blog.Request;

namespace OunoRentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost()]
    public async Task<IActionResult> CreateBlogAsync(CreateBlogRequest createBlogRequest)
    {
        var result = await _mediator.Send(new CreateBlogCommand(createBlogRequest));
        return Ok(result);
    }

    [HttpGet("{blogId:guid}")]
    public async Task<IActionResult> GetBlog(Guid blogId)
    {
        var result = await _mediator.Send(new GetBlogQuery(blogId));
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetBlogs()
    {
        var result = await _mediator.Send(new GetBlogsQuery());
        return Ok(result);
    }

    [HttpGet("GetActive")]
    public async Task<IActionResult> GetActiveBlogs()
    {
        var result = await _mediator.Send(new GetActiveBlogsQuery());
        return Ok(result);
    }

    [HttpDelete("{blogId:guid}")]
    public async Task<IActionResult> Delete(Guid blogId)
    {
        var result = await _mediator.Send(new DeleteBlogCommand(blogId));

        return Ok(result);
    }

    [HttpPut("{blogId:guid}")]
    public async Task<IActionResult> Update(UpdateBlogRequest updateBlogRequest)
    {
        var result = await _mediator.Send(new UpdateBlogCommand(updateBlogRequest));

        return Ok(result);
    }
}
