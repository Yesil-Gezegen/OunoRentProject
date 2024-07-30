using MediatR;
using Shared.DTO.Blog.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Blog.Query;

public sealed record GetActiveBlogsQuery : IRequest<List<GetBlogsResponse>>;

class GetActiveBlogsQueryHandler : IRequestHandler<GetActiveBlogsQuery, List<GetBlogsResponse>>
{
    private readonly IBlogRepository _blogRepository;

    public GetActiveBlogsQueryHandler(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }
    
    public Task<List<GetBlogsResponse>> Handle(GetActiveBlogsQuery request, CancellationToken cancellationToken)
    {
        var blogResponse = _blogRepository.GetBlogsAsync(b => b.IsActive);
        return blogResponse;
    }
}