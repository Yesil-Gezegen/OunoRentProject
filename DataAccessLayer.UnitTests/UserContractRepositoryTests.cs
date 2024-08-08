using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.FAQ.Request;
using Shared.DTO.UserContracts.Request;

public class UserContractRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public UserContractRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        services.AddScoped<UserContractRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        _serviceProvider = services.BuildServiceProvider();
    }

    #region CreateUserContract

    [Fact]
    public async Task CreateUserContract_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserContractRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password()
                });

            var fakeContract = new Faker<Contract>()
                .CustomInstantiator(f => new Contract()
                {
                    ContractId = Guid.NewGuid(),
                    Body = f.Lorem.Paragraph(),
                    IsActive = f.Random.Bool(),
                    Name = f.Lorem.Text(),
                    Type = f.Random.Int(1, 2),
                    Version = f.Random.Double(),
                    PreviousVersion = f.Random.Double(),
                    RequiresAt = f.Lorem.Sentence(),
                    CreateDate = DateTime.Now
                });

            var user = fakeUser.Generate();
            var contract = fakeContract.Generate();

            // Adding necessary data to in-memory database
            dbContext.Users.Add(user);
            dbContext.Contracts.Add(contract);
            await dbContext.SaveChangesAsync();

            var request = new CreateUserContractRequest
            (
                UserId: user.UserId,
                ContractId: contract.ContractId,
                FileName: "TestFile.pdf"
            );

            var result = await repo.CreateUserContractAsync(request);
            var userContract =
                dbContext.UserContracts.FirstOrDefault(p => p.UserContractId == result.UserContractId);

            Assert.NotNull(result);
            Assert.NotNull(userContract);
            Assert.NotEqual(Guid.Empty, result.UserContractId);
            Assert.Equal(user.UserId, userContract.User.UserId);
            Assert.Equal(contract.ContractId, userContract.Contract.ContractId);
            Assert.Equal("TestFile.pdf", userContract.FileName);
            Assert.True(userContract.Date <= DateTime.UtcNow);
        }
    }

    [Fact]
    public async Task CreateUserContract_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserContractRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeContract = new Faker<Contract>()
                .CustomInstantiator(f => new Contract
                {
                    ContractId = Guid.NewGuid(),
                    Body = f.Lorem.Paragraph(),
                    IsActive = f.Random.Bool(),
                    Name = f.Lorem.Text(),
                    Type = f.Random.Int(1, 2),
                    Version = f.Random.Double(),
                    PreviousVersion = f.Random.Double(),
                    RequiresAt = f.Lorem.Sentence(),
                    CreateDate = DateTime.Now
                });

            var contract = fakeContract.Generate();

            // Adding necessary data to in-memory database
            dbContext.Contracts.Add(contract);
            await dbContext.SaveChangesAsync();

            var request = new CreateUserContractRequest
            (
                UserId: Guid.NewGuid(),
                ContractId: contract.ContractId,
                FileName: "TestFile.pdf"
            );

            await Assert.ThrowsAsync<NotFoundException>(() => repo.CreateUserContractAsync(request));
        }
    }

    [Fact]
    public async Task CreateUserContract_ThrowsNotFoundException_WhenContractDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserContractRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password()
                });

            var user = fakeUser.Generate();

            // Adding necessary data to in-memory database
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var request = new CreateUserContractRequest
            (
                UserId: user.UserId,
                ContractId: Guid.NewGuid(), // Non-existent contract
                FileName: "TestFile.pdf"
            );

            await Assert.ThrowsAsync<NotFoundException>(() => repo.CreateUserContractAsync(request));
        }
    }

    #endregion

    #region GetUserContract

    [Fact]
    public async Task GetUserContract_ReturnsUserContract()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserContractRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password()
                });

            var fakeContract = new Faker<Contract>()
                .CustomInstantiator(f => new Contract
                {
                    ContractId = Guid.NewGuid(),
                    Body = f.Lorem.Paragraph(),
                    IsActive = f.Random.Bool(),
                    Name = f.Lorem.Text(),
                    Type = f.Random.Int(1, 2),
                    Version = f.Random.Double(),
                    PreviousVersion = f.Random.Double(),
                    RequiresAt = f.Lorem.Sentence(),
                    CreateDate = DateTime.Now
                });

            var fakeUserContract = new Faker<UserContract>()
                .CustomInstantiator(f => new UserContract
                {
                    UserContractId = Guid.NewGuid(),
                    UserId = f.Random.Guid(),
                    ContractId = f.Random.Guid(),
                    FileName = f.System.FileName(),
                    Date = DateTime.Now
                });

            var user = fakeUser.Generate();
            var contract = fakeContract.Generate();
            var userContract = fakeUserContract.Generate();
            userContract.UserId = user.UserId;
            userContract.ContractId = contract.ContractId;

            // Adding necessary data to in-memory database
            dbContext.Users.Add(user);
            dbContext.Contracts.Add(contract);
            dbContext.UserContracts.Add(userContract);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetUserContractAsync(userContract.UserContractId);

            Assert.NotNull(result);
            Assert.Equal(userContract.UserContractId, result.UserContractId);
            Assert.Equal(user.UserId, result.User.UserId);
            Assert.Equal(contract.ContractId, result.Contract.ContractId);
        }
    }

    [Fact]
    public async Task GetUserContract_ThrowsNotFoundException_WhenUserContractDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserContractRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetUserContractAsync(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetUsersContract

    [Fact]
    public async Task GetUserContracts_ReturnsUserContracts()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<UserContractRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeUser = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    UserId = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    PasswordHash = f.Internet.Password()
                });

            var fakeContract = new Faker<Contract>()
                .CustomInstantiator(f => new Contract
                {
                    ContractId = Guid.NewGuid(),
                    Body = f.Lorem.Paragraph(),
                    IsActive = f.Random.Bool(),
                    Name = f.Lorem.Text(),
                    Type = f.Random.Int(1, 2),
                    Version = f.Random.Double(),
                    PreviousVersion = f.Random.Double(),
                    RequiresAt = f.Lorem.Sentence(),
                    CreateDate = DateTime.Now
                });

            var fakeUserContract = new Faker<UserContract>()
                .CustomInstantiator(f => new UserContract
                {
                    UserContractId = Guid.NewGuid(),
                    UserId = f.Random.Guid(),
                    ContractId = f.Random.Guid(),
                    FileName = f.System.FileName(),
                    Date = DateTime.Now
                });

            var user1 = fakeUser.Generate();
            var user2 = fakeUser.Generate();
            var contract1 = fakeContract.Generate();
            var contract2 = fakeContract.Generate();
            var userContract1 = fakeUserContract.Generate();
            var userContract2 = fakeUserContract.Generate();
            userContract1.UserId = user1.UserId;
            userContract1.ContractId = contract1.ContractId;
            userContract2.UserId = user2.UserId;
            userContract2.ContractId = contract2.ContractId;

            // Adding necessary data to in-memory database
            dbContext.Users.AddRange(user1, user2);
            dbContext.Contracts.AddRange(contract1, contract2);
            dbContext.UserContracts.AddRange(userContract1, userContract2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetUserContractsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, uc => uc.UserContractId == userContract1.UserContractId);
            Assert.Contains(result, uc => uc.UserContractId == userContract2.UserContractId);
        }
    }

    #endregion
}