using MediatR;
using Shared.DTO.Blog.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Blog.Query;

public sealed record GetBlogsCommand() : IRequest<List<GetBlogsResponse>>;

public class GetBlogsCommandHandler : IRequestHandler<GetBlogsCommand, List<GetBlogsResponse>>
{
    private readonly IBlogRepository _blogRepository;

    public GetBlogsCommandHandler(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<List<GetBlogsResponse>> Handle(GetBlogsCommand request, CancellationToken cancellationToken)
    {
        var result = await _blogRepository.GetBlogsAsync();
        return result;
    }
}
