using MediatR;
using Shared.DTO.ContactForm.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.ContactForm.Query;

public sealed record GetContactFormsQuery: IRequest<List<GetContactFormsResponse>>
{
    internal class GetContactFormsQueryHandler : IRequestHandler<GetContactFormsQuery, List<GetContactFormsResponse>>
    {
        private readonly IContactFormRepository _contactFormRepository;

        public GetContactFormsQueryHandler(IContactFormRepository contactFormRepository)
        {
            _contactFormRepository = contactFormRepository;
        }

        public async Task<List<GetContactFormsResponse>> Handle(GetContactFormsQuery request, CancellationToken cancellationToken)
        {
            return await _contactFormRepository.GetContactForms();
        }
    }
}