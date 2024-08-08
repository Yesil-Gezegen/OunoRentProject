using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.FAQ.Request;

namespace DataAccessLayer.UnitTests
{
    public class FAQRepositoryTests
    {
        private readonly ServiceProvider _serviceProvider;

        public FAQRepositoryTests()
        {
            var services = new ServiceCollection();

            // Using In-Memory database for testing
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Registering FAQRepository and IMapper
            services.AddScoped<FAQRepository>();
            services.AddAutoMapper(typeof(MapperProfile));

            _serviceProvider = services.BuildServiceProvider();
        }


        #region CreateFAQ

        [Fact]
        public async Task CreateFAQ_ReturnsSuccess()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeFAQRequest = new Faker<CreateFAQRequest>()
                    .CustomInstantiator(f => new CreateFAQRequest
                    (
                        Label: f.Lorem.Sentence(),
                        Text: f.Lorem.Paragraph(),
                        OrderNumber: f.Random.Int(1, 100),
                        IsActive: f.Random.Bool()
                    ));

                var request = fakeFAQRequest.Generate();

                var result = await repo.CreateFAQAsync(request);
                Assert.NotNull(result);
                Assert.NotEqual(Guid.Empty, result.FAQId);

                var faq = dbContext.FAQ.FirstOrDefault(p => p.FAQId == result.FAQId);
                Assert.NotNull(faq);
                Assert.Equal(request.Label, faq.Label);
                Assert.Equal(request.Text, faq.Text);
                Assert.Equal(request.OrderNumber, faq.OrderNumber);
                Assert.Equal(request.IsActive, faq.IsActive);
            }
        }

        [Fact]
        public async Task CreateFAQ_ThrowsConflictException_WhenOrderNumberExists()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeFAQ = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = f.Lorem.Sentence(),
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = 1,
                        IsActive = f.Random.Bool(),
                        CreatedDateTime = DateTime.Now
                    });

                var faq = fakeFAQ.Generate();

                dbContext.FAQ.Add(faq);
                await dbContext.SaveChangesAsync();

                var fakeFAQRequest = new Faker<CreateFAQRequest>()
                    .CustomInstantiator(f => new CreateFAQRequest
                    (
                        Label: f.Lorem.Sentence(),
                        Text: f.Lorem.Paragraph(),
                        OrderNumber: 1, // Same order number
                        IsActive: f.Random.Bool()
                    ));

                var request = fakeFAQRequest.Generate();

                await Assert.ThrowsAsync<ConflictException>(() => repo.CreateFAQAsync(request));
            }
        }

        [Fact]
        public async Task CreateFAQ_ThrowsConflictException_WhenLabelExists()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeFAQ = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = "Test Label",
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = f.Random.Int(1, 100),
                        IsActive = f.Random.Bool(),
                        CreatedDateTime = DateTime.Now
                    });

                var faq = fakeFAQ.Generate();

                dbContext.FAQ.Add(faq);
                await dbContext.SaveChangesAsync();

                var fakeFAQRequest = new Faker<CreateFAQRequest>()
                    .CustomInstantiator(f => new CreateFAQRequest
                    (
                        Label: "Test Label", // Same label
                        Text: f.Lorem.Paragraph(),
                        OrderNumber: f.Random.Int(1, 100),
                        IsActive: f.Random.Bool()
                    ));

                var request = fakeFAQRequest.Generate();

                await Assert.ThrowsAsync<ConflictException>(() => repo.CreateFAQAsync(request));
            }
        }

        #endregion

        #region GetFAQ

        [Fact]
        public async Task GetFAQ_ReturnsFAQ()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeFAQ = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = f.Lorem.Sentence(),
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = f.Random.Int(1, 100),
                        IsActive = f.Random.Bool(),
                        CreatedDateTime = DateTime.Now
                    });

                var faq = fakeFAQ.Generate();

                dbContext.FAQ.Add(faq);
                await dbContext.SaveChangesAsync();

                var result = await repo.GetFAQAsync(faq.FAQId);

                Assert.NotNull(result);
                Assert.Equal(faq.FAQId, result.FAQId);
                Assert.Equal(faq.Label, result.Label);
                Assert.Equal(faq.Text, result.Text);
                Assert.Equal(faq.OrderNumber, result.OrderNumber);
                Assert.Equal(faq.IsActive, result.IsActive);
            }
        }

        [Fact]
        public async Task GetFAQ_ThrowsNotFoundException_WhenFAQDoesNotExist()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();

                await Assert.ThrowsAsync<NotFoundException>(() => repo.GetFAQAsync(Guid.NewGuid()));
            }
        }

        #endregion

        #region GetFAQs

        [Fact]
        public async Task GetFAQs_ReturnsFAQs()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeFAQ = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = f.Lorem.Sentence(),
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = f.Random.Int(1, 100),
                        IsActive = f.Random.Bool(),
                        CreatedDateTime = DateTime.Now
                    });

                var faq1 = fakeFAQ.Generate();
                var faq2 = fakeFAQ.Generate();

                dbContext.FAQ.RemoveRange(dbContext.FAQ);
                dbContext.FAQ.AddRange(faq1, faq2);
                await dbContext.SaveChangesAsync();

                var result = await repo.GetFAQsAsync();

                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.Contains(result, r => r.FAQId == faq1.FAQId);
                Assert.Contains(result, r => r.FAQId == faq2.FAQId);
            }
        }

        [Fact]
        public async Task GetActiveFAQs_ReturnsFAQs()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeFAQ1 = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = f.Lorem.Sentence(),
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = f.Random.Int(1, 100),
                        IsActive = true,
                        CreatedDateTime = DateTime.Now
                    });
                
                var fakeFAQ2 = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = f.Lorem.Sentence(),
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = f.Random.Int(1, 100),
                        IsActive = false,
                        CreatedDateTime = DateTime.Now
                    });

                var faq1 = fakeFAQ1.Generate();
                var faq2 = fakeFAQ2.Generate();

                dbContext.FAQ.RemoveRange(dbContext.FAQ);
                dbContext.FAQ.AddRange(faq1, faq2);
                await dbContext.SaveChangesAsync();

                var result = await repo.GetFAQsAsync(p => p.IsActive);

                Assert.NotNull(result);
                Assert.Single(result);
                Assert.Contains(result, r => r.FAQId == faq1.FAQId);
                foreach (var item in result)
                {
                    Assert.True(item.IsActive);
                }
            }
        }

        #endregion

        #region UpdateFAQ

        [Fact]
        public async Task UpdateFAQ_ReturnsUpdatedFAQ()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeFAQ = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = f.Lorem.Sentence(),
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = f.Random.Int(1, 100),
                        IsActive = f.Random.Bool(),
                        CreatedDateTime = DateTime.Now
                    });

                var faq = fakeFAQ.Generate();

                dbContext.FAQ.Add(faq);
                await dbContext.SaveChangesAsync();

                var fakeUpdateFAQRequest = new Faker<UpdateFAQRequest>()
                    .CustomInstantiator(f => new UpdateFAQRequest
                    (
                        FAQId: faq.FAQId,
                        Label: f.Lorem.Sentence(),
                        Text: f.Lorem.Paragraph(),
                        OrderNumber: f.Random.Int(1, 100),
                        IsActive: f.Random.Bool()
                    ));

                var request = fakeUpdateFAQRequest.Generate();
                var result = await repo.UpdateFAQAsync(request);

                Assert.NotNull(result);
                Assert.NotEqual(Guid.Empty, result.FAQId);

                var updatedFaq = dbContext.FAQ.FirstOrDefault(p => p.FAQId == result.FAQId);

                Assert.NotNull(updatedFaq);
                Assert.Equal(request.FAQId, updatedFaq.FAQId);
                Assert.Equal(request.Label, updatedFaq.Label);
                Assert.Equal(request.Text, updatedFaq.Text);
                Assert.Equal(request.OrderNumber, updatedFaq.OrderNumber);
                Assert.Equal(request.IsActive, updatedFaq.IsActive);
            }
        }

        [Fact]
        public async Task UpdateFAQ_ThrowsConflictException_WhenOrderNumberExists()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeFAQ1 = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = f.Lorem.Sentence(),
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = 1,
                        IsActive = f.Random.Bool(),
                        CreatedDateTime = DateTime.Now
                    });

                var fakeFAQ2 = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = f.Lorem.Sentence(),
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = 2,
                        IsActive = f.Random.Bool(),
                        CreatedDateTime = DateTime.Now
                    });

                var faq1 = fakeFAQ1.Generate();
                var faq2 = fakeFAQ2.Generate();

                dbContext.FAQ.AddRange(faq1, faq2);
                await dbContext.SaveChangesAsync();

                var fakeUpdateFAQRequest = new Faker<UpdateFAQRequest>()
                    .CustomInstantiator(f => new UpdateFAQRequest
                    (
                        FAQId: faq2.FAQId,
                        Label: f.Lorem.Sentence(),
                        Text: f.Lorem.Paragraph(),
                        OrderNumber: 1, // Same order number as faq1
                        IsActive: f.Random.Bool()
                    ));

                var request = fakeUpdateFAQRequest.Generate();

                await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateFAQAsync(request));
            }
        }

        [Fact]
        public async Task UpdateFAQ_ThrowsConflictException_WhenLabelExists()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeFAQ1 = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = "Test Label",
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = f.Random.Int(1, 100),
                        IsActive = f.Random.Bool(),
                        CreatedDateTime = DateTime.Now
                    });

                var fakeFAQ2 = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = f.Lorem.Sentence(),
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = f.Random.Int(1, 100),
                        IsActive = f.Random.Bool(),
                        CreatedDateTime = DateTime.Now
                    });

                var faq1 = fakeFAQ1.Generate();
                var faq2 = fakeFAQ2.Generate();

                dbContext.FAQ.AddRange(faq1, faq2);
                await dbContext.SaveChangesAsync();

                var fakeUpdateFAQRequest = new Faker<UpdateFAQRequest>()
                    .CustomInstantiator(f => new UpdateFAQRequest
                    (
                        FAQId: faq2.FAQId,
                        Label: "Test Label", // Same label as faq1
                        Text: f.Lorem.Paragraph(),
                        OrderNumber: f.Random.Int(1, 100),
                        IsActive: f.Random.Bool()
                    ));

                var request = fakeUpdateFAQRequest.Generate();

                await Assert.ThrowsAsync<ConflictException>(() => repo.UpdateFAQAsync(request));
            }
        }

        #endregion

        #region DeleteFAQ

        [Fact]
        public async Task DeleteFAQ_ReturnsDeletedFAQ()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                var fakeFAQ = new Faker<FAQ>()
                    .CustomInstantiator(f => new FAQ
                    {
                        FAQId = Guid.NewGuid(),
                        Label = f.Lorem.Sentence(),
                        Text = f.Lorem.Paragraph(),
                        OrderNumber = f.Random.Int(1, 100),
                        IsActive = f.Random.Bool(),
                        CreatedDateTime = DateTime.Now
                    });

                var faq = fakeFAQ.Generate();

                dbContext.FAQ.Add(faq);
                await dbContext.SaveChangesAsync();

                var result = await repo.DeleteFAQAsync(faq.FAQId);

                Assert.NotNull(result);
                Assert.Equal(faq.FAQId, result.FAQId);
            }
        }

        [Fact]
        public async Task DeleteFAQ_ThrowsNotFoundException_WhenFAQDoesNotExist()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var repo = scopedService.GetRequiredService<FAQRepository>();
                var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

                await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteFAQAsync(Guid.NewGuid()));
            }
        }

        #endregion
    }
}