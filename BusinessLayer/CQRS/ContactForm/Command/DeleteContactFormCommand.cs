using MediatR;
using Shared.Interface;

namespace BusinessLayer.CQRS.ContactForm.Command;

public sealed record DeleteContactFormCommand(Guid ContactFormId) : IRequest<Guid>
{
    internal  class DeleteContactFormCommandHandler : IRequestHandler<DeleteContactFormCommand, Guid>
    {
        private readonly IContactFormRepository _contactFormRepository;

        public DeleteContactFormCommandHandler(IContactFormRepository contactFormRepository)
        {
            _contactFormRepository = contactFormRepository;
        }

        public async Task<Guid> Handle(DeleteContactFormCommand request, CancellationToken cancellationToken)
        {
            return await _contactFormRepository.DeleteContactForm(request.ContactFormId);
        }
    }
}