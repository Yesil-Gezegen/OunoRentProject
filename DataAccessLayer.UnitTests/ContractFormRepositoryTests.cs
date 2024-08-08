using Bogus;
using BusinessLayer.Mapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using DataAccessLayer.Concrete.Repository;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.ContactForm.Request;

namespace DataAccessLayer.UnitTests;

public class ContactFormRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    public ContactFormRepositoryTests()
    {
        var services = new ServiceCollection();

        // Using In-Memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Registering ContactFormRepository and IMapper
        services.AddScoped<ContactFormRepository>();
        services.AddAutoMapper(typeof(MapperProfile));

        _serviceProvider = services.BuildServiceProvider();
    }

    #region CreateContactForm

    [Fact]
    public async Task CreateContactForm_ReturnsSuccess()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ContactFormRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeContactFormRequest = new Faker<CreateContactFormRequest>()
                .CustomInstantiator(f => new CreateContactFormRequest
                (
                    Name: f.Name.FullName(),
                    Email: f.Internet.Email(),
                    SubjectCategory: f.Commerce.Department(),
                    Subject: f.Lorem.Sentence(),
                    Body: f.Lorem.Paragraph(),
                    FormDate: f.Date.Past()
                ));

            var request = fakeContactFormRequest.Generate();

            var result = await repo.CreateContactForm(request);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.ContactFormId);

            var contactForm = dbContext.ContactForms.FirstOrDefault(p => p.ContactFormId == result.ContactFormId);
            Assert.NotNull(contactForm);
            Assert.Equal(request.Name.Trim(), contactForm.Name);
            Assert.Equal(request.Email.Trim(), contactForm.Email);
            Assert.Equal(request.SubjectCategory.Trim(), contactForm.SubjectCategory);
            Assert.Equal(request.Subject.Trim(), contactForm.Subject);
            Assert.Equal(request.Body.Trim(), contactForm.Body);
            Assert.Equal(request.FormDate, contactForm.FormDate);
        }
    }

    #endregion
    
    #region DeleteContactForm

    [Fact]
    public async Task DeleteContactForm_ReturnsDeletedContactFormId()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ContactFormRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeContactForm = new Faker<ContactForm>()
                .CustomInstantiator(f => new ContactForm
                {
                    ContactFormId = Guid.NewGuid(),
                    Name = f.Name.FullName(),
                    Email = f.Internet.Email(),
                    SubjectCategory = f.Commerce.Department(),
                    Subject = f.Lorem.Sentence(),
                    Body = f.Lorem.Paragraph(),
                    FormDate = f.Date.Past()
                });

            var contactForm = fakeContactForm.Generate();

            dbContext.ContactForms.Add(contactForm);
            await dbContext.SaveChangesAsync();

            var result = await repo.DeleteContactForm(contactForm.ContactFormId);

            Assert.Equal(contactForm.ContactFormId, result);
        }
    }

    [Fact]
    public async Task DeleteContactForm_ThrowsNotFoundException_WhenContactFormDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ContactFormRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.DeleteContactForm(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetContactForm

    [Fact]
    public async Task GetContactForm_ReturnsContactForm()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ContactFormRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeContactForm = new Faker<ContactForm>()
                .CustomInstantiator(f => new ContactForm
                {
                    ContactFormId = Guid.NewGuid(),
                    Name = f.Name.FullName(),
                    Email = f.Internet.Email(),
                    SubjectCategory = f.Commerce.Department(),
                    Subject = f.Lorem.Sentence(),
                    Body = f.Lorem.Paragraph(),
                    FormDate = f.Date.Past()
                });

            var contactForm = fakeContactForm.Generate();

            dbContext.ContactForms.Add(contactForm);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetContactForm(contactForm.ContactFormId);

            Assert.NotNull(result);
            Assert.Equal(contactForm.ContactFormId, result.ContactFormId);
            Assert.Equal(contactForm.Name, result.Name);
            Assert.Equal(contactForm.Email, result.Email);
            Assert.Equal(contactForm.SubjectCategory, result.SubjectCategory);
            Assert.Equal(contactForm.Subject, result.Subject);
            Assert.Equal(contactForm.Body, result.Body);
            Assert.Equal(contactForm.FormDate, result.FormDate);
        }
    }

    [Fact]
    public async Task GetContactForm_ThrowsNotFoundException_WhenContactFormDoesNotExist()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ContactFormRepository>();

            await Assert.ThrowsAsync<NotFoundException>(() => repo.GetContactForm(Guid.NewGuid()));
        }
    }

    #endregion

    #region GetContactForms

    [Fact]
    public async Task GetContactForms_ReturnsContactForms()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var repo = scopedService.GetRequiredService<ContactFormRepository>();
            var dbContext = scopedService.GetRequiredService<ApplicationDbContext>();

            var fakeContactForm1 = new Faker<ContactForm>()
                .CustomInstantiator(f => new ContactForm
                {
                    ContactFormId = Guid.NewGuid(),
                    Name = f.Name.FullName(),
                    Email = f.Internet.Email(),
                    SubjectCategory = f.Commerce.Department(),
                    Subject = f.Lorem.Sentence(),
                    Body = f.Lorem.Paragraph(),
                    FormDate = f.Date.Past()
                });

            var fakeContactForm2 = new Faker<ContactForm>()
                .CustomInstantiator(f => new ContactForm
                {
                    ContactFormId = Guid.NewGuid(),
                    Name = f.Name.FullName(),
                    Email = f.Internet.Email(),
                    SubjectCategory = f.Commerce.Department(),
                    Subject = f.Lorem.Sentence(),
                    Body = f.Lorem.Paragraph(),
                    FormDate = f.Date.Past()
                });

            var contactForm1 = fakeContactForm1.Generate();
            var contactForm2 = fakeContactForm2.Generate();

            dbContext.ContactForms.AddRange(contactForm1, contactForm2);
            await dbContext.SaveChangesAsync();

            var result = await repo.GetContactForms();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.ContactFormId == contactForm1.ContactFormId);
            Assert.Contains(result, r => r.ContactFormId == contactForm2.ContactFormId);
        }
    }

    #endregion
}