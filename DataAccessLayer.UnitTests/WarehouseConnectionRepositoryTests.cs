using Bogus;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.WarehouseConnection.Request;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;

namespace DataAccessLayer.UnitTests;

public class WarehouseConnectionRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public WarehouseConnectionRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering WarehouseConnectionRepository and IMapper
        services.AddScoped<WarehouseConnectionRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        _serviceProvider = services.BuildServiceProvider();
    }

    #region CreateWarehouseConnection

    [Fact]
    public async Task CreateWarehouseConnection_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseConnectionRepository>();

            var fakeWarehouse = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = f.Random.Bool()
                });

            var fakeChannel = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Commerce.ProductName(),
                    Logo = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var warehouse = fakeWarehouse.Generate();
            var channel = fakeChannel.Generate();

            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();
            dbContext.Warehouses.Add(warehouse);
            dbContext.Channels.Add(channel);
            await dbContext.SaveChangesAsync();

            var fakeRequest = new Faker<CreateWarehouseConnectionRequest>()
                .CustomInstantiator(f => new CreateWarehouseConnectionRequest
                (
                    WarehouseId: warehouse.WarehouseId,
                    ChannelId: channel.ChannelId,
                    IsActive: f.Random.Bool()
                ));

            var request = fakeRequest.Generate();

            var result = await repo.CreateWarehouseConnection(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.WarehouseConnectionId);

            var warehouseConnection =
                dbContext.WarehouseConnections.FirstOrDefault(p =>
                    p.WarehouseConnectionId == result.WarehouseConnectionId);
            Assert.NotNull(warehouseConnection);
            Assert.Equal(request.WarehouseId, warehouseConnection.WarehouseId);
            Assert.Equal(request.ChannelId, warehouseConnection.ChannelId);
            Assert.Equal(request.IsActive, warehouseConnection.IsActive);
        }
    }

    [Fact]
    public async Task CreateWarehouseConnection_ThrowsConflictException_WhenWarehouseConnectionExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseConnectionRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = f.Random.Bool()
                });

            var fakeChannel = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Commerce.ProductName(),
                    Logo = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var warehouse = fakeWarehouse.Generate();
            var channel = fakeChannel.Generate();

            dbContext.Warehouses.Add(warehouse);
            dbContext.Channels.Add(channel);

            var fakeWarehouseConnection = new Faker<WarehouseConnection>()
                .CustomInstantiator(f => new WarehouseConnection
                {
                    WarehouseConnectionId = Guid.NewGuid(),
                    WarehouseId = warehouse.WarehouseId,
                    ChannelId = channel.ChannelId,
                    IsActive = true
                });

            var warehouseConnection = fakeWarehouseConnection.Generate();

            dbContext.WarehouseConnections.Add(warehouseConnection);
            await dbContext.SaveChangesAsync();

            var fakeRequest = new Faker<CreateWarehouseConnectionRequest>()
                .CustomInstantiator(f => new CreateWarehouseConnectionRequest
                (
                    WarehouseId: warehouse.WarehouseId,
                    ChannelId: channel.ChannelId,
                    IsActive: f.Random.Bool()
                ));

            var request = fakeRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateWarehouseConnection(request));
        }
    }

    #endregion

    #region UpdateWarehouseConnection

    [Fact]
    public async Task UpdateWarehouseConnection_ReturnsUpdatedWarehouseConnection()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseConnectionRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = f.Random.Bool()
                });

            var fakeChannel = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Commerce.ProductName(),
                    Logo = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var warehouse = fakeWarehouse.Generate();
            var channel = fakeChannel.Generate();

            dbContext.Warehouses.Add(warehouse);
            dbContext.Channels.Add(channel);

            var fakeWarehouseConnection = new Faker<WarehouseConnection>()
                .CustomInstantiator(f => new WarehouseConnection
                {
                    WarehouseConnectionId = Guid.NewGuid(),
                    WarehouseId = warehouse.WarehouseId,
                    ChannelId = channel.ChannelId,
                    IsActive = true
                });

            var warehouseConnection = fakeWarehouseConnection.Generate();

            dbContext.WarehouseConnections.Add(warehouseConnection);
            await dbContext.SaveChangesAsync();

            var fakeUpdateRequest = new Faker<UpdateWarehouseConnectionRequest>()
                .CustomInstantiator(f => new UpdateWarehouseConnectionRequest
                (
                    WarehouseConnectionId: warehouseConnection.WarehouseConnectionId,
                    WarehouseId: warehouse.WarehouseId,
                    ChannelId: channel.ChannelId,
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateRequest.Generate();

            var result = await repo.UpdateWarehouseConnection(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.WarehouseConnectionId);

            var updatedWarehouseConnection =
                dbContext.WarehouseConnections.FirstOrDefault(p =>
                    p.WarehouseConnectionId == result.WarehouseConnectionId);
            Assert.NotNull(updatedWarehouseConnection);
            Assert.Equal(request.WarehouseConnectionId, updatedWarehouseConnection.WarehouseConnectionId);
            Assert.Equal(request.WarehouseId, updatedWarehouseConnection.WarehouseId);
            Assert.Equal(request.ChannelId, updatedWarehouseConnection.ChannelId);
            Assert.Equal(request.IsActive, updatedWarehouseConnection.IsActive);
        }
    }

    [Fact]
    public async Task UpdateWarehouseConnection_ThrowsNotFoundException_WhenWarehouseConnectionDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseConnectionRepository>();

            var fakeUpdateRequest = new Faker<UpdateWarehouseConnectionRequest>()
                .CustomInstantiator(f => new UpdateWarehouseConnectionRequest
                (
                    WarehouseConnectionId: Guid.NewGuid(),
                    WarehouseId: Guid.NewGuid(),
                    ChannelId: Guid.NewGuid(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateRequest.Generate();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.UpdateWarehouseConnection(request));
        }
    }

    #endregion

    #region DeleteWarehouseConnection

    [Fact]
    public async Task DeleteWarehouseConnection_ReturnsDeletedWarehouseConnectionId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseConnectionRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    LogoWarehouseId = f.Random.Int()
                });

            var fakeChannel = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Commerce.ProductName(),
                    Logo = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var warehouse = fakeWarehouse.Generate();
            var channel = fakeChannel.Generate();

            dbContext.Warehouses.Add(warehouse);
            dbContext.Channels.Add(channel);

            var fakeWarehouseConnection = new Faker<WarehouseConnection>()
                .CustomInstantiator(f => new WarehouseConnection
                {
                    WarehouseConnectionId = Guid.NewGuid(),
                    WarehouseId = warehouse.WarehouseId,
                    ChannelId = channel.ChannelId,
                    IsActive = true
                });

            var warehouseConnection = fakeWarehouseConnection.Generate();

            dbContext.WarehouseConnections.Add(warehouseConnection);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteWarehouseConnection(warehouseConnection.WarehouseConnectionId);

            Assert.Equal(warehouseConnection.WarehouseConnectionId, result);
        }
    }

    [Fact]
    public async Task DeleteWarehouseConnection_ThrowsNotFoundException_WhenWarehouseConnectionDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseConnectionRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteWarehouseConnection(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetWarehouseConnection

    [Fact]
    public async Task GetWarehouseConnection_ReturnsWarehouseConnection()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseConnectionRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = f.Random.Bool()
                });

            var fakeChannel = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Commerce.ProductName(),
                    Logo = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var warehouse = fakeWarehouse.Generate();
            var channel = fakeChannel.Generate();

            dbContext.Warehouses.Add(warehouse);
            dbContext.Channels.Add(channel);

            var fakeWarehouseConnection = new Faker<WarehouseConnection>()
                .CustomInstantiator(f => new WarehouseConnection
                {
                    WarehouseConnectionId = Guid.NewGuid(),
                    WarehouseId = warehouse.WarehouseId,
                    ChannelId = channel.ChannelId,
                    IsActive = true
                });

            var warehouseConnection = fakeWarehouseConnection.Generate();

            dbContext.WarehouseConnections.Add(warehouseConnection);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetWarehouseConnection(warehouseConnection.WarehouseConnectionId);

            Assert.NotNull(result);
            Assert.Equal(warehouseConnection.WarehouseConnectionId, result.WarehouseConnectionId);
            Assert.Equal(warehouse.Name, result.WarehouseName);
            Assert.Equal(channel.Name, result.ChannelName);
            Assert.Equal(warehouseConnection.IsActive, result.IsActive);
        }
    }

    [Fact]
    public async Task GetWarehouseConnection_ThrowsNotFoundException_WhenWarehouseConnectionDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseConnectionRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetWarehouseConnection(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetWarehouseConnections

    [Fact]
    public async Task GetWarehouseConnections_ReturnsWarehouseConnections()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseConnectionRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Commerce.Department(),
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = f.Random.Bool()
                });

            var fakeChannel = new Faker<Channel>()
                .CustomInstantiator(f => new Channel
                {
                    ChannelId = Guid.NewGuid(),
                    Name = f.Commerce.ProductName(),
                    Logo = f.Image.PicsumUrl(),
                    IsActive = f.Random.Bool()
                });

            var warehouse = fakeWarehouse.Generate();
            var channel = fakeChannel.Generate();

            dbContext.Warehouses.Add(warehouse);
            dbContext.Channels.Add(channel);

            var fakeWarehouseConnection1 = new Faker<WarehouseConnection>()
                .CustomInstantiator(f => new WarehouseConnection
                {
                    WarehouseConnectionId = Guid.NewGuid(),
                    WarehouseId = warehouse.WarehouseId,
                    ChannelId = channel.ChannelId,
                    IsActive = true
                });

            var fakeWarehouseConnection2 = new Faker<WarehouseConnection>()
                .CustomInstantiator(f => new WarehouseConnection
                {
                    WarehouseConnectionId = Guid.NewGuid(),
                    WarehouseId = warehouse.WarehouseId,
                    ChannelId = channel.ChannelId,
                    IsActive = false
                });

            var warehouseConnection1 = fakeWarehouseConnection1.Generate();
            var warehouseConnection2 = fakeWarehouseConnection2.Generate();

            dbContext.WarehouseConnections.RemoveRange(dbContext.WarehouseConnections);
            dbContext.WarehouseConnections.AddRange(warehouseConnection1, warehouseConnection2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetWarehouseConnections();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.WarehouseConnectionId == warehouseConnection1.WarehouseConnectionId);
            Assert.Contains(result, r => r.WarehouseConnectionId == warehouseConnection2.WarehouseConnectionId);
        }
    }

    // [Fact]
    // public async Task GetActiveWarehouseConnections_ReturnsWarehouseConnections()
    // {
    //     using (var scope = _serviceProvider.CreateScope())
    //     {
    //         var scopedService = scope.ServiceProvider;
    //         var repo = scopedService.GetRequiredService<WarehouseConnectionRepository>();
    //         var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();
    //
    //         var fakeWarehouse = new Faker<Warehouse>()
    //             .CustomInstantiator(f => new Warehouse
    //             {
    //                 WarehouseId = Guid.NewGuid(),
    //                 Name = f.Commerce.Department()
    //             });
    //
    //         var fakeChannel = new Faker<Channel>()
    //             .CustomInstantiator(f => new Channel
    //             {
    //                 ChannelId = Guid.NewGuid(),
    //                 Name = f.Commerce.ProductName()
    //             });
    //
    //         var warehouse = fakeWarehouse.Generate();
    //         var channel = fakeChannel.Generate();
    //
    //         dbContext.Warehouses.Add(warehouse);
    //         dbContext.Channels.Add(channel);
    //
    //         var fakeWarehouseConnection1 = new Faker<WarehouseConnection>()
    //             .CustomInstantiator(f => new WarehouseConnection
    //             {
    //                 WarehouseConnectionId = Guid.NewGuid(),
    //                 WarehouseId = warehouse.WarehouseId,
    //                 ChannelId = channel.ChannelId,
    //                 IsActive = true
    //             });
    //
    //         var fakeWarehouseConnection2 = new Faker<WarehouseConnection>()
    //             .CustomInstantiator(f => new WarehouseConnection
    //             {
    //                 WarehouseConnectionId = Guid.NewGuid(),
    //                 WarehouseId = warehouse.WarehouseId,
    //                 ChannelId = channel.ChannelId,
    //                 IsActive = false
    //             });
    //
    //         var warehouseConnection1 = fakeWarehouseConnection1.Generate();
    //         var warehouseConnection2 = fakeWarehouseConnection2.Generate();
    //
    //         dbContext.RemoveRange(dbContext.WarehouseConnections);
    //         dbContext.WarehouseConnections.AddRange(warehouseConnection1, warehouseConnection2);
    //         await dbContext.SaveChangesAsync();
    //
    //         var result = await repo.GetWarehouseConnections(p => p.IsActive);
    //
    //         Assert.NotNull(result);
    //         Assert.Single(result);
    //         Assert.Contains(result, r => r.WarehouseConnectionId == warehouseConnection1.WarehouseConnectionId);
    //         foreach (var item in result)
    //         {
    //             Assert.True(item.IsActive);
    //         }
    //     }
    // }

    #endregion
}