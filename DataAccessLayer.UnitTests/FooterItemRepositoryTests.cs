using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.FooterItem.Request;

namespace DataAccessLayer.UnitTests;

public class FooterItemRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public FooterItemRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering FooterItemRepository and IMapper
        services.AddScoped<FooterItemRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        _serviceProvider = services.BuildServiceProvider();
    }

    #region CreateFooterItem

    [Fact]
    public async Task CreateFooterItem_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFooterItemRequest = new Faker<CreateFooterItemRequest>()
                .CustomInstantiator(f => new CreateFooterItemRequest
                (
                    Label: f.Lorem.Word(),
                    Column: f.Random.Int(1, 3),
                    OrderNumber: f.Random.Int(1, 100),
                    TargetUrl: f.Internet.Url(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeFooterItemRequest.Generate();

            var result = await repo.CreateFooterItem(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.FooterItemId);

            var footerItem = dbContext.FooterItems.FirstOrDefault(p => p.FooterItemId == result.FooterItemId);
            Assert.NotNull(footerItem);
            Assert.Equal(request.Label.Trim(), footerItem.Label);
            Assert.Equal(request.Column, footerItem.Column);
            Assert.Equal(request.OrderNumber, footerItem.OrderNumber);
            Assert.Equal(request.TargetUrl.Trim(), footerItem.TargetUrl);
            Assert.Equal(request.IsActive, footerItem.IsActive);
        }
    }

    [Fact]
    public async Task CreateFooterItem_ThrowsConflictException_WhenLabelExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFooterItem = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = "Existing Label",
                    Column = f.Random.Int(1, 3),
                    OrderNumber = f.Random.Int(1, 100),
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var footerItem = fakeFooterItem.Generate();

            dbContext.FooterItems.Add(footerItem);
            await dbContext.SaveChangesAsync();

            var fakeFooterItemRequest = new Faker<CreateFooterItemRequest>()
                .CustomInstantiator(f => new CreateFooterItemRequest
                (
                    Label: "Existing Label", // Same label
                    Column: f.Random.Int(1, 3),
                    OrderNumber: f.Random.Int(1, 100),
                    TargetUrl: f.Internet.Url(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeFooterItemRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateFooterItem(request));
        }
    }

    [Fact]
    public async Task CreateFooterItem_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFooterItem = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = f.Lorem.Text(),
                    Column = f.Random.Int(1, 3),
                    OrderNumber = 1,
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var footerItem = fakeFooterItem.Generate();

            dbContext.FooterItems.Add(footerItem);
            await dbContext.SaveChangesAsync();

            var fakeFooterItemRequest = new Faker<CreateFooterItemRequest>()
                .CustomInstantiator(f => new CreateFooterItemRequest
                (
                    Label: f.Lorem.Text(), // Same label
                    Column: f.Random.Int(1, 3),
                    OrderNumber: 1,
                    TargetUrl: f.Internet.Url(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeFooterItemRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateFooterItem(request));
        }
    }

    #endregion

    #region UpdateFooterItem

    [Fact]
    public async Task UpdateFooterItem_ReturnsUpdatedFooterItem()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFooterItem = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = f.Lorem.Word(),
                    Column = f.Random.Int(1, 3),
                    OrderNumber = f.Random.Int(1, 100),
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var footerItem = fakeFooterItem.Generate();

            dbContext.FooterItems.Add(footerItem);
            await dbContext.SaveChangesAsync();

            var fakeUpdateFooterItemRequest = new Faker<UpdateFooterItemRequest>()
                .CustomInstantiator(f => new UpdateFooterItemRequest
                (
                    FooterItemId: footerItem.FooterItemId,
                    Label: f.Lorem.Word(),
                    Column: f.Random.Int(1, 3),
                    OrderNumber: f.Random.Int(1, 100),
                    TargetUrl: f.Internet.Url(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateFooterItemRequest.Generate();

            var result = await repo.UpdateFooterItem(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.FooterItemId);

            var updatedFooterItem = dbContext.FooterItems.FirstOrDefault(p => p.FooterItemId == result.FooterItemId);
            Assert.NotNull(updatedFooterItem);
            Assert.Equal(request.FooterItemId, updatedFooterItem.FooterItemId);
            Assert.Equal(request.Label.Trim(), updatedFooterItem.Label);
            Assert.Equal(request.Column, updatedFooterItem.Column);
            Assert.Equal(request.OrderNumber, updatedFooterItem.OrderNumber);
            Assert.Equal(request.TargetUrl.Trim(), updatedFooterItem.TargetUrl);
            Assert.Equal(request.IsActive, updatedFooterItem.IsActive);
        }
    }

    [Fact]
    public async Task UpdateFooterItem_ThrowsConflictException_WhenOrderNumberExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFooterItem1 = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = f.Lorem.Word(),
                    Column = f.Random.Int(1, 3),
                    OrderNumber = 1, // Specific order number
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var fakeFooterItem2 = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = f.Lorem.Word(),
                    Column = f.Random.Int(1, 3),
                    OrderNumber = 2,
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var footerItem1 = fakeFooterItem1.Generate();
            var footerItem2 = fakeFooterItem2.Generate();

            dbContext.FooterItems.AddRange(footerItem1, footerItem2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateFooterItemRequest = new Faker<UpdateFooterItemRequest>()
                .CustomInstantiator(f => new UpdateFooterItemRequest
                (
                    FooterItemId: footerItem2.FooterItemId,
                    Label: f.Lorem.Word(),
                    Column: f.Random.Int(1, 3),
                    OrderNumber: 1, // Same order number as footerItem1
                    TargetUrl: f.Internet.Url(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateFooterItemRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateFooterItem(request));
        }
    }

    [Fact]
    public async Task UpdateFooterItem_ThrowsConflictException_WhenLabelExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFooterItem1 = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = "Footer Item 1",
                    Column = f.Random.Int(1, 3),
                    OrderNumber = f.Random.Int(1, 1000), // Specific order number
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var fakeFooterItem2 = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = "Footer Item 1",
                    Column = f.Random.Int(1, 3),
                    OrderNumber = f.Random.Int(1, 1000),
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var footerItem1 = fakeFooterItem1.Generate();
            var footerItem2 = fakeFooterItem2.Generate();

            dbContext.FooterItems.AddRange(footerItem1, footerItem2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateFooterItemRequest = new Faker<UpdateFooterItemRequest>()
                .CustomInstantiator(f => new UpdateFooterItemRequest
                (
                    FooterItemId: footerItem2.FooterItemId,
                    Label: f.Lorem.Word(),
                    Column: f.Random.Int(1, 3),
                    OrderNumber: 1, // Same order number as footerItem1
                    TargetUrl: f.Internet.Url(),
                    IsActive: f.Random.Bool()
                ));

            var request = fakeUpdateFooterItemRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateFooterItem(request));
        }
    }

    #endregion

    #region DeleteFooterItem

    [Fact]
    public async Task DeleteFooterItem_ReturnsDeletedFooterItemId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFooterItem = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = f.Lorem.Word(),
                    Column = f.Random.Int(1, 3),
                    OrderNumber = f.Random.Int(1, 100),
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var footerItem = fakeFooterItem.Generate();

            dbContext.FooterItems.Add(footerItem);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteFooterItem(footerItem.FooterItemId);

            Assert.Equal(footerItem.FooterItemId, result);
        }
    }

    [Fact]
    public async Task DeleteFooterItem_ThrowsNotFoundException_WhenFooterItemDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteFooterItem(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetFooterItem

    [Fact]
    public async Task GetFooterItem_ReturnsFooterItem()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFooterItem = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = f.Lorem.Word(),
                    Column = f.Random.Int(1, 3),
                    OrderNumber = f.Random.Int(1, 100),
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var footerItem = fakeFooterItem.Generate();

            dbContext.FooterItems.Add(footerItem);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetFooterItem(footerItem.FooterItemId);

            Assert.NotNull(result);
            Assert.Equal(footerItem.FooterItemId, result.FooterItemId);
            Assert.Equal(footerItem.Label, result.Label);
            Assert.Equal(footerItem.Column, result.Column);
            Assert.Equal(footerItem.OrderNumber, result.OrderNumber);
            Assert.Equal(footerItem.TargetUrl, result.TargetUrl);
            Assert.Equal(footerItem.IsActive, result.IsActive);
        }
    }

    [Fact]
    public async Task GetFooterItem_ThrowsNotFoundException_WhenFooterItemDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetFooterItem(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetFooterItems

    [Fact]
    public async Task GetFooterItems_ReturnsFooterItems()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<FooterItemRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeFooterItem1 = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = f.Lorem.Word(),
                    Column = f.Random.Int(1, 3),
                    OrderNumber = f.Random.Int(1, 100),
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var fakeFooterItem2 = new Faker<FooterItem>()
                .CustomInstantiator(f => new FooterItem
                {
                    FooterItemId = Guid.NewGuid(),
                    Label = f.Lorem.Word(),
                    Column = f.Random.Int(1, 3),
                    OrderNumber = f.Random.Int(1, 100),
                    TargetUrl = f.Internet.Url(),
                    IsActive = f.Random.Bool()
                });

            var footerItem1 = fakeFooterItem1.Generate();
            var footerItem2 = fakeFooterItem2.Generate();

            dbContext.FooterItems.AddRange(footerItem1, footerItem2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetFooterItems();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.FooterItemId == footerItem1.FooterItemId);
            Assert.Contains(result, r => r.FooterItemId == footerItem2.FooterItemId);
        }
    }

    // [Fact]
    // public async Task GetActiveFooterItems_ReturnsFooterItems()
    // {
    //     using (var scope = _serviceProvider.CreateScope())
    //     {
    //         var scopedService = scope.ServiceProvider;
    //         var repo = scopedService.GetRequiredService<FooterItemRepository>();
    //         var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();
    //
    //         var fakeFooterItem1 = new Faker<FooterItem>()
    //             .CustomInstantiator(f => new FooterItem
    //             {
    //                 FooterItemId = Guid.NewGuid(),
    //                 Label = f.Lorem.Word(),
    //                 Column = f.Random.Int(1, 3),
    //                 OrderNumber = f.Random.Int(1, 100),
    //                 TargetUrl = f.Internet.Url(),
    //                 IsActive = true
    //             });
    //
    //         var fakeFooterItem2 = new Faker<FooterItem>()
    //             .CustomInstantiator(f => new FooterItem
    //             {
    //                 FooterItemId = Guid.NewGuid(),
    //                 Label = f.Lorem.Word(),
    //                 Column = f.Random.Int(1, 3),
    //                 OrderNumber = f.Random.Int(1, 100),
    //                 TargetUrl = f.Internet.Url(),
    //                 IsActive = false
    //             });
    //
    //         var footerItem1 = fakeFooterItem1.Generate();
    //         var footerItem2 = fakeFooterItem2.Generate();
    //
    //         dbContext.FooterItems.AddRange(footerItem1, footerItem2);
    //         await dbContext.SaveChangesAsync();
    //
    //         var result = await repo.GetFooterItems(p => p.IsActive);
    //
    //         Assert.NotNull(result);
    //         Assert.Single(result);
    //         Assert.Contains(result, r => r.FooterItemId == footerItem1.FooterItemId);
    //         foreach (var item in result)
    //         {
    //             Assert.True(item.IsActive);
    //         }
    //     }
    // }

    #endregion
}