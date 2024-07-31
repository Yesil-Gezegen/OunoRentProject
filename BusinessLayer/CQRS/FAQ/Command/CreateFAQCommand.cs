using MediatR;
using Shared.DTO.FAQ.Request;
using Shared.DTO.FAQ.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FAQ.Command;

public sealed record CreateFAQCommand(CreateFAQRequest CreateFaqRequest) : IRequest<FAQResponse>;

public class CreateFAQCommandHandler : IRequestHandler<CreateFAQCommand, FAQResponse>
{
    private readonly IFAQRepository _faqRepository;

    public CreateFAQCommandHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }
    
    public async Task<FAQResponse> Handle(CreateFAQCommand request, CancellationToken cancellationToken)
    {
        var faqResponse = await _faqRepository.CreateFAQAsync(request.CreateFaqRequest);
        return faqResponse;
    }
}