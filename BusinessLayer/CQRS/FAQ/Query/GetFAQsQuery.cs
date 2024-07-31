using MediatR;
using Shared.DTO.FAQ.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.FAQ.Query;

public sealed record GetFAQsQuery() : IRequest<List<GetFAQsResponse>>;

public class GetFAQsQueryHandler : IRequestHandler<GetFAQsQuery, List<GetFAQsResponse>>
{
    private readonly IFAQRepository _faqRepository;

    public GetFAQsQueryHandler(IFAQRepository faqRepository)
    {
        _faqRepository = faqRepository;
    }
    
    public async Task<List<GetFAQsResponse>> Handle(GetFAQsQuery request, CancellationToken cancellationToken)
    {
        var faqResponse = await _faqRepository.GetFAQsAsync();
        return faqResponse;
    }
}