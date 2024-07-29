using MediatR;
using Shared.DTO.Blog.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Blog.Query;

public sealed record GetBlogCommand(Guid Id) : IRequest<GetBlogResponse>;

public class GetBlogCommandHandle : IRequestHandler<GetBlogCommand, GetBlogResponse>
{
    private readonly IBlogRepository _blogRepository;

    public GetBlogCommandHandle(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<GetBlogResponse> Handle(GetBlogCommand request, CancellationToken cancellationToken)
    {
        var result = await _blogRepository.GetBlogAsync(request.Id);
        return result;
    }
}
