using Bogus;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.Address.Request;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;

namespace DataAccessLayer.UnitTests;

public class AddressRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public AddressRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering AddressRepository and IMapper
        services.AddScoped<AddressRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        _serviceProvider = services.BuildServiceProvider();
    }

    #region CreateAddress

    [Fact]
    public async Task CreateAddress_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password()
                });

            var user = fakeUser.Generate();

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var fakeAddressRequest = new Faker<CreateAddressRequest>()
                .CustomInstantiator(f => new CreateAddressRequest
                (
                    UserId: user.UserId,
                    Type: 0,
                    Title: f.Address.StreetName(),
                    City: f.Address.City(),
                    District: f.Address.County(),
                    Neighborhood: f.Address.StreetAddress(),
                    AddressDetail: f.Address.FullAddress(),
                    TaxNo: null,
                    TaxOffice: null,
                    CompanyName: null
                ));

            var request = fakeAddressRequest.Generate();

            var result = await repo.CreateAddressAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.AddressId);

            var address = dbContext.Addresses.FirstOrDefault(p => p.AddressId == result.AddressId);
            Assert.Equal(request.Title, address.Title);
            Assert.Equal(request.City, address.City);
            Assert.Equal(request.District, address.District);
            Assert.Equal(request.Neighborhood, address.Neighborhood);
            Assert.Equal(request.AddressDetail, address.AddressDetail);
            Assert.Equal(request.UserId, address.UserId);
        }
    }

    [Fact]
    public async Task CreateAddress_ThrowsConflictException_WhenTitleExistsForUser()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password()
                });

            var user = fakeUser.Generate();

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var fakeAddress = new Faker<Address>()
                .CustomInstantiator(f => new Address
                {
                    AddressId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Type = 0,
                    Title = "Existing Title",
                    City = f.Address.City(),
                    District = f.Address.County(),
                    Neighborhood = f.Address.StreetAddress(),
                    AddressDetail = f.Address.FullAddress()
                });

            var address = fakeAddress.Generate();

            dbContext.Addresses.Add(address);
            await dbContext.SaveChangesAsync();

            var fakeAddressRequest = new Faker<CreateAddressRequest>()
                .CustomInstantiator(f => new CreateAddressRequest
                (
                    UserId: user.UserId,
                    Type: 0,
                    Title: "Existing Title", // Same title
                    City: f.Address.City(),
                    District: f.Address.County(),
                    Neighborhood: f.Address.StreetAddress(),
                    AddressDetail: f.Address.FullAddress(),
                    TaxNo: null,
                    TaxOffice: null,
                    CompanyName: null
                ));

            var request = fakeAddressRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.CreateAddressAsync(request));
        }
    }

    [Fact]
    public async Task CreateAddress_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();

            var fakeAddressRequest = new Faker<CreateAddressRequest>()
                .CustomInstantiator(f => new CreateAddressRequest
                (
                    UserId: Guid.NewGuid(), // Non-existent user
                    Type: 0,
                    Title: f.Address.StreetName(),
                    City: f.Address.City(),
                    District: f.Address.County(),
                    Neighborhood: f.Address.StreetAddress(),
                    AddressDetail: f.Address.FullAddress(),
                    TaxNo: null,
                    TaxOffice: null,
                    CompanyName: null
                ));

            var request = fakeAddressRequest.Generate();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.CreateAddressAsync(request));
        }
    }

    #endregion

    #region UpdateAddress

    [Fact]
    public async Task UpdateAddress_ReturnsUpdatedAddress()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password()
                });

            var user = fakeUser.Generate();

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var fakeAddress = new Faker<Address>()
                .CustomInstantiator(f => new Address
                {
                    AddressId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Type = 0,
                    Title = f.Address.StreetName(),
                    City = f.Address.City(),
                    District = f.Address.County(),
                    Neighborhood = f.Address.StreetAddress(),
                    AddressDetail = f.Address.FullAddress()
                });

            var address = fakeAddress.Generate();

            dbContext.Addresses.Add(address);
            await dbContext.SaveChangesAsync();

            var fakeUpdateAddressRequest = new Faker<UpdateAddressRequest>()
                .CustomInstantiator(f => new UpdateAddressRequest
                (
                    AddressId: address.AddressId,
                    UserId: user.UserId,
                    Type: 1,
                    Title: f.Address.StreetName(),
                    City: f.Address.City(),
                    District: f.Address.County(),
                    Neighborhood: f.Address.StreetAddress(),
                    AddressDetail: f.Address.FullAddress(),
                    TaxOffice: null,
                    TaxNo: null,
                    CompanyName: null
                ));

            var request = fakeUpdateAddressRequest.Generate();

            var result = await repo.UpdateAddressAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.AddressId);

            var updatedAddress = dbContext.Addresses.FirstOrDefault(p => p.AddressId == result.AddressId);
            Assert.NotNull(updatedAddress);
            Assert.Equal(request.Title, updatedAddress.Title);
            Assert.Equal(request.City, updatedAddress.City);
            Assert.Equal(request.District, updatedAddress.District);
            Assert.Equal(request.Neighborhood, updatedAddress.Neighborhood);
            Assert.Equal(request.AddressDetail, updatedAddress.AddressDetail);
            Assert.Equal(request.UserId, updatedAddress.UserId);
        }
    }

    [Fact]
    public async Task UpdateAddress_ThrowsConflictException_WhenTitleExistsForUser()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password()
                });

            var user = fakeUser.Generate();

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var fakeAddress1 = new Faker<Address>()
                .CustomInstantiator(f => new Address
                {
                    AddressId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Type = 0,
                    Title = "Existing Title",
                    City = f.Address.City(),
                    District = f.Address.County(),
                    Neighborhood = f.Address.StreetAddress(),
                    AddressDetail = f.Address.FullAddress()
                });

            var fakeAddress2 = new Faker<Address>()
                .CustomInstantiator(f => new Address
                {
                    AddressId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Type = 1,
                    Title = f.Address.StreetName(),
                    City = f.Address.City(),
                    District = f.Address.County(),
                    Neighborhood = f.Address.StreetAddress(),
                    AddressDetail = f.Address.FullAddress()
                });

            var address1 = fakeAddress1.Generate();
            var address2 = fakeAddress2.Generate();

            dbContext.Addresses.AddRange(address1, address2);
            await dbContext.SaveChangesAsync();

            var fakeUpdateAddressRequest = new Faker<UpdateAddressRequest>()
                .CustomInstantiator(f => new UpdateAddressRequest
                (
                    AddressId: address2.AddressId,
                    UserId: user.UserId,
                    Type: 1,
                    Title: "Existing Title", // Same title as address1
                    City: f.Address.City(),
                    District: f.Address.County(),
                    Neighborhood: f.Address.StreetAddress(),
                    AddressDetail: f.Address.FullAddress(),
                    TaxOffice: null,
                    TaxNo: null,
                    CompanyName: null
                ));

            var request = fakeUpdateAddressRequest.Generate();

            await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateAddressAsync(request));
        }
    }

    [Fact]
    public async Task UpdateAddress_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();

            var fakeUpdateAddressRequest = new Faker<UpdateAddressRequest>()
                .CustomInstantiator(f => new UpdateAddressRequest
                (
                    AddressId: Guid.NewGuid(), // Non-existent address
                    UserId: Guid.NewGuid(), // Non-existent user
                    Type: 0,
                    Title: f.Address.StreetName(),
                    City: f.Address.City(),
                    District: f.Address.County(),
                    Neighborhood: f.Address.StreetAddress(),
                    AddressDetail: f.Address.FullAddress(),
                    TaxOffice: null,
                    TaxNo: null,
                    CompanyName: null
                ));

            var request = fakeUpdateAddressRequest.Generate();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.UpdateAddressAsync(request));
        }
    }

    #endregion

    #region DeleteAddress

    [Fact]
    public async Task DeleteAddress_ReturnsDeletedAddress()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeAddress = new Faker<Address>()
                .CustomInstantiator(f => new Address
                {
                    AddressId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Type = 0,
                    Title = f.Address.StreetName(),
                    City = f.Address.City(),
                    District = f.Address.County(),
                    Neighborhood = f.Address.StreetAddress(),
                    AddressDetail = f.Address.FullAddress()
                });

            var address = fakeAddress.Generate();

            dbContext.Addresses.Add(address);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteAddressAsync(address.AddressId);

            Assert.NotNull(result);
            Assert.Equal(address.AddressId, result.AddressId);
        }
    }

    [Fact]
    public async Task DeleteAddress_ThrowsNotFoundException_WhenAddressDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetAddressAsync(Guid.NewGuid()));
        }
    }

    #endregion
        
    #region GetAddress

    [Fact]
    public async Task GetAddress_ReturnsAddress()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password()
                });

            var fakeAddress = new Faker<Address>()
                .CustomInstantiator(f => new Address
                {
                    AddressId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Type = 0,
                    Title = f.Address.StreetName(),
                    City = f.Address.City(),
                    District = f.Address.County(),
                    Neighborhood = f.Address.StreetAddress(),
                    AddressDetail = f.Address.FullAddress(),
                    TaxOffice = null,
                    TaxNo = null,
                    CompanyName = null
                });

            var user = fakeUser.Generate();
            var address = fakeAddress.Generate();

            address.UserId = user.UserId;

            dbContext.Users.Add(user);
            dbContext.Addresses.Add(address);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetAddressAsync(address.AddressId);

            Assert.NotNull(result);
            Assert.Equal(address.AddressId, result.AddressId);
            Assert.Equal(address.Title, result.Title);
            Assert.Equal(address.City, result.City);
            Assert.Equal(address.District, result.District);
            Assert.Equal(address.Neighborhood, result.Neighborhood);
            Assert.Equal(address.AddressDetail, result.AddressDetail);
            Assert.Equal(address.UserId, result.User.UserId);
        }
    }

    [Fact]
    public async Task GetAddress_ThrowsNotFoundException_WhenAddressDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetAddressAsync(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetAddresses

    [Fact]
    public async Task GetAddresses_ReturnsAddresses()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<AddressRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password()
                });

            var fakeAddress1 = new Faker<Address>()
                .CustomInstantiator(f => new Address
                {
                    AddressId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Type = 0,
                    Title = f.Address.StreetName(),
                    City = f.Address.City(),
                    District = f.Address.County(),
                    Neighborhood = f.Address.StreetAddress(),
                    AddressDetail = f.Address.FullAddress()
                });

            var fakeAddress2 = new Faker<Address>()
                .CustomInstantiator(f => new Address
                {
                    AddressId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Type = 1,
                    Title = f.Address.StreetName(),
                    City = f.Address.City(),
                    District = f.Address.County(),
                    Neighborhood = f.Address.StreetAddress(),
                    AddressDetail = f.Address.FullAddress()
                });

            var user1 = fakeUser.Generate();
            var user2 = fakeUser.Generate();
            var address1 = fakeAddress1.Generate();
            var address2 = fakeAddress2.Generate();

            address1.UserId = user1.UserId;
            address2.UserId = user2.UserId;

            dbContext.Addresses.RemoveRange(dbContext.Addresses);
            dbContext.Users.AddRange(user1, user2);
            dbContext.Addresses.AddRange(address1, address2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetAddressesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.AddressId == address1.AddressId);
            Assert.Contains(result, r => r.AddressId == address2.AddressId);
        }
    }

    #endregion
}