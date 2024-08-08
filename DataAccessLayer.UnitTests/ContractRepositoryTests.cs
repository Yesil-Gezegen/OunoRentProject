using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.Contract.Request;

namespace DataAccessLayer.UnitTests
{
    public class ContractRepositoryTests
    {
        private readonly ServiceProvider _serviceProvider;

        public ContractRepositoryTests()
        {
            var services = new ServiceCollection();

            // Using In-Memory database for testing
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Registering ContractRepository and IMapper
            services.AddScoped<ContractRepository>();
            services.AddAutoMapper(typeof(MapperProfile));

            _serviceProvider = services.BuildServiceProvider();
        }

        #region CreateContract

        [Fact]
        public async Task CreateContract_ReturnsSuccess()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<ContractRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeContractRequest = new Faker<CreateContractRequest>()
                    .CustomInstantiator(f => new CreateContractRequest
                    (
                        Name: f.Company.CompanyName(),
                        Body: f.Lorem.Paragraphs(),
                        Type: f.Random.Int(1, 2),
                        RequiresAt: f.Lorem.Sentence(),
                        IsActive: f.Random.Bool()
                    ));

                var request = fakeContractRequest.Generate();

                var result = await repo.CreateContract(request);

                Assert.NotNull(result);
                Assert.NotEqual(Guid.Empty, result.ContractId);

                var contract = dbContext.Contracts.FirstOrDefault(p => p.ContractId == result.ContractId);
                Assert.NotNull(contract);
                Assert.Equal(request.Name.Trim(), contract.Name);
                Assert.Equal(request.Body.Trim(), contract.Body);
                Assert.Equal(request.Type, contract.Type);
                Assert.Equal(request.RequiresAt.Trim(), contract.RequiresAt);
                Assert.Equal(request.IsActive, contract.IsActive);
            }
        }

        [Fact]
        public async Task CreateContract_SetsCorrectVersion()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<ContractRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeContract = new Faker<Contract>()
                    .CustomInstantiator(f => new Contract
                    {
                        ContractId = Guid.NewGuid(),
                        Name = f.Company.CompanyName(),
                        Body = f.Lorem.Paragraphs(),
                        Type = f.Random.Int(1, 2),
                        RequiresAt = f.Lorem.Sentence(),
                        IsActive = f.Random.Bool(),
                        CreateDate = f.Date.Past(),
                        Version = 1.0,
                        PreviousVersion = 0.0
                    });

                var contract = fakeContract.Generate();

                dbContext.Contracts.Add(contract);
                await dbContext.SaveChangesAsync();

                var fakeContractRequest = new Faker<CreateContractRequest>()
                    .CustomInstantiator(f => new CreateContractRequest
                    (
                        Name: f.Company.CompanyName(),
                        Body: f.Lorem.Paragraphs(),
                        Type: f.Random.Int(1, 2),
                        RequiresAt: f.Lorem.Sentence(),
                        IsActive: f.Random.Bool()
                    ));

                var request = fakeContractRequest.Generate();

                var result = await repo.CreateContract(request);

                Assert.NotNull(result);
                Assert.NotEqual(Guid.Empty, result.ContractId);

                var createdContract = dbContext.Contracts.FirstOrDefault(p => p.ContractId == result.ContractId);
                Assert.NotNull(createdContract);
                Assert.Equal(contract.Version + 0.1, createdContract.Version);
                Assert.Equal(contract.Version, createdContract.PreviousVersion);
            }
        }

        #endregion

        #region GetContract

        [Fact]
        public async Task GetContract_ReturnsContract()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<ContractRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeContract = new Faker<Contract>()
                    .CustomInstantiator(f => new Contract
                    {
                        ContractId = Guid.NewGuid(),
                        Name = f.Company.CompanyName(),
                        Body = f.Lorem.Paragraphs(),
                        Type = f.Random.Int(1, 2),
                        RequiresAt = f.Lorem.Sentence(),
                        IsActive = f.Random.Bool(),
                        CreateDate = f.Date.Past(),
                        Version = 1.0,
                        PreviousVersion = 0.0
                    });

                var contract = fakeContract.Generate();

                dbContext.Contracts.Add(contract);
                await dbContext.SaveChangesAsync();

                var result = await repo.GetContract(contract.ContractId);

                Assert.NotNull(result);
                Assert.Equal(contract.ContractId, result.ContractId);
                Assert.Equal(contract.Name, result.Name);
                Assert.Equal(contract.Body, result.Body);
                Assert.Equal(contract.Type, result.Type);
                Assert.Equal(contract.RequiresAt, result.RequiresAt);
                Assert.Equal(contract.IsActive, result.IsActive);
            }
        }

        [Fact]
        public async Task GetContract_ThrowsNotFoundException_WhenContractDoesNotExist()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<ContractRepository>();

                await Assert.ThrowsAsync<NotFoundException>(() => repo.GetContract(Guid.NewGuid()));
            }
        }

        #endregion

        #region GetContracts

        [Fact]
        public async Task GetContracts_ReturnsContracts()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<ContractRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeContract1 = new Faker<Contract>()
                    .CustomInstantiator(f => new Contract
                    {
                        ContractId = Guid.NewGuid(),
                        Name = f.Company.CompanyName(),
                        Body = f.Lorem.Paragraphs(),
                        Type = f.Random.Int(1, 2),
                        RequiresAt = f.Lorem.Sentence(),
                        IsActive = f.Random.Bool(),
                        CreateDate = f.Date.Past(),
                        Version = 1.0,
                        PreviousVersion = 0.0
                    });

                var fakeContract2 = new Faker<Contract>()
                    .CustomInstantiator(f => new Contract
                    {
                        ContractId = Guid.NewGuid(),
                        Name = f.Company.CompanyName(),
                        Body = f.Lorem.Paragraphs(),
                        Type = f.Random.Int(1, 2),
                        RequiresAt = f.Lorem.Sentence(),
                        IsActive = f.Random.Bool(),
                        CreateDate = f.Date.Past(),
                        Version = 1.1,
                        PreviousVersion = 1.0
                    });

                var contract1 = fakeContract1.Generate();
                var contract2 = fakeContract2.Generate();

                dbContext.Contracts.AddRange(contract1, contract2);
                await dbContext.SaveChangesAsync();

                var result = await repo.GetContracts();

                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.Contains(result, r => r.ContractId == contract1.ContractId);
                Assert.Contains(result, r => r.ContractId == contract2.ContractId);
            }
        }

        #endregion
        
        //TODO Sözleşmelerde ad farklı olursa yeni ekleme yapacak aynıysa versiyon güncelleme yapacak testleri gir
    }
}