using MediatR;
using Shared.DTO.ContactForm.Request;
using Shared.DTO.ContactForm.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.ContactForm.Command;

public sealed record CreateContactFormCommand(CreateContactFormRequest CreateContactFormRequest) : IRequest<ContactFormResponse>
{
    internal class CreateContactFormCommandHandler : IRequestHandler<CreateContactFormCommand, ContactFormResponse>
    {
        private readonly IContactFormRepository _contactFormRepository;

        public CreateContactFormCommandHandler(IContactFormRepository contactFormRepository)
        {
            _contactFormRepository = contactFormRepository;
        }

        public async Task<ContactFormResponse> Handle(CreateContactFormCommand request, CancellationToken cancellationToken)
        {
            return await _contactFormRepository.CreateContactForm(request.CreateContactFormRequest);
        }
    }
}