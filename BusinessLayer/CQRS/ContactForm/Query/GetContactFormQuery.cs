using MediatR;
using Shared.DTO.ContactForm.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.ContactForm.Query;

public sealed record GetContactFormQuery(Guid ContactFormId) : IRequest<GetContactFormResponse>
{
    internal class GetContactFormQueryHandler : IRequestHandler<GetContactFormQuery, GetContactFormResponse>
    {
        private readonly IContactFormRepository _contactFormRepository;

        public GetContactFormQueryHandler(IContactFormRepository contactFormRepository)
        {
            _contactFormRepository = contactFormRepository;
        }

        public async Task<GetContactFormResponse> Handle(GetContactFormQuery request, CancellationToken cancellationToken)
        {
            return await _contactFormRepository.GetContactForm(request.ContactFormId);
        }
    }
}