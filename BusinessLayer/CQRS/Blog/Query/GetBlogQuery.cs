using MediatR;
using Shared.DTO.Blog.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Blog.Query;

public sealed record GetBlogQuery(Guid Id) : IRequest<GetBlogResponse>;

public class GetBlogQueryHandler : IRequestHandler<GetBlogQuery, GetBlogResponse>
{
    private readonly IBlogRepository _blogRepository;

    public GetBlogQueryHandler(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<GetBlogResponse> Handle(GetBlogQuery request, CancellationToken cancellationToken)
    {
        var result = await _blogRepository.GetBlogAsync(request.Id);
        return result;
    }
}
