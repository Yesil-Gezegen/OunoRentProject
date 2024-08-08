using Bogus;
using BusinessLayer.Mapper;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.DTO.SubCategory.Request;
using Shared.Interface;
using BusinessLayer.Middlewares;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.UnitTests;

public class SubCategoryRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public SubCategoryRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering SubCategoryRepository and IMapper
        services.AddScoped<SubCategoryRepository>();
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

    #region CreateSubCategory

    [Fact]
    public async Task CreateSubCategory_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl(),
                    OrderNumber = f.Random.Int(1, 1000),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeSubCategoryRequest = new Faker<CreateSubCategoryRequest>()
                .CustomInstantiator(f => new CreateSubCategoryRequest
                (
                    Name: f.Commerce.ProductName(),
                    Description: f.Lorem.Sentence(),
                    Icon: new Mock<IFormFile>().Object,
                    OrderNumber: f.Random.Int(1, 100)
                ));

            var request = fakeSubCategoryRequest.Generate();

            var result = await repo.CreateSubCategory(category.CategoryId, request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.SubCategoryId);

            var subCategory = dbContext.SubCategories.FirstOrDefault(p => p.SubCategoryId == result.SubCategoryId);
            Assert.NotNull(subCategory);
            Assert.Equal(request.Name.Trim(), subCategory.Name);
            Assert.Equal(request.Description.Trim(), subCategory.Description);
            Assert.Equal("http://test.com/image.jpg", subCategory.Icon); // Mocked Image Service URL
            Assert.Equal(request.OrderNumber, subCategory.OrderNumber);
            Assert.True(subCategory.IsActive);
        }
    }

    [Fact]
    public async Task CreateSubCategory_ThrowsConflictException_WhenNameExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl(),
                    OrderNumber = f.Random.Int(1, 1000),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = "Existing Name",
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image.jpg",
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = true
                });

            var subCategory = fakeSubCategory.Generate();

            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeSubCategoryRequest = new Faker<CreateSubCategoryRequest>()
                .CustomInstantiator(f => new CreateSubCategoryRequest
                (
                    Name: "Existing Name", // Same name
                    Description: f.Lorem.Sentence(),
                    Icon: new Mock<IFormFile>().Object,
                    OrderNumber: f.Random.Int(1, 100)
                ));

            var request = fakeSubCategoryRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateSubCategory(category.CategoryId, request));
        }
    }

    [Fact]
    public async Task CreateSubCategory_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl(),
                    OrderNumber = f.Random.Int(1, 1000),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image.jpg",
                    OrderNumber = 1,
                    IsActive = true
                });

            var subCategory = fakeSubCategory.Generate();

            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeSubCategoryRequest = new Faker<CreateSubCategoryRequest>()
                .CustomInstantiator(f => new CreateSubCategoryRequest
                (
                    Name: f.Commerce.Categories(1)[0],
                    Description: f.Lorem.Sentence(),
                    Icon: new Mock<IFormFile>().Object,
                    OrderNumber: 1
                ));

            var request = fakeSubCategoryRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateSubCategory(category.CategoryId, request));
        }
    }

    #endregion

    #region UpdateSubCategory

    [Fact]
    public async Task UpdateSubCategory_ReturnsUpdatedSubCategory()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl(),
                    OrderNumber = f.Random.Int(1, 1000),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = f.Commerce.ProductName(),
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image.jpg",
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = true
                });

            var subCategory = fakeSubCategory.Generate();

            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeUpdateSubCategoryRequest = new Faker<UpdateSubCategoryRequest>()
                .CustomInstantiator(f => new UpdateSubCategoryRequest
                (
                    SubCategoryId: subCategory.SubCategoryId,
                    Name: f.Commerce.ProductName(),
                    Description: f.Lorem.Sentence(),
                    Icon: new Mock<IFormFile>().Object,
                    OrderNumber: f.Random.Int(1, 100)
                ));

            var request = fakeUpdateSubCategoryRequest.Generate();

            var result = await repo.UpdateSubCategory(category.CategoryId, request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.SubCategoryId);

            var updatedSubCategory =
                dbContext.SubCategories.FirstOrDefault(p => p.SubCategoryId == result.SubCategoryId);
            Assert.NotNull(updatedSubCategory);
            Assert.Equal(request.SubCategoryId, updatedSubCategory.SubCategoryId);
            Assert.Equal(request.Name.Trim(), updatedSubCategory.Name);
            Assert.Equal(request.Description.Trim(), updatedSubCategory.Description);
            Assert.Equal("http://test.com/image.jpg", updatedSubCategory.Icon); // Mocked Image Service URL
            Assert.Equal(request.OrderNumber, updatedSubCategory.OrderNumber);
        }
    }

    [Fact]
    public async Task UpdateSubCategory_ThrowsConflictException_WhenNameExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl(),
                    OrderNumber = f.Random.Int(1, 1000),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);

            var fakeSubCategory1 = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = "SubCategory 1",
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image1.jpg",
                    OrderNumber = f.Random.Int(1, 1000),
                    IsActive = true
                });

            var fakeSubCategory2 = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = "SubCategory 2",
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image2.jpg",
                    OrderNumber = f.Random.Int(1, 1000),
                    IsActive = true
                });

            var subCategory1 = fakeSubCategory1.Generate();
            var subCategory2 = fakeSubCategory2.Generate();

            dbContext.SubCategories.AddRange(subCategory1, subCategory2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateSubCategoryRequest = new Faker<UpdateSubCategoryRequest>()
                .CustomInstantiator(f => new UpdateSubCategoryRequest
                (
                    SubCategoryId: subCategory2.SubCategoryId,
                    Name: "SubCategory 1",
                    Description: f.Lorem.Sentence(),
                    Icon: new Mock<IFormFile>().Object,
                    OrderNumber: f.Random.Int(1, 1000)
                ));

            var request = fakeUpdateSubCategoryRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateSubCategory(category.CategoryId, request));
        }
    }

    [Fact]
    public async Task UpdateSubCategory_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl(),
                    OrderNumber = f.Random.Int(1, 1000),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);

            var fakeSubCategory1 = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = f.Commerce.ProductName(),
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image1.jpg",
                    OrderNumber = 1, // Specific order number
                    IsActive = true
                });

            var fakeSubCategory2 = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = f.Commerce.ProductName(),
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image2.jpg",
                    OrderNumber = 2,
                    IsActive = true
                });

            var subCategory1 = fakeSubCategory1.Generate();
            var subCategory2 = fakeSubCategory2.Generate();

            dbContext.SubCategories.AddRange(subCategory1, subCategory2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateSubCategoryRequest = new Faker<UpdateSubCategoryRequest>()
                .CustomInstantiator(f => new UpdateSubCategoryRequest
                (
                    SubCategoryId: subCategory2.SubCategoryId,
                    Name: f.Commerce.ProductName(),
                    Description: f.Lorem.Sentence(),
                    Icon: new Mock<IFormFile>().Object,
                    OrderNumber: 1 // Same order number as subCategory1
                ));

            var request = fakeUpdateSubCategoryRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateSubCategory(category.CategoryId, request));
        }
    }

    #endregion

    #region DeleteSubCategory

    [Fact]
    public async Task DeleteSubCategory_ReturnsDeletedSubCategoryId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl(),
                    OrderNumber = f.Random.Int(1, 1000),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = f.Commerce.ProductName(),
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image.jpg",
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = true
                });

            var subCategory = fakeSubCategory.Generate();

            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteSubCategory(subCategory.SubCategoryId);

            Assert.Equal(subCategory.SubCategoryId, result);
        }
    }

    [Fact]
    public async Task DeleteSubCategory_ThrowsNotFoundException_WhenSubCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteSubCategory(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetSubCategory

    [Fact]
    public async Task GetSubCategory_ReturnsSubCategory()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl(),
                    OrderNumber = f.Random.Int(1, 1000),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = f.Commerce.ProductName(),
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image.jpg",
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = true
                });

            var subCategory = fakeSubCategory.Generate();

            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetSubCategory(category.CategoryId, subCategory.SubCategoryId);

            Assert.NotNull(result);
            Assert.Equal(subCategory.SubCategoryId, result.SubCategoryId);
            Assert.Equal(subCategory.Name, result.Name);
            Assert.Equal(subCategory.Description, result.Description);
            Assert.Equal(subCategory.Icon, result.Icon);
            Assert.Equal(subCategory.OrderNumber, result.OrderNumber);
            Assert.Equal(subCategory.IsActive, result.IsActive);
        }
    }

    [Fact]
    public async Task GetSubCategory_ThrowsNotFoundException_WhenSubCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetSubCategory(Guid.NewGuid(), Guid.NewGuid()));
        }
    }

    #endregion

    #region GetSubCategories

    [Fact]
    public async Task GetSubCategories_ReturnsSubCategories()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl(),
                    OrderNumber = f.Random.Int(1, 1000),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);

            var fakeSubCategory1 = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = f.Commerce.ProductName(),
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image1.jpg",
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = true
                });

            var fakeSubCategory2 = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = f.Commerce.ProductName(),
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image2.jpg",
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = true
                });

            var subCategory1 = fakeSubCategory1.Generate();
            var subCategory2 = fakeSubCategory2.Generate();

            dbContext.SubCategories.AddRange(subCategory1, subCategory2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetSubCategories(category.CategoryId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.SubCategoryId == subCategory1.SubCategoryId);
            Assert.Contains(result, r => r.SubCategoryId == subCategory2.SubCategoryId);
        }
    }

    [Fact]
    public async Task GetActiveSubCategories_ReturnsSubCategories()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SubCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    Description = f.Lorem.Paragraphs(),
                    Icon = f.Image.PicsumUrl(),
                    OrderNumber = f.Random.Int(1, 1000),
                    ImageHorizontalUrl = f.Image.PicsumUrl(),
                    ImageSquareUrl = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);

            var fakeSubCategory1 = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = f.Commerce.ProductName(),
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image1.jpg",
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = true
                });

            var fakeSubCategory2 = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = category.CategoryId,
                    Name = f.Commerce.ProductName(),
                    Description = f.Lorem.Sentence(),
                    Icon = "http://test.com/image2.jpg",
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = false
                });

            var subCategory1 = fakeSubCategory1.Generate();
            var subCategory2 = fakeSubCategory2.Generate();

            dbContext.SubCategories.AddRange(subCategory1, subCategory2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetSubCategories(category.CategoryId, p => p.IsActive);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, r => r.SubCategoryId == subCategory1.SubCategoryId);
            foreach (var item in result)
            {
                Assert.True(item.IsActive);
            }
        }
    }

    #endregion
}