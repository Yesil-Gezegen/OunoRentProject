namespace Shared.DTO.ContactForm.Request;

public sealed record CreateContactFormRequest(string Name, string Email, string SubjectCategory, string Subject, string Body, DateTime FormDate );
