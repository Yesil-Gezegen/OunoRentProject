using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.FeaturedCategories.Request;

namespace DataAccessLayer.UnitTests;

public class FeaturedCategoryRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public FeaturedCategoryRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering FeaturedCategoryRepository and IMapper
        services.AddScoped<FeaturedCategoryRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        _serviceProvider = services.BuildServiceProvider();
    }

    #region CreateFeaturedCategory

    [Fact]
    public async Task CreateFeaturedCategory_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    ImageHorizontalUrl = f.Lorem.Slug(),
                    ImageSquareUrl = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategoryRequest = new Faker<CreateFeaturedCategoryRequest>()
                .CustomInstantiator(f => new CreateFeaturedCategoryRequest
                (
                    OrderNumber: f.Random.Int(1, 100),
                    IsActive: f.Random.Bool(),
                    CategoryId: category.CategoryId
                ));

            var request = fakeFeaturedCategoryRequest.Generate();

            var result = await repo.CreateFeaturedCategoryAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.FeaturedCategoryId);

            var featredCategory =
                dbContext.FeaturedCategories.FirstOrDefault(p => p.FeaturedCategoryId == result.FeaturedCategoryId);
            Assert.NotNull(featredCategory);
            Assert.Equal(request.OrderNumber, featredCategory.OrderNumber);
            Assert.Equal(request.IsActive, featredCategory.IsActive);
            Assert.Equal(request.CategoryId, featredCategory.CategoryId);
        }
    }

    [Fact]
    public async Task CreateFeaturedCategory_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    ImageHorizontalUrl = f.Lorem.Slug(),
                    ImageSquareUrl = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategory1 = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = 1,
                    IsActive = f.Random.Bool(),
                    CategoryId = category.CategoryId
                });

            var featuredCategory1 = fakeFeaturedCategory1.Generate();

            dbContext.FeaturedCategories.Add(featuredCategory1);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategoryRequest = new Faker<CreateFeaturedCategoryRequest>()
                .CustomInstantiator(f => new CreateFeaturedCategoryRequest
                (
                    OrderNumber: 1, // Same order number
                    IsActive: f.Random.Bool(),
                    CategoryId: category.CategoryId
                ));

            var request = fakeFeaturedCategoryRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateFeaturedCategoryAsync(request));
        }
    }

    [Fact]
    public async Task CreateFeaturedCategory_ThrowsConflictException_WhenCategoryExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    ImageHorizontalUrl = f.Lorem.Slug(),
                    ImageSquareUrl = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategory1 = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = f.Random.Bool(),
                    CategoryId = category.CategoryId
                });

            var featuredCategory1 = fakeFeaturedCategory1.Generate();

            dbContext.FeaturedCategories.Add(featuredCategory1);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategoryRequest = new Faker<CreateFeaturedCategoryRequest>()
                .CustomInstantiator(f => new CreateFeaturedCategoryRequest
                (
                    OrderNumber: f.Random.Int(1, 100),
                    IsActive: f.Random.Bool(),
                    CategoryId: category.CategoryId // Same category
                ));

            var request = fakeFeaturedCategoryRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateFeaturedCategoryAsync(request));
        }
    }

    #endregion

    #region UpdateFeaturedCategory

    [Fact]
    public async Task UpdateFeaturedCategory_ReturnsUpdatedFeaturedCategory()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    ImageHorizontalUrl = f.Lorem.Slug(),
                    ImageSquareUrl = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategory = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = f.Random.Bool(),
                    CategoryId = category.CategoryId
                });

            var featuredCategory = fakeFeaturedCategory.Generate();

            dbContext.FeaturedCategories.Add(featuredCategory);
            await dbContext.SaveChangesAsync();

            var fakeUpdateFeaturedCategoryRequest = new Faker<UpdateFeaturedCategoryRequest>()
                .CustomInstantiator(f => new UpdateFeaturedCategoryRequest
                (
                    FeaturedCategoryId: featuredCategory.FeaturedCategoryId,
                    OrderNumber: f.Random.Int(1, 100),
                    IsActive: f.Random.Bool(),
                    CategoryId: category.CategoryId
                ));

            var request = fakeUpdateFeaturedCategoryRequest.Generate();

            var result = await repo.UpdateFeaturedCategoryAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.FeaturedCategoryId);

            var updatedFeaturedCategory =
                dbContext.FeaturedCategories.FirstOrDefault(p => p.FeaturedCategoryId == result.FeaturedCategoryId);
            Assert.NotNull(updatedFeaturedCategory);
            Assert.Equal(request.FeaturedCategoryId, updatedFeaturedCategory.FeaturedCategoryId);
            Assert.Equal(request.OrderNumber, updatedFeaturedCategory.OrderNumber);
            Assert.Equal(request.IsActive, updatedFeaturedCategory.IsActive);
            Assert.Equal(request.CategoryId, updatedFeaturedCategory.CategoryId);
        }
    }

    [Fact]
    public async Task UpdateFeaturedCategory_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    ImageHorizontalUrl = f.Lorem.Slug(),
                    ImageSquareUrl = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategory1 = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = 1,
                    IsActive = f.Random.Bool(),
                    CategoryId = category.CategoryId
                });

            var fakeFeaturedCategory2 = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = 2,
                    IsActive = f.Random.Bool(),
                    CategoryId = category.CategoryId
                });

            var featuredCategory1 = fakeFeaturedCategory1.Generate();
            var featuredCategory2 = fakeFeaturedCategory2.Generate();

            dbContext.FeaturedCategories.AddRange(featuredCategory1, featuredCategory2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateFeaturedCategoryRequest = new Faker<UpdateFeaturedCategoryRequest>()
                .CustomInstantiator(f => new UpdateFeaturedCategoryRequest
                (
                    FeaturedCategoryId: featuredCategory2.FeaturedCategoryId,
                    OrderNumber: 1, // Same order number as featuredCategory1
                    IsActive: f.Random.Bool(),
                    CategoryId: category.CategoryId
                ));

            var request = fakeUpdateFeaturedCategoryRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateFeaturedCategoryAsync(request));
        }
    }

    [Fact]
    public async Task UpdateFeaturedCategory_ThrowsConflictException_WhenCategoryExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    ImageHorizontalUrl = f.Lorem.Slug(),
                    ImageSquareUrl = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategory1 = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = f.Random.Bool(),
                    CategoryId = category.CategoryId
                });

            var fakeFeaturedCategory2 = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = f.Random.Bool(),
                    CategoryId = category.CategoryId
                });

            var featuredCategory1 = fakeFeaturedCategory1.Generate();
            var featuredCategory2 = fakeFeaturedCategory2.Generate();

            dbContext.FeaturedCategories.AddRange(featuredCategory1, featuredCategory2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateFeaturedCategoryRequest = new Faker<UpdateFeaturedCategoryRequest>()
                .CustomInstantiator(f => new UpdateFeaturedCategoryRequest
                (
                    FeaturedCategoryId: featuredCategory2.FeaturedCategoryId,
                    OrderNumber: f.Random.Int(1, 100),
                    IsActive: f.Random.Bool(),
                    CategoryId: category.CategoryId // Same category as featuredCategory1
                ));

            var request = fakeUpdateFeaturedCategoryRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateFeaturedCategoryAsync(request));
        }
    }

    #endregion

    #region DeleteFeaturedCategory

    [Fact]
    public async Task DeleteFeaturedCategory_ReturnsDeletedFeaturedCategoryId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFeaturedCategory = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = f.Random.Bool(),
                    CategoryId = Guid.NewGuid()
                });

            var featuredCategory = fakeFeaturedCategory.Generate();

            dbContext.FeaturedCategories.Add(featuredCategory);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteFeaturedCategoryAsync(featuredCategory.FeaturedCategoryId);

            Assert.Equal(featuredCategory.FeaturedCategoryId, result);
        }
    }

    [Fact]
    public async Task DeleteFeaturedCategory_ThrowsNotFoundException_WhenFeaturedCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteFeaturedCategoryAsync(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetFeaturedCategory

    [Fact]
    public async Task GetFeaturedCategory_ReturnsFeaturedCategory()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    ImageHorizontalUrl = f.Lorem.Slug(),
                    ImageSquareUrl = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategory = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = f.Random.Bool(),
                    CategoryId = category.CategoryId
                });

            var featuredCategory = fakeFeaturedCategory.Generate();

            dbContext.FeaturedCategories.Add(featuredCategory);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetFeaturedCategoryAsync(featuredCategory.FeaturedCategoryId);

            Assert.NotNull(result);
            Assert.Equal(featuredCategory.FeaturedCategoryId, result.FeaturedCategoryId);
            Assert.Equal(featuredCategory.OrderNumber, result.OrderNumber);
            Assert.Equal(featuredCategory.IsActive, result.IsActive);
            Assert.Equal(featuredCategory.CategoryId, result.GetCategoryResponse.CategoryId);
        }
    }

    [Fact]
    public async Task GetFeaturedCategory_ThrowsNotFoundException_WhenFeaturedCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetFeaturedCategoryAsync(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetFeaturedCategories

    [Fact]
    public async Task GetFeaturedCategories_ReturnsFeaturedCategories()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    ImageHorizontalUrl = f.Lorem.Slug(),
                    ImageSquareUrl = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategory = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = f.Random.Bool(),
                    CategoryId = category.CategoryId
                });

            var featuredCategory1 = fakeFeaturedCategory.Generate();
            var featuredCategory2 = fakeFeaturedCategory.Generate();

            dbContext.FeaturedCategories.RemoveRange(dbContext.FeaturedCategories);
            dbContext.FeaturedCategories.AddRange(featuredCategory1, featuredCategory2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetFeaturedCategoriesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.FeaturedCategoryId == featuredCategory1.FeaturedCategoryId);
            Assert.Contains(result, r => r.FeaturedCategoryId == featuredCategory2.FeaturedCategoryId);
        }
    }
    
        [Fact]
    public async Task GetActiveFeaturedCategories_ReturnsFeaturedCategories()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeaturedCategoryRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeCategory = new Faker<Category>()
                .CustomInstantiator(f => new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    ImageHorizontalUrl = f.Lorem.Slug(),
                    ImageSquareUrl = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var fakeFeaturedCategory1 = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = true,
                    CategoryId = category.CategoryId
                });
            
            var fakeFeaturedCategory2 = new Faker<FeaturedCategory>()
                .CustomInstantiator(f => new FeaturedCategory
                {
                    FeaturedCategoryId = Guid.NewGuid(),
                    OrderNumber = f.Random.Int(1, 100),
                    IsActive = false,
                    CategoryId = category.CategoryId
                });


            var featuredCategory1 = fakeFeaturedCategory1.Generate();
            var featuredCategory2 = fakeFeaturedCategory2.Generate();

            dbContext.FeaturedCategories.RemoveRange(dbContext.FeaturedCategories);
            dbContext.FeaturedCategories.AddRange(featuredCategory1, featuredCategory2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetFeaturedCategoriesAsync(p => p.IsActive);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, r => r.FeaturedCategoryId == featuredCategory1.FeaturedCategoryId);
            foreach (var item in result)
            {
                Assert.True(item.IsActive);
            }
        }
    }

    #endregion
}