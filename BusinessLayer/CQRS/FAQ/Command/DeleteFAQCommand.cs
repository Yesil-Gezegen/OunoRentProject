using MediatR;
using Shared.DTO.FAQ.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FAQ.Command;

public sealed record DeleteFAQCommand(Guid FAQId) : IRequest<FAQResponse>;

public class DeleteFAQCommandHandler : IRequestHandler<DeleteFAQCommand, FAQResponse>
{
    private readonly IFAQRepository _faqRepository;

    public DeleteFAQCommandHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }
    
    public async Task<FAQResponse> Handle(DeleteFAQCommand request, CancellationToken cancellationToken)
    {
        var faqResponse = await _faqRepository.DeleteFAQAsync(request.FAQId);
        return faqResponse;
    }
}