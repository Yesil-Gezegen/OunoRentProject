using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.Feature.Request;

namespace DataAccessLayer.UnitTests;

public class FeatureRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public FeatureRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering FeatureRepository and IMapper
        services.AddScoped<FeatureRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        _serviceProvider = services.BuildServiceProvider();
    }


    #region CreateFeature

    [Fact]
    public async Task CreateFeature_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();
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

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = Guid.NewGuid(), // This will be replaced
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    IsActive = f.Random.Bool()
                });

            var category = fakeCategory.Generate();
            var subCategory = fakeSubCategory.Generate();
            subCategory.CategoryId = category.CategoryId;

            dbContext.Categories.Add(category);
            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeFeatureRequest = new Faker<CreateFeatureRequest>()
                .CustomInstantiator(f => new CreateFeatureRequest
                (
                    FeatureName: f.Commerce.ProductName(),
                    FeatureType: f.Commerce.ProductMaterial(),
                    CategoryId: category.CategoryId,
                    SubCategoryId: subCategory.SubCategoryId,
                    IsActive: f.Random.Bool()
                ));

            var request = fakeFeatureRequest.Generate();

            var result = await repo.CreateFeatureAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.FeatureId);

            var feature = dbContext.Features.FirstOrDefault(p => p.FeatureId == result.FeatureId);

            Assert.NotNull(feature);
            Assert.Equal(request.FeatureName, feature.FeatureName);
            Assert.Equal(request.FeatureType, feature.FeatureType);
            Assert.Equal(request.CategoryId, feature.CategoryId);
            Assert.Equal(request.SubCategoryId, feature.SubCategoryId);
            Assert.Equal(request.IsActive, feature.IsActive);
        }
    }

    [Fact]
    public async Task CreateFeature_ThrowsNotFoundException_WhenCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();

            var fakeFeatureRequest = new Faker<CreateFeatureRequest>()
                .CustomInstantiator(f => new CreateFeatureRequest
                (
                    FeatureName: f.Commerce.ProductName(),
                    FeatureType: f.Commerce.ProductMaterial(),
                    CategoryId: Guid.NewGuid(), // Non-existent category
                    SubCategoryId: Guid.NewGuid(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeFeatureRequest.Generate();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.CreateFeatureAsync(request));
        }
    }

    [Fact]
    public async Task CreateFeature_ThrowsNotFoundException_WhenSubCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();
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

            var fakeFeatureRequest = new Faker<CreateFeatureRequest>()
                .CustomInstantiator(f => new CreateFeatureRequest
                (
                    FeatureName: f.Commerce.ProductName(),
                    FeatureType: f.Commerce.ProductMaterial(),
                    CategoryId: category.CategoryId,
                    SubCategoryId: Guid.NewGuid(), // Non-existent sub-category
                    IsActive: f.Random.Bool()
                ));

            var request = fakeFeatureRequest.Generate();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.CreateFeatureAsync(request));
        }
    }

    #endregion

    #region UpdateFeature

    [Fact]
    public async Task UpdateFeature_ReturnsUpdatedFeature()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();
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

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = Guid.NewGuid(), // This will be replaced
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();
            var subCategory = fakeSubCategory.Generate();
            subCategory.CategoryId = category.CategoryId;

            dbContext.Categories.Add(category);
            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeFeature = new Faker<Feature>()
                .CustomInstantiator(f => new Feature
                {
                    FeatureId = Guid.NewGuid(),
                    FeatureName = f.Commerce.ProductName(),
                    FeatureType = f.Commerce.ProductMaterial(),
                    CategoryId = category.CategoryId,
                    SubCategoryId = subCategory.SubCategoryId,
                    IsActive = f.Random.Bool()
                });

            var feature = fakeFeature.Generate();

            dbContext.Features.Add(feature);
            await dbContext.SaveChangesAsync();

            var fakeUpdateFeatureRequest = new Faker<UpdateFeatureRequest>()
                .CustomInstantiator(f => new UpdateFeatureRequest
                (
                    FeatureId: feature.FeatureId,
                    FeatureName: f.Commerce.ProductName(),
                    FeatureType: f.Commerce.ProductMaterial(),
                    CategoryId: category.CategoryId,
                    SubCategoryId: subCategory.SubCategoryId,
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateFeatureRequest.Generate();

            var result = await repo.UpdateFeatureAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.FeatureId);

            var updatedFeature = dbContext.Features.FirstOrDefault(p => p.FeatureId == result.FeatureId);
            Assert.NotNull(updatedFeature);
            Assert.Equal(request.FeatureId, updatedFeature.FeatureId);
            Assert.Equal(request.FeatureName, updatedFeature.FeatureName);
            Assert.Equal(request.FeatureType, updatedFeature.FeatureType);
            Assert.Equal(request.CategoryId, updatedFeature.CategoryId);
            Assert.Equal(request.SubCategoryId, updatedFeature.SubCategoryId);
            Assert.Equal(request.IsActive, updatedFeature.IsActive);
        }
    }

    [Fact]
    public async Task UpdateFeature_ThrowsNotFoundException_WhenCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();

            var fakeUpdateFeatureRequest = new Faker<UpdateFeatureRequest>()
                .CustomInstantiator(f => new UpdateFeatureRequest
                (
                    FeatureId: Guid.NewGuid(), // Non-existent feature
                    FeatureName: f.Commerce.ProductName(),
                    FeatureType: f.Commerce.ProductMaterial(),
                    CategoryId: Guid.NewGuid(), // Non-existent category
                    SubCategoryId: Guid.NewGuid(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateFeatureRequest.Generate();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.UpdateFeatureAsync(request));
        }
    }

    [Fact]
    public async Task UpdateFeature_ThrowsNotFoundException_WhenSubCategoryDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();
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

            var fakeFeature = new Faker<Feature>()
                .CustomInstantiator(f => new Feature
                {
                    FeatureId = Guid.NewGuid(),
                    FeatureName = f.Commerce.ProductName(),
                    FeatureType = f.Commerce.ProductMaterial(),
                    CategoryId = category.CategoryId,
                    SubCategoryId = Guid.NewGuid(), // Non-existent sub-category
                    IsActive = f.Random.Bool()
                });

            var feature = fakeFeature.Generate();

            dbContext.Features.Add(feature);
            await dbContext.SaveChangesAsync();

            var fakeUpdateFeatureRequest = new Faker<UpdateFeatureRequest>()
                .CustomInstantiator(f => new UpdateFeatureRequest
                (
                    FeatureId: feature.FeatureId,
                    FeatureName: f.Commerce.ProductName(),
                    FeatureType: f.Commerce.ProductMaterial(),
                    CategoryId: category.CategoryId,
                    SubCategoryId: Guid.NewGuid(), // Non-existent sub-category
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateFeatureRequest.Generate();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.UpdateFeatureAsync(request));
        }
    }

    #endregion

    #region DeleteFeature

    [Fact]
    public async Task DeleteFeature_ReturnsDeletedFeature()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFeature = new Faker<Feature>()
                .CustomInstantiator(f => new Feature
                {
                    FeatureId = Guid.NewGuid(),
                    FeatureName = f.Commerce.ProductName(),
                    FeatureType = f.Commerce.ProductMaterial(),
                    CategoryId = Guid.NewGuid(),
                    SubCategoryId = Guid.NewGuid(),
                    IsActive = f.Random.Bool()
                });

            var feature = fakeFeature.Generate();

            dbContext.Features.Add(feature);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteFeatureAsync(feature.FeatureId);

            Assert.NotNull(result);
            Assert.Equal(feature.FeatureId, result.FeatureId);
        }
    }

    [Fact]
    public async Task DeleteFeature_ThrowsNotFoundException_WhenFeatureDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteFeatureAsync(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetFeature

    [Fact]
    public async Task GetFeature_ReturnsFeature()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();
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

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = Guid.NewGuid(), // This will be replaced
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();
            var subCategory = fakeSubCategory.Generate();
            subCategory.CategoryId = category.CategoryId;

            dbContext.Categories.Add(category);
            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeFeature = new Faker<Feature>()
                .CustomInstantiator(f => new Feature
                {
                    FeatureId = Guid.NewGuid(),
                    FeatureName = f.Commerce.ProductName(),
                    FeatureType = f.Commerce.ProductMaterial(),
                    CategoryId = category.CategoryId,
                    SubCategoryId = subCategory.SubCategoryId,
                    IsActive = f.Random.Bool()
                });

            var feature = fakeFeature.Generate();

            dbContext.Features.Add(feature);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetFeatureAsync(feature.FeatureId);

            Assert.NotNull(result);
            Assert.Equal(feature.FeatureId, result.FeatureId);
            Assert.Equal(feature.FeatureName, result.FeatureName);
            Assert.Equal(feature.FeatureType, result.FeatureType);
            Assert.Equal(feature.CategoryId, result.Category.CategoryId);
            Assert.Equal(feature.SubCategoryId, result.SubCategory.SubCategoryId);
            Assert.Equal(feature.IsActive, result.IsActive);
        }
    }

    [Fact]
    public async Task GetFeature_ThrowsNotFoundException_WhenFeatureDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetFeatureAsync(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetFeatures

    [Fact]
    public async Task GetFeatures_ReturnsFeatures()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();
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

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = Guid.NewGuid(), // This will be replaced
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();
            var subCategory = fakeSubCategory.Generate();
            subCategory.CategoryId = category.CategoryId;

            dbContext.Categories.Add(category);
            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeFeature = new Faker<Feature>()
                .CustomInstantiator(f => new Feature
                {
                    FeatureId = Guid.NewGuid(),
                    FeatureName = f.Commerce.ProductName(),
                    FeatureType = f.Commerce.ProductMaterial(),
                    CategoryId = category.CategoryId,
                    SubCategoryId = subCategory.SubCategoryId,
                    IsActive = f.Random.Bool()
                });

            var feature1 = fakeFeature.Generate();
            var feature2 = fakeFeature.Generate();

            dbContext.Features.RemoveRange(dbContext.Features);
            dbContext.Features.AddRange(feature1, feature2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetFeaturesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.FeatureId == feature1.FeatureId);
            Assert.Contains(result, r => r.FeatureId == feature2.FeatureId);
        }
    }

        [Fact]
    public async Task GetActiveFeatures_ReturnsFeatures()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FeatureRepository>();
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

            var fakeSubCategory = new Faker<SubCategory>()
                .CustomInstantiator(f => new SubCategory
                {
                    SubCategoryId = Guid.NewGuid(),
                    CategoryId = Guid.NewGuid(), // This will be replaced
                    Name = f.Commerce.Categories(1)[0],
                    Description = f.Lorem.Paragraph(),
                    OrderNumber = f.Random.Int(),
                    Icon = f.Lorem.Slug(),
                    IsActive = f.Random.Bool(),
                });

            var category = fakeCategory.Generate();
            var subCategory = fakeSubCategory.Generate();
            subCategory.CategoryId = category.CategoryId;

            dbContext.Categories.Add(category);
            dbContext.SubCategories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var fakeFeature1 = new Faker<Feature>()
                .CustomInstantiator(f => new Feature
                {
                    FeatureId = Guid.NewGuid(),
                    FeatureName = f.Commerce.ProductName(),
                    FeatureType = f.Commerce.ProductMaterial(),
                    CategoryId = category.CategoryId,
                    SubCategoryId = subCategory.SubCategoryId,
                    IsActive = true
                });
            
            var fakeFeature2 = new Faker<Feature>()
                .CustomInstantiator(f => new Feature
                {
                    FeatureId = Guid.NewGuid(),
                    FeatureName = f.Commerce.ProductName(),
                    FeatureType = f.Commerce.ProductMaterial(),
                    CategoryId = category.CategoryId,
                    SubCategoryId = subCategory.SubCategoryId,
                    IsActive = false
                });

            var feature1 = fakeFeature1.Generate();
            var feature2 = fakeFeature2.Generate();

            dbContext.Features.RemoveRange(dbContext.Features);
            dbContext.Features.AddRange(feature1, feature2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetFeaturesAsync(p => p.IsActive);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, r => r.FeatureId == feature1.FeatureId);
            foreach (var item in result)
            {
                Assert.True(item.IsActive);
            }
        }
    }
    #endregion
}