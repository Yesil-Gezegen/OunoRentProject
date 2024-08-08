using Bogus;
using BusinessLayer.Mapper;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.Slider.Request;
using Shared.Interface;
using BusinessLayer.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;

namespace DataAccessLayer.UnitTests;

public class SliderRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public SliderRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering SliderRepository and IMapper
        services.AddScoped<SliderRepository>();
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

    #region CreateSlider

    [Fact]
    public async Task CreateSlider_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSliderRequest = new Faker<CreateSliderRequest>()
                .CustomInstantiator(f => new CreateSliderRequest
                (
                    Title: f.Lorem.Sentence(),
                    MainImage: new Mock<IFormFile>().Object,
                    MobileImage: new Mock<IFormFile>().Object,
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: f.Random.Int(1, 100),
                    ActiveFrom: DateTime.UtcNow,
                    ActiveTo: DateTime.UtcNow.AddMonths(1),
                    Duration: f.Random.Int(1, 60),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeSliderRequest.Generate();

            var result = await repo.CreateSlider(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.SliderId);

            var slider = dbContext.Sliders.FirstOrDefault(p => p.SliderId == result.SliderId);
            Assert.NotNull(slider);
            Assert.Equal(request.Title, slider.Title);
            Assert.Equal(request.TargetUrl, slider.TargetUrl);
            Assert.Equal(request.OrderNumber, slider.OrderNumber);
            Assert.Equal(request.ActiveFrom.ToUniversalTime(), slider.ActiveFrom);
            Assert.Equal(request.ActiveTo.ToUniversalTime(), slider.ActiveTo);
            Assert.Equal(request.Duration, slider.Duration);
            Assert.Equal(request.IsActive, slider.IsActive);
        }
    }

    [Fact]
    public async Task CreateSlider_ThrowsConflictException_WhenTitleExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSlider = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = "Existing Title",
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = f.Random.Bool()
                });

            var slider = fakeSlider.Generate();

            dbContext.Sliders.Add(slider);
            await dbContext.SaveChangesAsync();

            var fakeSliderRequest = new Faker<CreateSliderRequest>()
                .CustomInstantiator(f => new CreateSliderRequest
                (
                    Title: "Existing Title", // Same title
                    MainImage: new Mock<IFormFile>().Object,
                    MobileImage: new Mock<IFormFile>().Object,
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: f.Random.Int(1, 100),
                    ActiveFrom: DateTime.UtcNow,
                    ActiveTo: DateTime.UtcNow.AddMonths(1),
                    Duration: f.Random.Int(1, 60),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeSliderRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateSlider(request));
        }
    }

    [Fact]
    public async Task CreateSlider_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSlider = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = f.Lorem.Sentence(),
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = 1, // Specific order number
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = f.Random.Bool()
                });

            var slider = fakeSlider.Generate();

            dbContext.Sliders.Add(slider);
            await dbContext.SaveChangesAsync();

            var fakeSliderRequest = new Faker<CreateSliderRequest>()
                .CustomInstantiator(f => new CreateSliderRequest
                (
                    Title: f.Lorem.Sentence(),
                    MainImage: new Mock<IFormFile>().Object,
                    MobileImage: new Mock<IFormFile>().Object,
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: 1, // Same order number
                    ActiveFrom: DateTime.UtcNow,
                    ActiveTo: DateTime.UtcNow.AddMonths(1),
                    Duration: f.Random.Int(1, 60),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeSliderRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateSlider(request));
        }
    }

    #endregion

    #region UpdateSlider

    [Fact]
    public async Task UpdateSlider_ReturnsUpdatedSlider()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSlider = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = f.Lorem.Sentence(),
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = f.Random.Bool()
                });

            var slider = fakeSlider.Generate();

            dbContext.Sliders.Add(slider);
            await dbContext.SaveChangesAsync();

            var fakeUpdateSliderRequest = new Faker<UpdateSliderRequest>()
                .CustomInstantiator(f => new UpdateSliderRequest
                (
                    SliderId: slider.SliderId,
                    Title: f.Lorem.Sentence(),
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: f.Random.Int(1, 100),
                    ActiveFrom: DateTime.UtcNow,
                    ActiveTo: DateTime.UtcNow.AddMonths(1),
                    Duration: f.Random.Int(1, 60),
                    IsActive: f.Random.Bool(),
                    MainImage: new Mock<IFormFile>().Object,
                    MobileImage: new Mock<IFormFile>().Object
                ));

            var request = fakeUpdateSliderRequest.Generate();

            var result = await repo.UpdateSlider(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.SliderId);

            var updatedSlider = dbContext.Sliders.FirstOrDefault(p => p.SliderId == result.SliderId);
            Assert.NotNull(updatedSlider);
            Assert.Equal(request.SliderId, updatedSlider.SliderId);
            Assert.Equal(request.Title, updatedSlider.Title);
            Assert.Equal(request.TargetUrl, updatedSlider.TargetUrl);
            Assert.Equal(request.OrderNumber, updatedSlider.OrderNumber);
            Assert.Equal(request.ActiveFrom.ToUniversalTime(), updatedSlider.ActiveFrom);
            Assert.Equal(request.ActiveTo.ToUniversalTime(), updatedSlider.ActiveTo);
            Assert.Equal(request.Duration, updatedSlider.Duration);
            Assert.Equal(request.IsActive, updatedSlider.IsActive);
        }
    }

    [Fact]
    public async Task UpdateSlider_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSlider1 = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = f.Lorem.Sentence(),
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = 1, // Specific order number
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = f.Random.Bool()
                });

            var fakeSlider2 = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = f.Lorem.Sentence(),
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = 2,
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = f.Random.Bool()
                });

            var slider1 = fakeSlider1.Generate();
            var slider2 = fakeSlider2.Generate();

            dbContext.Sliders.AddRange(slider1, slider2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateSliderRequest = new Faker<UpdateSliderRequest>()
                .CustomInstantiator(f => new UpdateSliderRequest
                (
                    SliderId: slider2.SliderId,
                    Title: f.Lorem.Sentence(),
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: 1, // Same order number as slider1
                    ActiveFrom: DateTime.UtcNow,
                    ActiveTo: DateTime.UtcNow.AddMonths(1),
                    Duration: f.Random.Int(1, 60),
                    IsActive: f.Random.Bool(),
                    MainImage: new Mock<IFormFile>().Object,
                    MobileImage: new Mock<IFormFile>().Object
                ));

            var request = fakeUpdateSliderRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateSlider(request));
        }
    }

    #endregion

    #region DeleteSlider

    [Fact]
    public async Task DeleteSlider_ReturnsDeletedSliderId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSlider = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = f.Lorem.Sentence(),
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = f.Random.Bool()
                });

            var slider = fakeSlider.Generate();

            dbContext.Sliders.Add(slider);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteSlider(slider.SliderId);

            Assert.Equal(slider.SliderId, result);
        }
    }

    [Fact]
    public async Task DeleteSlider_ThrowsNotFoundException_WhenSliderDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteSlider(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetSlider

    [Fact]
    public async Task GetSlider_ReturnsSlider()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSlider = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = f.Lorem.Sentence(),
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = f.Random.Bool()
                });

            var slider = fakeSlider.Generate();

            dbContext.Sliders.Add(slider);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetSlider(slider.SliderId);

            Assert.NotNull(result);
            Assert.Equal(slider.SliderId, result.SliderId);
            Assert.Equal(slider.Title, result.Title);
            Assert.Equal(slider.MainImageUrl, result.MainImageUrl);
            Assert.Equal(slider.MobileImageUrl, result.MobileImageUrl);
            Assert.Equal(slider.TargetUrl, result.TargetUrl);
            Assert.Equal(slider.OrderNumber, result.OrderNumber);
            Assert.Equal(slider.ActiveFrom.ToUniversalTime(), result.ActiveFrom);
            Assert.Equal(slider.ActiveTo.ToUniversalTime(), result.ActiveTo);
            Assert.Equal(slider.Duration, result.Duration);
            Assert.Equal(slider.IsActive, result.IsActive);
        }
    }

    [Fact]
    public async Task GetSlider_ThrowsNotFoundException_WhenSliderDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetSlider(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetSliders

    [Fact]
    public async Task GetSliders_ReturnsSliders()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSlider1 = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = f.Lorem.Sentence(),
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = f.Random.Bool()
                });

            var fakeSlider2 = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = f.Lorem.Sentence(),
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = f.Random.Bool()
                });

            var slider1 = fakeSlider1.Generate();
            var slider2 = fakeSlider2.Generate();

            dbContext.Sliders.RemoveRange(dbContext.Sliders);
            dbContext.Sliders.AddRange(slider1, slider2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetSliders();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.SliderId == slider1.SliderId);
            Assert.Contains(result, r => r.SliderId == slider2.SliderId);
        }
    }

    [Fact]
    public async Task GetActiveSliders_ReturnsSliders()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<SliderRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeSlider1 = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = f.Lorem.Sentence(),
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = true
                });

            var fakeSlider2 = new Faker<Slider>()
                .CustomInstantiator(f => new Slider
                {
                    SliderId = Guid.NewGuid(),
                    Title = f.Lorem.Sentence(),
                    MainImageUrl = "http://test.com/image.jpg",
                    MobileImageUrl = "http://test.com/image.jpg",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    ActiveFrom = DateTime.UtcNow,
                    ActiveTo = DateTime.UtcNow.AddMonths(1),
                    Duration = f.Random.Int(1, 60),
                    IsActive = false
                });

            var slider1 = fakeSlider1.Generate();
            var slider2 = fakeSlider2.Generate();

            dbContext.Sliders.RemoveRange(dbContext.Sliders);
            dbContext.Sliders.AddRange(slider1, slider2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetSliders(p => p.IsActive);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, r => r.SliderId == slider1.SliderId);
            foreach (var item in result)
            {
                Assert.True(item.IsActive);
            }
        }
    }

    #endregion
}