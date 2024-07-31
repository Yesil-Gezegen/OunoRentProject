using MediatR;
using Shared.DTO.FAQ.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FAQ.Query;

public sealed record GetActiveFAQsQuery : IRequest<List<GetFAQsResponse>>;

public class GetActiveFAQsQueryHandler : IRequestHandler<GetActiveFAQsQuery, List<GetFAQsResponse>>
{
    private readonly IFAQRepository _faqRepository;

    public GetActiveFAQsQueryHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }
    
    public async Task<List<GetFAQsResponse>> Handle(GetActiveFAQsQuery request, CancellationToken cancellationToken)
    {
        var faqResponse = await _faqRepository.GetFAQsAsync(f => f.IsActive);
        return faqResponse;
    }
}