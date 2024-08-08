using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.DTO.Blog.Request;
using Shared.Interface;

namespace DataAccessLayer.UnitTests;

public class BlogRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public BlogRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering BlogRepository and IMapper
        services.AddScoped<BlogRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        // Mocking IImageService
        var mockImageService = new Mock<IImageService>();
        mockImageService.Setup(s => s.SaveImageAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("http://test.com/image.jpg");
        mockImageService.Setup(s => s.DeleteImageAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        services.AddSingleton(mockImageService.Object);

        _serviceProvider = services.BuildServiceProvider();
    }

    #region CreateBlog

    [Fact]
    public async Task CreateBlog_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl()
                });

            var subCategory = fakeSubCategory.Generate();

            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeBlogRequest = new Faker<CreateBlogRequest>()
                .CustomInstantiator(f => new CreateBlogRequest
                (
                    SubCategoryId: subCategory.SubCategoryId,
                    OrderNumber: f.Random.Int(1, 100),
                    Slug: f.Lorem.Slug(),
                    Body: f.Lorem.Paragraphs(),
                    Title: f.Lorem.Sentence(),
                    Tags: f.Lorem.Words(5).ToString(),
                    LargeImage: new Mock<IFormFile>().Object,
                    SmallImage: new Mock<IFormFile>().Object
                ));

            var request = fakeBlogRequest.Generate();

            var result = await repo.CreateBlogAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.BlogId);

            var blog = dbContext.Blogs.FirstOrDefault(p => p.BlogId == result.BlogId);
            Assert.Equal(request.Slug, blog.Slug);
            Assert.Equal(request.Title, blog.Title);
            Assert.Equal(request.Body, blog.Body);
            Assert.Equal(request.Tags, blog.Tags);
            Assert.Equal(request.OrderNumber, blog.OrderNumber);
            Assert.Equal(request.SubCategoryId, blog.SubCategory.SubCategoryId);
        }
    }

    [Fact]
    public async Task CreateBlog_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl()
                });

            var subCategory = fakeSubCategory.Generate();

            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeBlog = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = subCategory.SubCategoryId,
                    OrderNumber = 1, // Specific order number
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg"
                });

            var blog = fakeBlog.Generate();

            dbContext.Blogs.Add(blog);
            await dbContext.SaveChangesAsync();

            var fakeBlogRequest = new Faker<CreateBlogRequest>()
                .CustomInstantiator(f => new CreateBlogRequest
                (
                    SubCategoryId: subCategory.SubCategoryId,
                    OrderNumber: 1, // Same order number
                    Slug: f.Lorem.Slug(),
                    Body: f.Lorem.Paragraphs(),
                    Title: f.Lorem.Sentence(),
                    Tags: f.Lorem.Words(5).ToString(),
                    LargeImage: new Mock<IFormFile>().Object,
                    SmallImage: new Mock<IFormFile>().Object
                ));

            var request = fakeBlogRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateBlogAsync(request));
        }
    }

    [Fact]
    public async Task CreateBlog_ThrowsNotFoundException_WhenSubCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();

            var fakeBlogRequest = new Faker<CreateBlogRequest>()
                .CustomInstantiator(f => new CreateBlogRequest
                (
                    SubCategoryId: Guid.NewGuid(), // Non-existent sub-category
                    OrderNumber: f.Random.Int(1, 100),
                    Slug: f.Lorem.Slug(),
                    Body: f.Lorem.Paragraphs(),
                    Title: f.Lorem.Sentence(),
                    Tags: f.Lorem.Words(5).ToString(),
                    LargeImage: new Mock<IFormFile>().Object,
                    SmallImage: new Mock<IFormFile>().Object
                ));

            var request = fakeBlogRequest.Generate();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.CreateBlogAsync(request));
        }
    }

    #endregion

    #region UpdateBlog

    [Fact]
    public async Task UpdateBlog_ReturnsUpdatedBlog()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl()
                });

            var subCategory = fakeSubCategory.Generate();

            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeBlog = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = subCategory.SubCategoryId,
                    OrderNumber = f.Random.Int(1, 100),
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg"
                });

            var blog = fakeBlog.Generate();

            dbContext.Blogs.Add(blog);
            await dbContext.SaveChangesAsync();

            var fakeUpdateBlogRequest = new Faker<UpdateBlogRequest>()
                .CustomInstantiator(f => new UpdateBlogRequest
                (
                    BlogId: blog.BlogId,
                    SubCategoryId: subCategory.SubCategoryId,
                    OrderNumber: f.Random.Int(1, 100),
                    Slug: f.Lorem.Slug(),
                    Body: f.Lorem.Paragraphs(),
                    Title: f.Lorem.Sentence(),
                    Tags: f.Lorem.Words(5).ToString(),
                    Date: DateTime.UtcNow,
                    IsActive: f.Random.Bool(),
                    LargeImage: new Mock<IFormFile>().Object,
                    SmallImage: new Mock<IFormFile>().Object
                ));

            var request = fakeUpdateBlogRequest.Generate();

            var result = await repo.UpdateBlog(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.BlogId);

            var updatedBlog = dbContext.Blogs.FirstOrDefault(p => p.BlogId == result.BlogId);
            Assert.Equal(request.BlogId, updatedBlog.BlogId);
            Assert.Equal(request.Slug, updatedBlog.Slug);
            Assert.Equal(request.Title, updatedBlog.Title);
            Assert.Equal(request.Body, updatedBlog.Body);
            Assert.Equal(request.Tags, updatedBlog.Tags);
            Assert.Equal(request.OrderNumber, updatedBlog.OrderNumber);
            Assert.Equal(request.SubCategoryId, updatedBlog.SubCategoryId);
            Assert.Equal(request.IsActive, updatedBlog.IsActive);
        }
    }

    [Fact]
    public async Task UpdateBlog_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl()
                });

            var subCategory = fakeSubCategory.Generate();

            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeBlog1 = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = subCategory.SubCategoryId,
                    OrderNumber = 1, // Specific order number
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg"
                });

            var fakeBlog2 = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = subCategory.SubCategoryId,
                    OrderNumber = f.Random.Int(1, 100),
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg"
                });

            var blog1 = fakeBlog1.Generate();
            var blog2 = fakeBlog2.Generate();

            dbContext.Blogs.AddRange(blog1, blog2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateBlogRequest = new Faker<UpdateBlogRequest>()
                .CustomInstantiator(f => new UpdateBlogRequest
                (
                    BlogId: blog2.BlogId,
                    SubCategoryId: subCategory.SubCategoryId,
                    OrderNumber: 1, // Same order number as blog1
                    Slug: f.Lorem.Slug(),
                    Body: f.Lorem.Paragraphs(),
                    Title: f.Lorem.Sentence(),
                    Tags: f.Lorem.Words(5).ToString(),
                    Date: DateTime.UtcNow,
                    IsActive: f.Random.Bool(),
                    LargeImage: new Mock<IFormFile>().Object,
                    SmallImage: new Mock<IFormFile>().Object
                ));

            var request = fakeUpdateBlogRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateBlog(request));
        }
    }

    [Fact]
    public async Task UpdateBlog_ThrowsNotFoundException_WhenSubCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeBlog = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = Guid.NewGuid(), // Non-existent sub-category
                    OrderNumber = f.Random.Int(1, 100),
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg"
                });

            var blogEntity = fakeBlog.Generate();

            dbContext.Blogs.Add(blogEntity);
            await dbContext.SaveChangesAsync();

            var fakeUpdateBlogRequest = new Faker<UpdateBlogRequest>()
                .CustomInstantiator(f => new UpdateBlogRequest
                (
                    BlogId: blogEntity.BlogId,
                    SubCategoryId: Guid.NewGuid(), // Non-existent sub-category
                    OrderNumber: f.Random.Int(1, 100),
                    Slug: f.Lorem.Slug(),
                    Body: f.Lorem.Paragraphs(),
                    Title: f.Lorem.Sentence(),
                    Tags: f.Lorem.Words(5).ToString(),
                    Date: DateTime.UtcNow,
                    IsActive: f.Random.Bool(),
                    LargeImage: new Mock<IFormFile>().Object,
                    SmallImage: new Mock<IFormFile>().Object
                ));

            var request = fakeUpdateBlogRequest.Generate();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.UpdateBlog(request));
        }
    }

    #endregion

    #region DeleteBlog

    [Fact]
    public async Task DeleteBlog_ReturnsDeletedBlogId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeBlog = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg"
                });

            var blog = fakeBlog.Generate();

            dbContext.Blogs.Add(blog);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteBlog(blog.BlogId);

            Assert.Equal(blog.BlogId, result);
        }
    }

    [Fact]
    public async Task DeleteBlog_ThrowsNotFoundException_WhenBlogDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteBlog(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetBlog

    [Fact]
    public async Task GetBlog_ReturnsBlog()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeBlog = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg"
                });

            var blog = fakeBlog.Generate();

            dbContext.Blogs.Add(blog);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetBlogAsync(blog.BlogId);

            Assert.NotNull(result);
            Assert.Equal(blog.BlogId, result.BlogId);
            Assert.Equal(blog.Slug, result.Slug);
            Assert.Equal(blog.Title, result.Title);
            Assert.Equal(blog.Body, result.Body);
            Assert.Equal(blog.Tags, result.Tags);
            Assert.Equal(blog.OrderNumber, result.OrderNumber);
            Assert.Equal(blog.SubCategoryId, result.SubCategoryId);
        }
    }

    [Fact]
    public async Task GetBlog_ThrowsNotFoundException_WhenBlogDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetBlogAsync(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetBlogs

    [Fact]
    public async Task GetBlogs_ReturnsBlogs()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeBlog1 = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg"
                });

            var fakeBlog2 = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg"
                });

            var blog1 = fakeBlog1.Generate();
            var blog2 = fakeBlog2.Generate();

            dbContext.RemoveRange(dbContext.Blogs);
            dbContext.Blogs.AddRange(blog1, blog2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetBlogsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.BlogId == blog1.BlogId);
            Assert.Contains(result, r => r.BlogId == blog2.BlogId);
        }
    }

    [Fact]
    public async Task GetActiveBlogs_ReturnsBlogs()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BlogRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeBlog1 = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg",
                    IsActive = true
                });

            var fakeBlog2 = new Faker<Blog>()
                .CustomInstantiator(f => new Blog
                {
                    BlogId = Guid.NewGuid(),
                    SubCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    Slug = f.Lorem.Slug(),
                    Body = f.Lorem.Paragraphs(),
                    Title = f.Lorem.Sentence(),
                    Tags = f.Lorem.Words(5).ToString(),
                    LargeImageUrl = "http://test.com/image.jpg",
                    SmallImageUrl = "http://test.com/image.jpg",
                    IsActive = false
                });

            var blog1 = fakeBlog1.Generate();
            var blog2 = fakeBlog2.Generate();

            dbContext.Blogs.RemoveRange(dbContext.Blogs);
            dbContext.Blogs.AddRange(blog1, blog2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetBlogsAsync(p => p.IsActive);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, r => r.BlogId == blog1.BlogId);
            foreach (var item in result)
            {
                Assert.True(item.IsActive);
            }
        }
    }

    #endregion
}