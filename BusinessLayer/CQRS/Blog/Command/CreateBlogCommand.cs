using MediatR;
using Shared.DTO.Blog.Request;
using Shared.DTO.Blog.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Blog.Command;

public sealed record CreateBlogCommand(CreateBlogRequest CreateBlogRequest) : IRequest<BlogResponse>;

public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, BlogResponse>
{
    private readonly IBlogRepository _blogRepository;

    public CreateBlogCommandHandler(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<BlogResponse> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        var result = await _blogRepository.CreateBlogAsync(request.CreateBlogRequest);
        return result;
    }
}
