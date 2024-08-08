using AutoMapper;
using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.MenuItem.Request;

namespace DataAccessLayer.UnitTests;

public class MenuItemRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public MenuItemRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering MenuItemRepository and IMapper
        services.AddScoped<MenuItemRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        _serviceProvider = services.BuildServiceProvider();
    }

    #region CreateMenuItem

    [Fact]
    public async Task CreateMenuItem_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeMenuItemRequest = new Faker<CreateMenuItemRequest>()
                .CustomInstantiator(f => new CreateMenuItemRequest
                (
                    Label: f.Commerce.ProductName(),
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: f.Random.Int(1, 100),
                    OnlyToMembers: f.Random.Bool(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeMenuItemRequest.Generate();

            var result = await repo.CreateMenuItemAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.MenuItemId);

            var menuItem = dbContext.MenuItems.FirstOrDefault(p => p.MenuItemId == result.MenuItemId);
            Assert.NotNull(menuItem);
            Assert.Equal(request.Label, menuItem.Label);
            Assert.Equal(request.TargetUrl, menuItem.TargetUrl);
            Assert.Equal(request.OrderNumber, menuItem.OrderNumber);
            Assert.Equal(request.OnlyToMembers, menuItem.OnlyToMembers);
            Assert.Equal(request.IsActive, menuItem.IsActive);
        }
    }

    [Fact]
    public async Task CreateMenuItem_ThrowsConflictException_WhenLabelExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeMenuItem = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = "Existing Label",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var menuItem = fakeMenuItem.Generate();

            dbContext.MenuItems.Add(menuItem);
            await dbContext.SaveChangesAsync();

            var fakeMenuItemRequest = new Faker<CreateMenuItemRequest>()
                .CustomInstantiator(f => new CreateMenuItemRequest
                (
                    Label: "Existing Label", // Same label
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: f.Random.Int(1, 100),
                    OnlyToMembers: f.Random.Bool(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeMenuItemRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateMenuItemAsync(request));
        }
    }

    [Fact]
    public async Task CreateMenuItem_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeMenuItem = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = 1, // Specific order number
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var menuItem = fakeMenuItem.Generate();

            dbContext.MenuItems.Add(menuItem);
            await dbContext.SaveChangesAsync();

            var fakeMenuItemRequest = new Faker<CreateMenuItemRequest>()
                .CustomInstantiator(f => new CreateMenuItemRequest
                (
                    Label: f.Commerce.ProductName(),
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: 1, // Same order number
                    OnlyToMembers: f.Random.Bool(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeMenuItemRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateMenuItemAsync(request));
        }
    }

    #endregion

    #region UpdateMenuItem

    [Fact]
    public async Task UpdateMenuItem_ReturnsUpdatedMenuItem()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeMenuItem = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var menuItem = fakeMenuItem.Generate();

            dbContext.MenuItems.Add(menuItem);
            await dbContext.SaveChangesAsync();

            var fakeUpdateMenuItemRequest = new Faker<UpdateMenuItemRequest>()
                .CustomInstantiator(f => new UpdateMenuItemRequest
                (
                    MenuItemId: menuItem.MenuItemId,
                    Label: f.Commerce.ProductName(),
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: f.Random.Int(1, 100),
                    OnlyToMembers: f.Random.Bool(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateMenuItemRequest.Generate();

            var result = await repo.UpdateMenuItemAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.MenuItemId);

            var updatedMenuItem = dbContext.MenuItems.FirstOrDefault(p => p.MenuItemId == result.MenuItemId);
            Assert.NotNull(updatedMenuItem);
            Assert.Equal(request.MenuItemId, result.MenuItemId);
            Assert.Equal(request.Label, updatedMenuItem.Label);
            Assert.Equal(request.TargetUrl, updatedMenuItem.TargetUrl);
            Assert.Equal(request.OrderNumber, updatedMenuItem.OrderNumber);
            Assert.Equal(request.OnlyToMembers, updatedMenuItem.OnlyToMembers);
            Assert.Equal(request.IsActive, updatedMenuItem.IsActive);
        }
    }

    [Fact]
    public async Task UpdateMenuItem_ThrowsConflictException_WhenLabelExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeMenuItem1 = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = "Existing Label",
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var fakeMenuItem2 = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var menuItem1 = fakeMenuItem1.Generate();
            var menuItem2 = fakeMenuItem2.Generate();

            dbContext.MenuItems.AddRange(menuItem1, menuItem2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateMenuItemRequest = new Faker<UpdateMenuItemRequest>()
                .CustomInstantiator(f => new UpdateMenuItemRequest
                (
                    MenuItemId: menuItem2.MenuItemId,
                    Label: "Existing Label", // Same label as menuItem1
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: f.Random.Int(1, 100),
                    OnlyToMembers: f.Random.Bool(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateMenuItemRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateMenuItemAsync(request));
        }
    }

    [Fact]
    public async Task UpdateMenuItem_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeMenuItem1 = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = 1, // Specific order number
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var fakeMenuItem2 = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = 2,
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var menuItem1 = fakeMenuItem1.Generate();
            var menuItem2 = fakeMenuItem2.Generate();

            dbContext.MenuItems.AddRange(menuItem1, menuItem2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateMenuItemRequest = new Faker<UpdateMenuItemRequest>()
                .CustomInstantiator(f => new UpdateMenuItemRequest
                (
                    MenuItemId: menuItem2.MenuItemId,
                    Label: f.Commerce.ProductName(),
                    TargetUrl: f.Internet.Url(),
                    OrderNumber: 1, // Same order number as menuItem1
                    OnlyToMembers: f.Random.Bool(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateMenuItemRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateMenuItemAsync(request));
        }
    }

    #endregion

    #region DeleteMenuItem

    [Fact]
    public async Task DeleteMenuItem_ReturnsDeletedMenuItemId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeMenuItem = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var menuItem = fakeMenuItem.Generate();

            dbContext.MenuItems.Add(menuItem);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteMenuItemAsync(menuItem.MenuItemId);

            Assert.Equal(menuItem.MenuItemId, result);
        }
    }

    [Fact]
    public async Task DeleteMenuItem_ThrowsNotFoundException_WhenMenuItemDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteMenuItemAsync(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetMenuItem

    [Fact]
    public async Task GetMenuItem_ReturnsMenuItem()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeMenuItem = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var menuItem = fakeMenuItem.Generate();

            dbContext.MenuItems.Add(menuItem);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetMenuItemAsync(menuItem.MenuItemId);

            Assert.NotNull(result);
            Assert.Equal(menuItem.MenuItemId, result.MenuItemId);
            Assert.Equal(menuItem.Label, result.Label);
            Assert.Equal(menuItem.TargetUrl, result.TargetUrl);
            Assert.Equal(menuItem.OrderNumber, result.OrderNumber);
            Assert.Equal(menuItem.OnlyToMembers, result.OnlyToMembers);
            Assert.Equal(menuItem.IsActive, result.IsActive);
        }
    }
    
    [Fact]
    public async Task GetMenuItem_ThrowsNotFoundException_WhenMenuItemDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetMenuItemAsync(Guid.NewGuid()));
        }
    }
    
    #endregion

    #region GetMenuItems

    [Fact]
    public async Task GetMenuItems_ReturnsMenuItems()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeMenuItem1 = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var fakeMenuItem2 = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = f.Random.Bool()
                });

            var menuItem1 = fakeMenuItem1.Generate();
            var menuItem2 = fakeMenuItem2.Generate();

            dbContext.MenuItems.RemoveRange(dbContext.MenuItems);
            dbContext.MenuItems.AddRange(menuItem1, menuItem2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetMenuItemsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.MenuItemId == menuItem1.MenuItemId);
            Assert.Contains(result, r => r.MenuItemId == menuItem2.MenuItemId);
        }
    }

    [Fact]
    public async Task GetActiveMenuItems_ReturnsMenuItems()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<MenuItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeMenuItem1 = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = true
                });

            var fakeMenuItem2 = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    MenuItemId = Guid.NewGuid(),
                    Label = f.Commerce.ProductName(),
                    TargetUrl = f.Internet.Url(),
                    OrderNumber = f.Random.Int(1, 100),
                    OnlyToMembers = f.Random.Bool(),
                    IsActive = false
                });

            var menuItem1 = fakeMenuItem1.Generate();
            var menuItem2 = fakeMenuItem2.Generate();

            dbContext.MenuItems.RemoveRange(dbContext.MenuItems);
            dbContext.MenuItems.AddRange(menuItem1, menuItem2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetMenuItemsAsync(p => p.IsActive);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, r => r.MenuItemId == menuItem1.MenuItemId);
            foreach (var item in result)
            {
                Assert.True(item.IsActive);
            }
        }
    }
    #endregion

}