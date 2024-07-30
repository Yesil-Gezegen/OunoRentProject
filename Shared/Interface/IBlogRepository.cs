using System.Linq.Expressions;
using Shared.DTO.Blog.Request;
using Shared.DTO.Blog.Response;

namespace Shared.Interface;

public interface IBlogRepository
{
    Task<BlogResponse> CreateBlogAsync(CreateBlogRequest createBlogRequest);
    Task<GetBlogResponse> GetBlogAsync(Guid id);
    Task<List<GetBlogsResponse>> GetBlogsAsync(Expression<Func<GetBlogResponse, bool>>? predicate = null);
    Task<Guid> DeleteBlog(Guid blogId);
    Task<BlogResponse> UpdateBlog(UpdateBlogRequest updateBlogRequest);
}
