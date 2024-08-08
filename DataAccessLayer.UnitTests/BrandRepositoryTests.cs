using Bogus;
using BusinessLayer.Mapper;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.DTO.Brand.Request;
using Shared.Interface;
using BusinessLayer.Middlewares;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.UnitTests;

public class BrandRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public BrandRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering BrandRepository and IMapper
        services.AddScoped<BrandRepository>();
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

    #region CreateBrand

    [Fact]
    public async Task CreateBrand_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BrandRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeBrandRequest = new Faker<CreateBrandRequest>()
                .CustomInstantiator(f => new CreateBrandRequest
                (
                    Name: f.Company.CompanyName(),
                    Logo: new Mock<IFormFile>().Object,
                    ShowOnBrands: f.Random.Bool(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeBrandRequest.Generate();

            var result = await repo.CreateBrand(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.BrandId);

            var brand = dbContext.Brands.FirstOrDefault(p => p.BrandId == result.BrandId);
            Assert.Equal(request.Name.Trim(), brand.Name);
            Assert.Equal("http://test.com/image.jpg", brand.Logo);
            Assert.Equal(request.ShowOnBrands, brand.ShowOnBrands);
            Assert.Equal(request.IsActive, brand.IsActive);
        }
    }

    #endregion

    #region UpdateBrand

    [Fact]
    public async Task UpdateBrand_ReturnsUpdatedBrand()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BrandRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeBrand = new Faker<Brand>()
                .CustomInstantiator(f => new Brand
                {
                    BrandId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    Logo = "http://test.com/image.jpg",
                    ShowOnBrands = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var brand = fakeBrand.Generate();

            dbContext.Brands.Add(brand);
            await dbContext.SaveChangesAsync();

            var fakeUpdateBrandRequest = new Faker<UpdateBrandRequest>()
                .CustomInstantiator(f => new UpdateBrandRequest
                (
                    BrandId: brand.BrandId,
                    Name: f.Company.CompanyName(),
                    Logo: new Mock<IFormFile>().Object,
                    ShowOnBrands: f.Random.Bool(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateBrandRequest.Generate();

            var result = await repo.UpdateBrand(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.BrandId);

            var updatedBrand = dbContext.Brands.FirstOrDefault(p => p.BrandId == result.BrandId);
            Assert.Equal(request.BrandId, updatedBrand.BrandId);
            Assert.Equal(request.Name.Trim(), updatedBrand.Name);
            Assert.Equal("http://test.com/image.jpg", updatedBrand.Logo); // Mocked Image Service URL
            Assert.Equal(request.ShowOnBrands, updatedBrand.ShowOnBrands);
            Assert.Equal(request.IsActive, updatedBrand.IsActive);
        }
    }

    #endregion

    #region DeleteBrand

    [Fact]
    public async Task DeleteBrand_ReturnsDeletedBrandId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BrandRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeBrand = new Faker<Brand>()
                .CustomInstantiator(f => new Brand
                {
                    BrandId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    Logo = "http://test.com/image.jpg",
                    ShowOnBrands = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var brand = fakeBrand.Generate();

            dbContext.Brands.Add(brand);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteBrand(brand.BrandId);

            Assert.Equal(brand.BrandId, result);
        }
    }

    [Fact]
    public async Task DeleteBrand_ThrowsNotFoundException_WhenBrandDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BrandRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetBrand(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetBrand

    [Fact]
    public async Task GetBrand_ReturnsBrand()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BrandRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeBrand = new Faker<Brand>()
                .CustomInstantiator(f => new Brand
                {
                    BrandId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    Logo = "http://test.com/image.jpg",
                    ShowOnBrands = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var brand = fakeBrand.Generate();

            dbContext.Brands.Add(brand);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetBrand(brand.BrandId);

            Assert.NotNull(result);
            Assert.Equal(brand.BrandId, result.BrandId);
            Assert.Equal(brand.Name, result.Name);
            Assert.Equal(brand.Logo, result.Logo);
            Assert.Equal(brand.ShowOnBrands, result.ShowOnBrands);
            Assert.Equal(brand.IsActive, result.IsActive);
        }
    }

    [Fact]
    public async Task GetBrand_ThrowsNotFoundException_WhenBrandDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BrandRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetBrand(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetBrands

    [Fact]
    public async Task GetBrands_ReturnsBrands()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<BrandRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeBrand1 = new Faker<Brand>()
                .CustomInstantiator(f => new Brand
                {
                    BrandId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    Logo = "http://test.com/image1.jpg",
                    ShowOnBrands = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var fakeBrand2 = new Faker<Brand>()
                .CustomInstantiator(f => new Brand
                {
                    BrandId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    Logo = "http://test.com/image2.jpg",
                    ShowOnBrands = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var brand1 = fakeBrand1.Generate();
            var brand2 = fakeBrand2.Generate();

            dbContext.Brands.RemoveRange(dbContext.Brands);
            dbContext.Brands.AddRange(brand1, brand2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetBrands();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.BrandId == brand1.BrandId);
            Assert.Contains(result, r => r.BrandId == brand2.BrandId);
        }
    }

    #endregion
}