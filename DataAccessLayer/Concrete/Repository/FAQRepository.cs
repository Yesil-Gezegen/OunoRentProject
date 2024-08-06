using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.FAQ.Request;
using Shared.DTO.FAQ.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class FAQRepository : IFAQRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public FAQRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateFAQ
    
    public async Task<FAQResponse> CreateFAQAsync(CreateFAQRequest createFaqRequest)
    {
        await HasConflict(createFaqRequest);

        var faq = new FAQ();

        faq.Label = createFaqRequest.Label.Trim();
        faq.Text = createFaqRequest.Text.Trim();
        faq.OrderNumber = createFaqRequest.OrderNumber;
        faq.IsActive = createFaqRequest.IsActive;

        await _applicationDbContext.FAQ.AddAsync(faq);
        await _applicationDbContext.SaveChangesAsync();

        var faqResponse = _mapper.Map<FAQResponse>(faq);
        return faqResponse;
    }
    
    #endregion

    #region GetFAQs
    
    public async Task<List<GetFAQsResponse>> GetFAQsAsync(Expression<Func<GetFAQResponse, bool>>? predicate = null)
    {
        var faqs = _applicationDbContext.FAQ.AsNoTracking();

        if (predicate != null)
        {
            var faqPredicate = _mapper.MapExpression<Expression<Func<FAQ, bool>>>(predicate);
            faqs = faqs.Where(faqPredicate);
        }

        var faqsList = await faqs
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();

        var faqResponse = _mapper.Map<List<GetFAQsResponse>>(faqsList);
        return faqResponse;
    }

    #endregion

    #region GetFAQ
    
    public async Task<GetFAQResponse> GetFAQAsync(Guid faqId)
    {
        var faq = await _applicationDbContext.FAQ.FirstOrDefaultAsync(f => f.FAQId == faqId)
                  ?? throw new NotFoundException(FAQExceptionMessages.NotFound);

        return _mapper.Map<GetFAQResponse>(faq);
    }

    #endregion

    #region UpdateFAQ
    
    public async Task<FAQResponse> UpdateFAQAsync(UpdateFAQRequest updateFaqRequest)
    {
        await HasConflict(updateFaqRequest);

        var faq = await _applicationDbContext.FAQ.FirstOrDefaultAsync(f => f.FAQId == updateFaqRequest.FAQId) ??
                  throw new NotFoundException(FAQExceptionMessages.NotFound);

        faq.Label = updateFaqRequest.Label.Trim();
        faq.Text = updateFaqRequest.Text.Trim();
        faq.OrderNumber = updateFaqRequest.OrderNumber;
        faq.IsActive = updateFaqRequest.IsActive;

        _applicationDbContext.FAQ.Update(faq);
        
        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<FAQResponse>(faq);
    }

    #endregion

    #region DeleteFAQ
    
    public async Task<FAQResponse> DeleteFAQAsync(Guid faqId)
    {
        var faq = await _applicationDbContext.FAQ.FirstOrDefaultAsync(f => f.FAQId == faqId) ??
                  throw new NotFoundException(FAQExceptionMessages.NotFound);

        _applicationDbContext.FAQ.Remove(faq);

        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<FAQResponse>(faq);
    }

    #endregion

    #region HasConflict
    
    private async Task HasConflict(CreateFAQRequest createFaqRequest)
    {
        var isOrderNumberExist = await _applicationDbContext.FAQ
            .AnyAsync(f => f.OrderNumber == createFaqRequest.OrderNumber);

        var isLabelExist = await _applicationDbContext.FAQ.AnyAsync(f => f.Label == createFaqRequest.Label);

        if (isOrderNumberExist)
            throw new ConflictException(FAQExceptionMessages.OrderNumberConflict);

        if (isLabelExist)
            throw new ConflictException(FAQExceptionMessages.LabelConflict);
    }

    private async Task HasConflict(UpdateFAQRequest updateFaqRequest)
    {
        var isOrderNumberExist = await _applicationDbContext.FAQ.AnyAsync(f =>
            f.FAQId != updateFaqRequest.FAQId && f.OrderNumber == updateFaqRequest.OrderNumber);

        var isLabelExist = await _applicationDbContext.FAQ.AnyAsync(f =>
            f.FAQId != updateFaqRequest.FAQId && f.Label == updateFaqRequest.Label);

        if (isOrderNumberExist)
            throw new ConflictException(FAQExceptionMessages.OrderNumberConflict);
        
        if (isLabelExist)
            throw new ConflictException(FAQExceptionMessages.LabelConflict);
    }
    
    #endregion

    #region IsExist

    private async Task<bool> IsExist(Expression<Func<FAQ, bool>> expression)
    {
        return await _applicationDbContext.FAQ.AnyAsync(expression);
    }
    
    #endregion
}
