using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.Price.Request;

namespace DataAccessLayer.UnitTests;

public class PriceRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public PriceRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering PriceRepository and IMapper
        services.AddScoped<PriceRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        _serviceProvider = services.BuildServiceProvider();
    }

    #region CreatePrice

    [Fact]
    public async Task CreatePrice_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<PriceRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakePriceRequest = new Faker<CreatePriceRequest>()
                .CustomInstantiator(f => new CreatePriceRequest
                (
                    Barcode: f.Commerce.Ean13(),
                    LogoPrice: Convert.ToDecimal(f.Commerce.Price())
                ));

            var request = fakePriceRequest.Generate();

            var result = await repo.CreatePrice(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.PriceId);

            var price = dbContext.Prices.FirstOrDefault(p => p.PriceId == result.PriceId);
            Assert.NotNull(price);
            Assert.Equal(request.Barcode, price.Barcode);
            Assert.Equal(request.LogoPrice, price.LogoPrice);
        }
    }

    #endregion

    #region UpdatePrice

    [Fact]
    public async Task UpdatePrice_ReturnsUpdatedPrice()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<PriceRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakePrice = new Faker<Price>()
                .CustomInstantiator(f => new Price
                {
                    PriceId = Guid.NewGuid(),
                    Barcode = f.Commerce.Ean13(),
                    LogoPrice = Convert.ToDecimal(f.Commerce.Price())
                });

            var price = fakePrice.Generate();

            dbContext.Prices.Add(price);
            await dbContext.SaveChangesAsync();

            var fakeUpdatePriceRequest = new Faker<UpdatePriceRequest>()
                .CustomInstantiator(f => new UpdatePriceRequest
                (
                    PriceId: price.PriceId,
                    LogoPrice: Convert.ToDecimal(f.Commerce.Price())
                ));

            var request = fakeUpdatePriceRequest.Generate();

            var result = await repo.UpdatePrice(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.PriceId);

            var updatedPrice = dbContext.Prices.FirstOrDefault(p => p.PriceId == result.PriceId);
            Assert.NotNull(updatedPrice);
            Assert.Equal(request.PriceId, updatedPrice.PriceId);
            Assert.Equal(request.LogoPrice, updatedPrice.LogoPrice);
        }
    }

    #endregion

    #region DeletePrice

    [Fact]
    public async Task DeletePrice_ReturnsDeletedPriceId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<PriceRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakePrice = new Faker<Price>()
                .CustomInstantiator(f => new Price
                {
                    PriceId = Guid.NewGuid(),
                    Barcode = f.Commerce.Ean13(),
                    LogoPrice = Convert.ToDecimal(f.Commerce.Price())
                });

            var price = fakePrice.Generate();

            dbContext.Prices.Add(price);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeletePrice(price.PriceId);

            Assert.Equal(price.PriceId, result);
        }
    }

    [Fact]
    public async Task DeletePrice_ThrowsNotFoundException_WhenPriceDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<PriceRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeletePrice(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetPrice

    [Fact]
    public async Task GetPrice_ReturnsPrice()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<PriceRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakePrice = new Faker<Price>()
                .CustomInstantiator(f => new Price
                {
                    PriceId = Guid.NewGuid(),
                    Barcode = f.Commerce.Ean13(),
                    LogoPrice = Convert.ToDecimal(f.Commerce.Price())
                });

            var price = fakePrice.Generate();

            dbContext.Prices.Add(price);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetPrice(price.PriceId);

            Assert.NotNull(result);
            Assert.Equal(price.PriceId, result.PriceId);
            Assert.Equal(price.Barcode, result.Barcode);
            Assert.Equal(price.LogoPrice, result.LogoPrice);
        }
    }

    [Fact]
    public async Task GetPrice_ThrowsNotFoundException_WhenPriceDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<PriceRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetPrice(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetPrices

    [Fact]
    public async Task GetPrices_ReturnsPrices()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<PriceRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakePrice1 = new Faker<Price>()
                .CustomInstantiator(f => new Price
                {
                    PriceId = Guid.NewGuid(),
                    Barcode = f.Commerce.Ean13(),
                    LogoPrice = Convert.ToDecimal(f.Commerce.Price())
                });

            var fakePrice2 = new Faker<Price>()
                .CustomInstantiator(f => new Price
                {
                    PriceId = Guid.NewGuid(),
                    Barcode = f.Commerce.Ean13(),
                    LogoPrice = Convert.ToDecimal(f.Commerce.Price())
                });

            var price1 = fakePrice1.Generate();
            var price2 = fakePrice2.Generate();

            dbContext.Prices.RemoveRange(dbContext.Prices);
            dbContext.Prices.AddRange(price1, price2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetPrices();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.PriceId == price1.PriceId);
            Assert.Contains(result, r => r.PriceId == price2.PriceId);
        }
    }

    #endregion
}