using AutoMapper;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.ContactForm.Request;
using Shared.DTO.ContactForm.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class ContactFormRepository : IContactFormRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    
    public ContactFormRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<ContactFormResponse> CreateContactForm(CreateContactFormRequest createContactFormRequest)
    {
        var contactForm = new ContactForm();
        
        contactForm.Name = createContactFormRequest.Name.Trim();
        contactForm.Email = createContactFormRequest.Email.Trim();
        contactForm.SubjectCategory = createContactFormRequest.SubjectCategory.Trim();
        contactForm.Subject = createContactFormRequest.Subject.Trim();
        contactForm.Body = createContactFormRequest.Body.Trim();
        contactForm.FormDate = createContactFormRequest.FormDate;
        
        _applicationDbContext.ContactForms.Add(contactForm);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return _mapper.Map<ContactFormResponse>(contactForm);

    }

    public async Task<List<GetContactFormsResponse>> GetContactForms()
    {
        var contactFormList = await _applicationDbContext.ContactForms.ToListAsync();
        
        return _mapper.Map<List<GetContactFormsResponse>>(contactFormList);
    }

    public async Task<GetContactFormResponse> GetContactForm(Guid contactFormId)
    {
        var contactForm = await _applicationDbContext.ContactForms
                              .FirstOrDefaultAsync(x => x.ContactFormId == contactFormId)
                          ?? throw new NotFoundException(ContactFormExceptionMessages.NotFound);

        return _mapper.Map<GetContactFormResponse>(contactForm);
    }

    public async Task<Guid> DeleteContactForm(Guid contactFormId)
    {
        var contactForm = await _applicationDbContext.ContactForms
                              .FirstOrDefaultAsync(x => x.ContactFormId == contactFormId)
                          ?? throw new NotFoundException(ContactFormExceptionMessages.NotFound);
        
        _applicationDbContext.ContactForms.Remove(contactForm);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return contactForm.ContactFormId;
    }
}