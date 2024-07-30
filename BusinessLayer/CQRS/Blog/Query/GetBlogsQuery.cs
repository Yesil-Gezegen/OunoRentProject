using MediatR;
using Shared.DTO.Blog.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Blog.Query;

public sealed record GetBlogsQuery() : IRequest<List<GetBlogsResponse>>;

public class GetBlogsQueryHandler : IRequestHandler<GetBlogsQuery, List<GetBlogsResponse>>
{
    private readonly IBlogRepository _blogRepository;

    public GetBlogsQueryHandler(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<List<GetBlogsResponse>> Handle(GetBlogsQuery request, CancellationToken cancellationToken)
    {
        var result = await _blogRepository.GetBlogsAsync();
        return result;
    }
}
