using Shared.DTO.ContactForm.Request;
using Shared.DTO.ContactForm.Response;

namespace Shared.Interface;

public interface IContactFormRepository
{
    Task<ContactFormResponse> CreateContactForm(CreateContactFormRequest createContactFormRequest);
    
    Task<List<GetContactFormsResponse>> GetContactForms();
    
    Task<GetContactFormResponse> GetContactForm(Guid contactFormId);
    
    Task<Guid> DeleteContactForm(Guid contactFormId);
}