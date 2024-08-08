using Bogus;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.User.Request;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;

namespace DataAccessLayer.UnitTests;

public class UserRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public UserRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering UserRepository and IMapper
        services.AddScoped<UserRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        _serviceProvider = services.BuildServiceProvider();
    }

    #region CreateUser

    [Fact]
    public async Task CreateUser_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUserRequest = new Faker<CreateUserRequest>()
                .CustomInstantiator(f => new CreateUserRequest
                (
                    Email: f.Internet.Email(),
                    PasswordHash: f.Internet.Password()
                ));

            var request = fakeUserRequest.Generate();

            var result = await repo.CreateUser(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.UserId);

            var user = dbContext.Users.FirstOrDefault(p => p.UserId == result.UserId);
            Assert.NotNull(user);
            Assert.Equal(request.Email, user.Email);
            Assert.True(user.AccountStatus);
        }
    }

    #endregion

    #region UpdateUser

    [Fact]
    public async Task UpdateUser_ReturnsUpdatedUser()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password(),
                    AccountStatus = true
                });

            var user = fakeUser.Generate();

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var fakeUpdateUserRequest = new Faker<UpdateUserRequest>()
                .CustomInstantiator(f => new UpdateUserRequest
                (
                    UserId: user.UserId,
                    Name: f.Name.FirstName(),
                    Surname: f.Name.LastName(),
                    Email: f.Internet.Email(),
                    PhoneNumber: f.Phone.PhoneNumber(),
                    Address: f.Address.FullAddress(),
                    Tc: f.Random.String2(11, "0123456789"),
                    Gender: f.Lorem.Word(),
                    BirthDate: f.Date.Past(30)
                ));

            var request = fakeUpdateUserRequest.Generate();

            var result = await repo.UpdateUser(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.UserId);

            var updatedUser = dbContext.Users.FirstOrDefault(p => p.UserId == result.UserId);
            Assert.NotNull(updatedUser);
            Assert.Equal(request.UserId, updatedUser.UserId);
            Assert.Equal(request.Name, updatedUser.Name);
            Assert.Equal(request.Surname, updatedUser.Surname);
            Assert.Equal(request.Email, updatedUser.Email);
            Assert.Equal(request.PhoneNumber, updatedUser.PhoneNumber);
            Assert.Equal(request.Address, updatedUser.Address);
            Assert.Equal(request.Tc, updatedUser.TC);
            Assert.Equal(request.Gender, updatedUser.Gender);
            Assert.Equal(request.BirthDate, updatedUser.BirthDate);
        }
    }

    #endregion

    #region DeleteUser

    [Fact]
    public async Task DeleteUser_ReturnsDeletedUserId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password(),
                    AccountStatus = true
                });

            var user = fakeUser.Generate();

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteUser(user.UserId);

            Assert.Equal(user.UserId, result.UserId);
        }
    }

    [Fact]
    public async Task DeleteUserByEmail_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteUser(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetUserById

    [Fact]
    public async Task GetUserById_ReturnsUser()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password(),
                    AccountStatus = true
                });

            var user = fakeUser.Generate();

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetUserById(user.UserId);

            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Email, result.Email);
        }
    }

    [Fact]
    public async Task GetUserById_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetUserById(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetUserByEmail

    [Fact]
    public async Task GetUserByEmail_ReturnsUser()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password(),
                    AccountStatus = true
                });

            var user = fakeUser.Generate();

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetUserByEmail(user.Email);

            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Email, result.Email);
        }
    }

    [Fact]
    public async Task GetUserByEmail_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetUserByEmail("nonexistent@example.com"));
        }
    }

    #endregion

    #region GetUsers

    [Fact]
    public async Task GetUsers_ReturnsUsers()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser1 = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password(),
                    AccountStatus = true
                });

            var fakeUser2 = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password(),
                    AccountStatus = true
                });

            var user1 = fakeUser1.Generate();
            var user2 = fakeUser2.Generate();

            dbContext.Users.AddRange(user1, user2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetUsers();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.UserId == user1.UserId);
            Assert.Contains(result, r => r.UserId == user2.UserId);
        }
    }

    #endregion

    #region IsExist

    [Fact]
    public async Task IsExistAsync_ReturnsTrue_WhenUserExists()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password(),
                    AccountStatus = true
                });

            var user = fakeUser.Generate();

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var result = await repo.IsExistAsync(user.Email);

            Assert.True(result);
        }
    }

    #endregion
}