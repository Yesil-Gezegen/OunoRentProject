using MediatR;
using Shared.DTO.FAQ.Request;
using Shared.DTO.FAQ.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FAQ.Command;

public sealed record UpdateFAQCommand(UpdateFAQRequest UpdateFaqRequest) : IRequest<FAQResponse>;

public class UpdateFAQCommandHandler : IRequestHandler<UpdateFAQCommand, FAQResponse>
{
    private readonly IFAQRepository _faqRepository;

    public UpdateFAQCommandHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }
    
    public async Task<FAQResponse> Handle(UpdateFAQCommand request, CancellationToken cancellationToken)
    {
        var faqResponse = await _faqRepository.UpdateFAQAsync(request.UpdateFaqRequest);
        return faqResponse;
    }
}