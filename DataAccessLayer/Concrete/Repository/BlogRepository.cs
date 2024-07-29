using System.Linq.Expressions;
using AutoMapper;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Blog.Request;
using Shared.DTO.Blog.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class BlogRepository : IBlogRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public BlogRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateBlog
    public async Task<BlogResponse> CreateBlogAsync(CreateBlogRequest createBlogRequest)
    {
        var isExistSubCategory = await _applicationDbContext.SubCategories
        .AnyAsync(x => x.SubCategoryId == createBlogRequest.SubCategoryId);
        if (!isExistSubCategory)
            throw new NotFoundException(SubCategoryExceptionMessages.NotFound);
        
        await IsExistOrderNumber(createBlogRequest.OrderNumber);

        var sanitizer = new HtmlSanitizer();
        
        var blog = new Blog();

        blog.LargeImageUrl = createBlogRequest.LargeImageUrl.Trim();
        blog.OrderNumber = createBlogRequest.OrderNumber;
        blog.Slug = createBlogRequest.Slug.Trim();
        blog.SmallImageUrl = createBlogRequest.SmallImageUrl.Trim();
        blog.IsActive = true;
        blog.Date = DateTime.UtcNow;
        blog.SubCategoryId = createBlogRequest.SubCategoryId;
        blog.Body = sanitizer.Sanitize(createBlogRequest.Body.Trim());
        blog.Title = sanitizer.Sanitize(createBlogRequest.Title.Trim());
        blog.Tags = sanitizer.Sanitize(createBlogRequest.Tags.Trim());

        var result = await _applicationDbContext.Blogs.AddAsync(blog);

        await _applicationDbContext.SaveChangesAsync();

        var blogResponse = _mapper.Map<BlogResponse>(blog);

        return blogResponse;
    }
    #endregion

    #region GetBlog
    public async Task<GetBlogResponse> GetBlogAsync(Guid blogId)
    {
        var result = await _applicationDbContext.Blogs
        .Include(x => x.SubCategory)
        .AsNoTracking()
        .FirstOrDefaultAsync(b => b.BlogId == blogId)
            ?? throw new NotFoundException(BlogExceptionMessages.NotFound);

        var blogResponse = _mapper.Map<GetBlogResponse>(result);

        return blogResponse;
    }
    #endregion

    #region GetBlogs
    public async Task<List<GetBlogsResponse>> GetBlogsAsync()
    {
        var blogList = await _applicationDbContext.Blogs
        .Include(x => x.SubCategory)
        .AsNoTracking()
        .ToListAsync();

        var blogResponse = _mapper.Map<List<GetBlogsResponse>>(blogList);

        return blogResponse;
    }
    #endregion

    #region DeleteBlog
    public async Task<Guid> DeleteBlog(Guid blogId)
    {
        var blog = await _applicationDbContext.Blogs
            .Where(x => x.BlogId == blogId)
            .FirstOrDefaultAsync()
        ?? throw new NotFoundException(BlogExceptionMessages.NotFound);

        _applicationDbContext.Blogs.Remove(blog);

        await _applicationDbContext.SaveChangesAsync();

        return blog.BlogId;
    }

    #endregion

    #region UpdateBlog
    public async Task<BlogResponse> UpdateBlog(UpdateBlogRequest updateBlogRequest)
    {
        var blog = await _applicationDbContext.Blogs
            .Where(x => x.BlogId == updateBlogRequest.BlogId)
            .FirstOrDefaultAsync()
        ?? throw new NotFoundException(BlogExceptionMessages.NotFound);

        var isExistSubCategory = await _applicationDbContext.SubCategories
         .AnyAsync(x => x.SubCategoryId == updateBlogRequest.SubCategoryId);
        if (!isExistSubCategory)
            throw new NotFoundException(SubCategoryExceptionMessages.NotFound);

        await IsExistOrderNumberWhenUpdate(updateBlogRequest.BlogId, updateBlogRequest.OrderNumber);

        blog.Title = updateBlogRequest.Title.Trim();
        blog.Tags = updateBlogRequest.Tags.Trim();
        blog.Slug = updateBlogRequest.Slug.Trim();
        blog.OrderNumber = updateBlogRequest.OrderNumber;
        blog.Date = updateBlogRequest.Date;
        blog.SubCategoryId = updateBlogRequest.SubCategoryId;
        blog.LargeImageUrl = updateBlogRequest.LargeImgUrl.Trim();
        blog.SmallImageUrl = updateBlogRequest.SmallImgUrl.Trim();
        blog.IsActive = updateBlogRequest.IsActive;

        await _applicationDbContext.SaveChangesAsync();

        var blogResponse = _mapper.Map<BlogResponse>(blog);

        return blogResponse;
    }

    #endregion
    
    #region IsExist
    private async Task IsExistOrderNumberWhenUpdate(Guid blogId, int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.Blogs
            .AnyAsync(x => x.BlogId != blogId && x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
        {
            throw new ConflictException(BlogExceptionMessages.OrderNumberConflict);
        }
    }
    
    private async Task IsExistOrderNumber(int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.Blogs
            .AnyAsync(x => x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
        {
            throw new ConflictException(BlogExceptionMessages.OrderNumberConflict);
        }
    }

    private async Task<bool> IsExistGeneric(Expression<Func<Blog, bool>> filter)
    {
        var result = await _applicationDbContext.Blogs.AnyAsync(filter);

        if (result)
            throw new ConflictException(BlogExceptionMessages.Conflict);

        return result;
    }

    #endregion
}
