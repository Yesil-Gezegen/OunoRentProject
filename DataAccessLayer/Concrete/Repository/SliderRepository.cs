using System.Linq.Expressions;
using AutoMapper;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Slider.Request;
using Shared.DTO.Slider.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class SliderRepository : ISliderRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public SliderRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateSlider

    public async Task<SliderResponse> CreateSlider(CreateSliderRequest createSliderRequest)
    {
        await IsExistGeneric(x => x.Title == createSliderRequest.Title);

        await IsExistOrderNumber(createSliderRequest.OrderNumber);

        var slider = new Slider
        {
            Title = createSliderRequest.Title,
            MainImageUrl = createSliderRequest.MainImageUrl,
            MobileImageUrl = createSliderRequest.MobileImageUrl,
            TargetUrl = createSliderRequest.TargetUrl,
            OrderNumber = createSliderRequest.OrderNumber,
            ActiveFrom = createSliderRequest.ActiveFrom,
            ActiveTo = createSliderRequest.ActiveTo,
            Duration = createSliderRequest.Duration,
            IsActive = createSliderRequest.IsActive
        };

        _applicationDbContext.Sliders.Add(slider);

        await _applicationDbContext.SaveChangesAsync();

        var sliderResponse = _mapper.Map<SliderResponse>(slider);

        return sliderResponse;
    }

    #endregion

    #region DeleteSlider

    public async Task<Guid> DeleteSlider(Guid sliderId)
    {
        var slider = await _applicationDbContext.Sliders
                         .FirstOrDefaultAsync(x => x.SliderId == sliderId)
                     ?? throw new NotFoundException("Slider not found");

        _applicationDbContext.Sliders.Remove(slider);

        await _applicationDbContext.SaveChangesAsync();

        return slider.SliderId;
    }

    #endregion

    #region GetSlider

    public async Task<GetSliderResponse> GetSlider(Guid sliderId)
    {
        var slider = await _applicationDbContext.Sliders
                         .AsNoTracking().FirstOrDefaultAsync(x => x.SliderId == sliderId)
                     ?? throw new NotFoundException("Slider not found");

        var getSliderResponse = _mapper.Map<GetSliderResponse>(slider);

        return getSliderResponse;
    }

    #endregion

    #region GetSliders

    public async Task<List<GetSlidersResponse>> GetSliders()
    {
        var sliders = await _applicationDbContext.Sliders
            .AsNoTracking()
            .ToListAsync();

        var getSlidersResponse = _mapper.Map<List<GetSlidersResponse>>(sliders);

        return getSlidersResponse;
    }

    #endregion

    #region UpdateSlider

    public async Task<SliderResponse> UpdateSlider(UpdateSliderRequest updateSliderRequest)
    {
        var slider = await _applicationDbContext.Sliders.FirstOrDefaultAsync(
                         x => x.SliderId == updateSliderRequest.SliderId)
                     ?? throw new NotFoundException("Slider not found");

        await IsExistOrderNumberWhenUpdate(updateSliderRequest.SliderId, updateSliderRequest.OrderNumber);

        slider.Title = updateSliderRequest.Title;
        slider.MainImageUrl = updateSliderRequest.MainImageUrl;
        slider.MobileImageUrl = updateSliderRequest.MobileImageUrl;
        slider.TargetUrl = updateSliderRequest.TargetUrl;
        slider.OrderNumber = updateSliderRequest.OrderNumber;
        slider.Duration = updateSliderRequest.Duration;
        slider.ActiveFrom = updateSliderRequest.ActiveFrom;
        slider.ActiveTo = updateSliderRequest.ActiveTo;
        slider.IsActive = updateSliderRequest.IsActive;

        await _applicationDbContext.SaveChangesAsync();

        var sliderResponse = _mapper.Map<SliderResponse>(slider);

        return sliderResponse;
    }

    #endregion

    #region IsExist

    private async Task IsExistOrderNumber(int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.Sliders
            .AnyAsync(x => x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
        {
            throw new ConflictException("Order number already exists");
        }
    }
    
    private async Task IsExistOrderNumberWhenUpdate(Guid sliderId, int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.Sliders
            .AnyAsync(x => x.SliderId != sliderId && x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
        {
            throw new ConflictException("Order number already exists");
        }
    }

    private async Task<bool> IsExistGeneric(Expression<Func<Slider, bool>> filter)
    {
        var result = await _applicationDbContext.Sliders.AnyAsync(filter);

        if (result)
            throw new ConflictException("Already exist");

        return result;
    }

    #endregion
}