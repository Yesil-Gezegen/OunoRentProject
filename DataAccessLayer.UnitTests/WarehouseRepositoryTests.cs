using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.Warehouse.Request;

namespace DataAccessLayer.UnitTests;

public class WarehouseRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public WarehouseRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering WarehouseRepository and IMapper
        services.AddScoped<WarehouseRepository>();
        services.AddAutoMapper(typeof(MapperProfile)); 

        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task CreateWarehouse_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();
                
            var fakeRequest = new Faker<CreateWarehouseRequest>()
                .CustomInstantiator(f => new CreateWarehouseRequest
                (
                    Name : f.Company.CompanyName(),
                    LogoWarehouseId : f.Random.Int(),
                    IsActive : f.Random.Bool()
                ));

            var request = fakeRequest.Generate();

            var result = await repo.CreateWarehouse(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.WarehouseId);

            var warehouse = dbContext.Warehouses.FirstOrDefault(p => p.WarehouseId == result.WarehouseId);
            Assert.NotNull(warehouse);
            Assert.Equal(request.Name, warehouse.Name);
            Assert.Equal(request.LogoWarehouseId, warehouse.LogoWarehouseId);
            Assert.Equal(request.IsActive, warehouse.IsActive);
        }
    }

    [Fact]
    public async Task CreateWarehouse_ThrowsConflictException_WhenWarehouseNameExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = "Existing Warehouse",
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = true
                });

            var warehouse = fakeWarehouse.Generate();

            dbContext.Warehouses.Add(warehouse);
            await dbContext.SaveChangesAsync();

            var fakeRequest = new Faker<CreateWarehouseRequest>()
                .CustomInstantiator(f => new CreateWarehouseRequest
                (
                    Name : "Existing Warehouse", // Same name
                    LogoWarehouseId : f.Random.Int(),
                    IsActive : f.Random.Bool()
                ));

            var request = fakeRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateWarehouse(request));
        }
    }
        
    [Fact]
    public async Task GetWarehouses_ReturnsWarehouses()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse1 = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = true
                });

            var fakeWarehouse2 = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = false
                });

            var warehouse1 = fakeWarehouse1.Generate();
            var warehouse2 = fakeWarehouse2.Generate();

            dbContext.RemoveRange(dbContext.Warehouses);
            dbContext.Warehouses.AddRange(warehouse1, warehouse2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetWarehouses();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.WarehouseId == warehouse1.WarehouseId);
            Assert.Contains(result, r => r.WarehouseId == warehouse2.WarehouseId);
        }
    }

    // [Fact]
    // public async Task GetActiveWarehouses_ReturnsWarehouses()
    // {
    //     using (var scope = _serviceProvider.CreateScope())
    //     {
    //         var scopedService = scope.ServiceProvider;
    //         var repo = scopedService.GetRequiredService<WarehouseRepository>();
    //         var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();
    //
    //         var fakeWarehouse1 = new Faker<Warehouse>()
    //             .CustomInstantiator(f => new Warehouse
    //             {
    //                 WarehouseId = Guid.NewGuid(),
    //                 Name = f.Company.CompanyName(),
    //                 LogoWarehouseId = f.Random.Int(),
    //                 IsActive = true
    //             });
    //
    //         var fakeWarehouse2 = new Faker<Warehouse>()
    //             .CustomInstantiator(f => new Warehouse
    //             {
    //                 WarehouseId = Guid.NewGuid(),
    //                 Name = f.Company.CompanyName(),
    //                 LogoWarehouseId = f.Random.Int(),
    //                 IsActive = false
    //             });
    //
    //         var warehouse1 = fakeWarehouse1.Generate();
    //         var warehouse2 = fakeWarehouse2.Generate();
    //
    //         dbContext.Warehouses.RemoveRange(dbContext.Warehouses);
    //         dbContext.Warehouses.AddRange(warehouse1, warehouse2);
    //         await dbContext.SaveChangesAsync();
    //
    //         var result = await repo.GetWarehouses(p => p.IsActive);
    //
    //         Assert.NotNull(result);
    //         Assert.Single(result);
    //         Assert.Contains(result, r => r.WarehouseId == warehouse1.WarehouseId);
    //         foreach (var item in result)
    //         {
    //             Assert.True(item.IsActive);
    //         }
    //     }
    // }
        
    [Fact]
    public async Task GetWarehouse_ReturnsWarehouse()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = true
                });

            var warehouse = fakeWarehouse.Generate();

            dbContext.Warehouses.Add(warehouse);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetWarehouse(warehouse.WarehouseId);

            Assert.NotNull(result);
            Assert.Equal(warehouse.WarehouseId, result.WarehouseId);
            Assert.Equal(warehouse.Name, result.Name);
            Assert.Equal(warehouse.LogoWarehouseId, result.LogoWarehouseId);
            Assert.Equal(warehouse.IsActive, result.IsActive);
        }
    }

    [Fact]
    public async Task GetWarehouse_ThrowsNotFoundException_WhenWarehouseDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetWarehouse(Guid.NewGuid()));
        }
    }
        
    [Fact]
    public async Task UpdateWarehouse_ReturnsUpdatedWarehouse()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = true
                });

            var warehouse = fakeWarehouse.Generate();

            dbContext.Warehouses.Add(warehouse);
            await dbContext.SaveChangesAsync();

            var fakeUpdateRequest = new Faker<UpdateWarehouseRequest>()
                .CustomInstantiator(f => new UpdateWarehouseRequest
                (
                    WarehouseId : warehouse.WarehouseId,
                    Name : f.Company.CompanyName(),
                    LogoWarehouseId : f.Random.Int(),
                    IsActive : f.Random.Bool()
                ));

            var request = fakeUpdateRequest.Generate();

            var result = await repo.UpdateWarehouse(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.WarehouseId);

            var updatedWarehouse = dbContext.Warehouses.FirstOrDefault(p => p.WarehouseId == result.WarehouseId);
            Assert.NotNull(updatedWarehouse);
            Assert.Equal(request.WarehouseId, updatedWarehouse.WarehouseId);
            Assert.Equal(request.Name, updatedWarehouse.Name);
            Assert.Equal(request.LogoWarehouseId, updatedWarehouse.LogoWarehouseId);
            Assert.Equal(request.IsActive, updatedWarehouse.IsActive);
        }
    }

    [Fact]
    public async Task UpdateWarehouse_ThrowsConflictException_WhenWarehouseNameExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse1 = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = "Warehouse 1",
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = true
                });

            var fakeWarehouse2 = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = "Warehouse 2",
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = true
                });

            var warehouse1 = fakeWarehouse1.Generate();
            var warehouse2 = fakeWarehouse2.Generate();

            dbContext.Warehouses.AddRange(warehouse1, warehouse2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateRequest = new Faker<UpdateWarehouseRequest>()
                .CustomInstantiator(f => new UpdateWarehouseRequest
                (
                    WarehouseId : warehouse2.WarehouseId,
                    Name : "Warehouse 2", // Same name as warehouse1
                    LogoWarehouseId : f.Random.Int(),
                    IsActive : f.Random.Bool()
                ));

            var request = fakeUpdateRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateWarehouse(request));
        }
    }
        
    [Fact]
    public async Task DeleteWarehouse_ReturnsDeletedWarehouseId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeWarehouse = new Faker<Warehouse>()
                .CustomInstantiator(f => new Warehouse
                {
                    WarehouseId = Guid.NewGuid(),
                    Name = f.Company.CompanyName(),
                    LogoWarehouseId = f.Random.Int(),
                    IsActive = true
                });

            var warehouse = fakeWarehouse.Generate();

            dbContext.Warehouses.Add(warehouse);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteWarehouse(warehouse.WarehouseId);

            Assert.Equal(warehouse.WarehouseId, result);
        }
    }
        
    [Fact]
    public async Task DeleteWarehouse_ThrowsNotFoundException_WhenWarehouseDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<WarehouseRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteWarehouse(Guid.NewGuid()));
        }
    }
}