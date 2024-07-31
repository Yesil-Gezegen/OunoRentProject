using System.Linq.Expressions;
using Shared.DTO.FAQ.Request;
using Shared.DTO.FAQ.Response;

namespace Shared.Interface;

public interface IFAQRepository
{
    Task<FAQResponse> CreateFAQAsync(CreateFAQRequest createFaqRequest);
    
    Task<List<GetFAQsResponse>> GetFAQsAsync(Expression<Func<GetFAQResponse, bool>>? predicate = null);
    
    Task<GetFAQResponse> GetFAQAsync(Guid faqId);
    
    Task<FAQResponse> UpdateFAQAsync(UpdateFAQRequest updateFaqRequest);
    
    Task<FAQResponse> DeleteFAQAsync(Guid faqId);
}