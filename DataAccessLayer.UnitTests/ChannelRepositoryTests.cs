using Bogus;
using BusinessLayer.Mapper;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.DTO.Channel.Request;
using Shared.Interface;
using BusinessLayer.Middlewares;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.UnitTests;

public class ChannelRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public ChannelRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering ChannelRepository and IMapper
        services.AddScoped<ChannelRepository>();
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

    #region CreateChannel

    [Fact]
    public async Task CreateChannel_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ChannelRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeChannelRequest = new Faker<CreateChannelRequest>()
                .CustomInstantiator(f => new CreateChannelRequest
                (
                    Name: f.Company.CompanyName(),
                    Logo: new Mock<IFormFile>().Object,
                    IsActive: f.Random.Bool()
                ));

            var request = fakeChannelRequest.Generate();

            var result = await repo.CreateChannel(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.ChannelId);

            var channel = dbContext.Channels.FirstOrDefault(p => p.ChannelId == result.ChannelId);
            Assert.NotNull(channel);
            Assert.Equal(request.Name.Trim(), channel.Name);
            Assert.Equal("http://test.com/image.jpg", channel.Logo);
            Assert.Equal(request.IsActive, channel.IsActive);
        }
    }

    #endregion

    #region UpdateChannel

    [Fact]
    public async Task UpdateChannel_ReturnsUpdatedChannel()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ChannelRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeChannel = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    Logo = "http://test.com/image.jpg",
                    IsActive = f.Random.Bool()
                });

            var channel = fakeChannel.Generate();

            dbContext.Channels.Add(channel);
            await dbContext.SaveChangesAsync();

            var fakeUpdateChannelRequest = new Faker<UpdateChannelRequest>()
                .CustomInstantiator(f => new UpdateChannelRequest
                (
                    ChannelId: channel.ChannelId,
                    Name: f.Company.CompanyName(),
                    Logo: new Mock<IFormFile>().Object,
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateChannelRequest.Generate();

            var result = await repo.UpdateChannel(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.ChannelId);

            var updatedChannel = dbContext.Channels.FirstOrDefault(p => p.ChannelId == result.ChannelId);
            Assert.NotNull(updatedChannel);
            Assert.Equal(request.ChannelId, updatedChannel.ChannelId);
            Assert.Equal(request.Name.Trim(), updatedChannel.Name);
            Assert.Equal("http://test.com/image.jpg", updatedChannel.Logo); // Mocked Image Service URL
            Assert.Equal(request.IsActive, updatedChannel.IsActive);
        }
    }

    #endregion

    #region DeleteChannel

    [Fact]
    public async Task DeleteChannel_ReturnsDeletedChannelId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ChannelRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeChannel = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    Logo = "http://test.com/image.jpg",
                    IsActive = f.Random.Bool()
                });

            var channel = fakeChannel.Generate();

            dbContext.Channels.Add(channel);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteChannel(channel.ChannelId);

            Assert.Equal(channel.ChannelId, result);
        }
    }

    [Fact]
    public async Task DeleteChannel_ThrowsNotFoundException_WhenChannelDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ChannelRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteChannel(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetChannel

    [Fact]
    public async Task GetChannel_ReturnsChannel()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ChannelRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeChannel = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    Logo = "http://test.com/image.jpg",
                    IsActive = f.Random.Bool()
                });

            var channel = fakeChannel.Generate();

            dbContext.Channels.Add(channel);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetChannel(channel.ChannelId);

            Assert.NotNull(result);
            Assert.Equal(channel.ChannelId, result.ChannelId);
            Assert.Equal(channel.Name, result.Name);
            Assert.Equal(channel.Logo, result.Logo);
            Assert.Equal(channel.IsActive, result.IsActive);
        }
    }

    [Fact]
    public async Task GetChannel_ThrowsNotFoundException_WhenChannelDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ChannelRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetChannel(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetChannels

    [Fact]
    public async Task GetChannels_ReturnsChannels()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ChannelRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeChannel1 = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    Logo = "http://test.com/image1.jpg",
                    IsActive = f.Random.Bool()
                });

            var fakeChannel2 = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    Logo = "http://test.com/image2.jpg",
                    IsActive = f.Random.Bool()
                });

            var channel1 = fakeChannel1.Generate();
            var channel2 = fakeChannel2.Generate();

            dbContext.Channels.AddRange(channel1, channel2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetChannels();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.ChannelId == channel1.ChannelId);
            Assert.Contains(result, r => r.ChannelId == channel2.ChannelId);
        }
    }

    // [Fact]
    // public async Task GetChannels_ReturnsChannels()
    // {
    //     using (var scope = _serviceProvider.CreateScope())
    //     {
    //         var scopedService = scope.ServiceProvider;
    //         var repo = scopedService.GetRequiredService<ChannelRepository>();
    //         var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();
    //
    //         var fakeChannel1 = new Faker<Channel>()
    //             .CustomInstantiator(f => new Channel
    //             {
    //                 ChannelId = Guid.NewGuid(),
    //                 Name = f.Company.CompanyName(),
    //                 Logo = "http://test.com/image1.jpg",
    //                 IsActive = true
    //             });
    //
    //         var fakeChannel2 = new Faker<Channel>()
    //             .CustomInstantiator(f => new Channel
    //             {
    //                 ChannelId = Guid.NewGuid(),
    //                 Name = f.Company.CompanyName(),
    //                 Logo = "http://test.com/image2.jpg",
    //                 IsActive = false
    //             });
    //
    //         var channel1 = fakeChannel1.Generate();
    //         var channel2 = fakeChannel2.Generate();
    //
    //         dbContext.Channels.AddRange(channel1, channel2);
    //         await dbContext.SaveChangesAsync();
    //
    //         var result = await repo.GetChannels(p => p.IsActive);
    //
    //         Assert.NotNull(result);
    //         Assert.Single(result);
    //         Assert.Contains(result, r => r.ChannelId == channel1.ChannelId);
    //         foreach (var item in result)
    //         {
    //             Assert.True(item.IsActive);
    //         }
    //     }
    // }

    #endregion
}