using MediatR;
using Shared.DTO.FAQ.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FAQ.Query;

public sealed record GetFAQQuery(Guid faqId) : IRequest<GetFAQResponse>;

public class GetFAQQueryHandler : IRequestHandler<GetFAQQuery, GetFAQResponse>
{
    private readonly IFAQRepository _faqRepository;

    public GetFAQQueryHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }
    
    public async Task<GetFAQResponse> Handle(GetFAQQuery request, CancellationToken cancellationToken)
    {
        var faqResponse = await _faqRepository.GetFAQAsync(request.faqId);
        return faqResponse;
    }
}