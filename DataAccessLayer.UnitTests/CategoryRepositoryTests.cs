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
using Shared.DTO.Category.Request;
using Shared.Interface;

namespace DataAccessLayer.UnitTests;

public class CategoryRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public CategoryRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering CategoryRepository and IMapper
        services.AddScoped<CategoryRepository>();
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

    #region CreateCategory

    [Fact]
    public async Task CreateCategory_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategoryRequest = new Faker<CreateCategoryRequest>()
                .CustomInstantiator(f => new CreateCategoryRequest
                (
                    Name: f.Commerce.Categories(1)[0],
                    Description: f.Lorem.Sentence(),
                    OrderNumber: f.Random.Int(1, 100),
                    Icon: new Mock<IFormFile>().Object,
                    ImageHorizontal: new Mock<IFormFile>().Object,
                    ImageSquare: new Mock<IFormFile>().Object
                ));

            var request = fakeCategoryRequest.Generate();

            var result = await repo.CreateCategory(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.CategoryId);

            var category = dbContext.Categories.FirstOrDefault(p => p.CategoryId == result.CategoryId);
            Assert.Equal(request.Name.Trim(), category.Name);
            Assert.Equal(request.Description.Trim(), category.Description);
            Assert.Equal(request.OrderNumber, category.OrderNumber);
            Assert.Equal("http://test.com/image.jpg", category.Icon);
            Assert.Equal("http://test.com/image.jpg", category.ImageHorizontalUrl);
            Assert.Equal("http://test.com/image.jpg", category.ImageSquareUrl);
        }
    }

    [Fact]
    public async Task CreateCategory_ThrowsConflictException_WhenNameExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory1 = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = "Test",
                    Description = f.Lorem.Sentence(),
                    OrderNumber = f.Random.Int(1, 1000),
                    Icon = f.Image.PicsumUrl(),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var fakeCategory2 = new Faker<CreateCategoryRequest>()
                .CustomInstantiator(f => new CreateCategoryRequest(
                    Name: "Test",
                    Description: f.Lorem.Sentence(),
                    OrderNumber: f.Random.Int(1, 1000),
                    Icon: new Mock<IFormFile>().Object,
                    ImageHorizontal: new Mock<IFormFile>().Object,
                    ImageSquare: new Mock<IFormFile>().Object
                ));

            var category1 = fakeCategory1.Generate();
            var category2 = fakeCategory2.Generate();

            dbContext.RemoveRange(dbContext.Categories);
            dbContext.Categories.Add(category1);
            await dbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateCategory(category2));
        }
    }

    [Fact]
    public async Task CreateCategory_ThrowsConflictException_WhenOrderNumberExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory1 = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    OrderNumber = 1,
                    Icon = f.Image.PicsumUrl(),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var fakeCategory2 = new Faker<CreateCategoryRequest>()
                .CustomInstantiator(f => new CreateCategoryRequest(
                    Name: f.Commerce.Categories(1)[0],
                    Description: f.Lorem.Sentence(),
                    OrderNumber: 1,
                    Icon: new Mock<IFormFile>().Object,
                    ImageHorizontal: new Mock<IFormFile>().Object,
                    ImageSquare: new Mock<IFormFile>().Object
                ));

            var category1 = fakeCategory1.Generate();
            var category2 = fakeCategory2.Generate();

            dbContext.RemoveRange(dbContext.Categories);
            dbContext.Categories.Add(category1);
            await dbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateCategory(category2));
        }
    }

    #endregion

    #region UpdateCategory

    [Fact]
    public async Task UpdateCategory_ReturnsUpdatedCategory()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    OrderNumber = f.Random.Int(1, 100),
                    Icon = "http://test.com/image.jpg",
                    ImageHorizontalUrl = "http://test.com/image.jpg",
                    ImageSquareUrl = "http://test.com/image.jpg",
                    IsActive = true
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeUpdateCategoryRequest = new Faker<UpdateCategoryRequest>()
                .CustomInstantiator(f => new UpdateCategoryRequest
                (
                    CategoryId: category.CategoryId,
                    Name: f.Commerce.Categories(1)[0],
                    Description: f.Lorem.Sentence(),
                    OrderNumber: f.Random.Int(1, 100),
                    Icon: new Mock<IFormFile>().Object,
                    ImageHorizontal: new Mock<IFormFile>().Object,
                    ImageSquare: new Mock<IFormFile>().Object
                ));

            var request = fakeUpdateCategoryRequest.Generate();

            var result = await repo.UpdateCategory(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.CategoryId);

            var updatedCategory = dbContext.Categories.FirstOrDefault(p => p.CategoryId == result.CategoryId);
            Assert.NotNull(updatedCategory);
            Assert.Equal(request.CategoryId, updatedCategory.CategoryId);
            Assert.Equal(request.Name.Trim(), updatedCategory.Name);
            Assert.Equal(request.Description.Trim(), updatedCategory.Description);
            Assert.Equal(request.OrderNumber, updatedCategory.OrderNumber);
            Assert.Equal("http://test.com/image.jpg", updatedCategory.Icon); // Mocked Image Service URL
            Assert.Equal("http://test.com/image.jpg", updatedCategory.ImageHorizontalUrl); // Mocked Image Service URL
            Assert.Equal("http://test.com/image.jpg", updatedCategory.ImageSquareUrl); // Mocked Image Service URL
        }
    }

    [Fact]
    public async Task UpdateCategory_ThrowConflictException_WhenOrderNumberExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();


            var fakeCategory1 = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    OrderNumber = 1,
                    Icon = "http://test.com/image.jpg",
                    ImageHorizontalUrl = "http://test.com/image.jpg",
                    ImageSquareUrl = "http://test.com/image.jpg",
                    IsActive = true
                });


            var fakeCategory2 = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    OrderNumber = 2,
                    Icon = "http://test.com/image.jpg",
                    ImageHorizontalUrl = "http://test.com/image.jpg",
                    ImageSquareUrl = "http://test.com/image.jpg",
                    IsActive = true
                });

            var category1 = fakeCategory1.Generate();
            var category2 = fakeCategory2.Generate();

            dbContext.RemoveRange(dbContext.Categories);
            dbContext.Categories.Add(category1);
            dbContext.Categories.Add(category2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateCategoryRequest = new Faker<UpdateCategoryRequest>()
                .CustomInstantiator(f => new UpdateCategoryRequest
                (
                    CategoryId: category2.CategoryId,
                    Name: f.Commerce.Categories(1)[0],
                    Description: f.Lorem.Sentence(),
                    OrderNumber: 1,
                    Icon: new Mock<IFormFile>().Object,
                    ImageHorizontal: new Mock<IFormFile>().Object,
                    ImageSquare: new Mock<IFormFile>().Object
                ));

            var request = fakeUpdateCategoryRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateCategory(request));
        }
    }

    [Fact]
    public async Task UpdateCategory_ThrowConflictException_WhenCategoryExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();


            var fakeCategory1 = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = f.Lorem.Sentence(),
                    OrderNumber = f.Random.Int(1, 1000),
                    Icon = "http://test.com/image.jpg",
                    ImageHorizontalUrl = "http://test.com/image.jpg",
                    ImageSquareUrl = "http://test.com/image.jpg",
                    IsActive = true
                });


            var fakeCategory2 = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = "Test 2",
                    Description = f.Lorem.Sentence(),
                    OrderNumber = f.Random.Int(1, 1000),
                    Icon = "http://test.com/image.jpg",
                    ImageHorizontalUrl = "http://test.com/image.jpg",
                    ImageSquareUrl = "http://test.com/image.jpg",
                    IsActive = true
                });

            var category1 = fakeCategory1.Generate();
            var category2 = fakeCategory2.Generate();

            dbContext.RemoveRange(dbContext.Categories);
            dbContext.Categories.Add(category1);
            dbContext.Categories.Add(category2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateCategoryRequest = new Faker<UpdateCategoryRequest>()
                .CustomInstantiator(f => new UpdateCategoryRequest
                (
                    CategoryId: category2.CategoryId,
                    Name: "Test 1",
                    Description: f.Lorem.Sentence(),
                    OrderNumber: f.Random.Int(1, 1000),
                    Icon: new Mock<IFormFile>().Object,
                    ImageHorizontal: new Mock<IFormFile>().Object,
                    ImageSquare: new Mock<IFormFile>().Object
                ));

            var request = fakeUpdateCategoryRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateCategory(request));
        }
    }

    #endregion

    #region DeleteCategory

    [Fact]
    public async Task DeleteCategory_ReturnsDeletedCategoryId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    OrderNumber = f.Random.Int(1, 100),
                    Icon = "http://test.com/image.jpg",
                    ImageHorizontalUrl = "http://test.com/image.jpg",
                    ImageSquareUrl = "http://test.com/image.jpg",
                    IsActive = true
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteCategory(category.CategoryId);

            Assert.Equal(category.CategoryId, result);
        }
    }

    [Fact]
    public async Task DeleteCategory_ThrowsNotFoundException_WhenCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteCategory(Guid.NewGuid()));
        }
    }

    [Fact]
    public async Task GetCategory_ThrowsNotFoundException_WhenCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetCategory(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetCategory

    [Fact]
    public async Task GetCategory_ReturnsCategory()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    OrderNumber = f.Random.Int(1, 100),
                    Icon = "http://test.com/image.jpg",
                    ImageHorizontalUrl = "http://test.com/image.jpg",
                    ImageSquareUrl = "http://test.com/image.jpg",
                    IsActive = true
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetCategory(category.CategoryId);

            Assert.NotNull(result);
            Assert.Equal(category.CategoryId, result.CategoryId);
            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.Description, result.Description);
            Assert.Equal(category.OrderNumber, result.OrderNumber);
            Assert.Equal(category.Icon, result.Icon);
            Assert.Equal(category.ImageHorizontalUrl, result.ImageHorizontalUrl);
            Assert.Equal(category.ImageSquareUrl, result.ImageSquareUrl);
        }
    }

    [Fact]
    public async Task GetCategory_ThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetCategory(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetCategories

    [Fact]
    public async Task GetCategories_ReturnsCategories()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory1 = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    OrderNumber = f.Random.Int(1, 100),
                    Icon = "http://test.com/image1.jpg",
                    ImageHorizontalUrl = "http://test.com/image1.jpg",
                    ImageSquareUrl = "http://test.com/image1.jpg",
                    IsActive = true
                });

            var fakeCategory2 = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    OrderNumber = f.Random.Int(1, 100),
                    Icon = "http://test.com/image2.jpg",
                    ImageHorizontalUrl = "http://test.com/image2.jpg",
                    ImageSquareUrl = "http://test.com/image2.jpg",
                    IsActive = true
                });

            var category1 = fakeCategory1.Generate();
            var category2 = fakeCategory2.Generate();

            dbContext.Categories.RemoveRange(dbContext.Categories);
            dbContext.Categories.AddRange(category1, category2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetCategories();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.CategoryId == category1.CategoryId);
            Assert.Contains(result, r => r.CategoryId == category2.CategoryId);
        }
    }

    [Fact]
    public async Task GetActiveCategories_ReturnsCategories()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<CategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory1 = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    OrderNumber = f.Random.Int(1, 100),
                    Icon = "http://test.com/image1.jpg",
                    ImageHorizontalUrl = "http://test.com/image1.jpg",
                    ImageSquareUrl = "http://test.com/image1.jpg",
                    IsActive = true
                });

            var fakeCategory2 = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    OrderNumber = f.Random.Int(1, 100),
                    Icon = "http://test.com/image2.jpg",
                    ImageHorizontalUrl = "http://test.com/image2.jpg",
                    ImageSquareUrl = "http://test.com/image2.jpg",
                    IsActive = false
                });

            var category1 = fakeCategory1.Generate();
            var category2 = fakeCategory2.Generate();

            dbContext.Categories.RemoveRange(dbContext.Categories);
            dbContext.Categories.AddRange(category1, category2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetCategories(p => p.IsActive);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, r => r.CategoryId == category1.CategoryId);
            foreach (var item in result)
            {
                Assert.True(item.IsActive);
            }
        }
    }

    #endregion
}