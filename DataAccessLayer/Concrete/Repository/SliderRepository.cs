using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Slider.Request;
using Shared.DTO.Slider.Response;
using Shared.Interface;
using ArgumentException = System.ArgumentException;

namespace DataAccessLayer.Concrete.Repository;

public class SliderRepository : ISliderRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;

    public SliderRepository(ApplicationDbContext applicationDbContext, IMapper mapper, IImageService imageService)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _imageService = imageService;
    }

    #region CreateSlider

    public async Task<SliderResponse> CreateSlider(CreateSliderRequest createSliderRequest)
    {
        await IsExistGeneric(x => x.Title == createSliderRequest.Title);

        await IsExistOrderNumber(createSliderRequest.OrderNumber);

        var slider = new Slider();

        slider.Title = createSliderRequest.Title;
        slider.MainImageUrl = await _imageService.SaveImageAsync(createSliderRequest.MainImage);
        slider.MobileImageUrl = await _imageService.SaveImageAsync(createSliderRequest.MobileImage);
        slider.TargetUrl = createSliderRequest.TargetUrl;
        slider.OrderNumber = createSliderRequest.OrderNumber;
        slider.ActiveFrom = createSliderRequest.ActiveFrom.ToUniversalTime();
        slider.ActiveTo = createSliderRequest.ActiveTo.ToUniversalTime();
        slider.Duration = createSliderRequest.Duration;
        slider.IsActive = createSliderRequest.IsActive;

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
                     ?? throw new NotFoundException(SliderExceptionMessages.NotFound);

        // Resimleri sil
        await _imageService.DeleteImageAsync(slider.MainImageUrl);

        await _imageService.DeleteImageAsync(slider.MobileImageUrl);

        // Slider'ı veritabanından sil
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
                     ?? throw new NotFoundException(SliderExceptionMessages.NotFound);

        var getSliderResponse = _mapper.Map<GetSliderResponse>(slider);

        return getSliderResponse;
    }

    #endregion

    #region GetSliders

    public async Task<List<GetSlidersResponse>> GetSliders(Expression<Func<GetSliderResponse, bool>>? predicate = null)
    {
        var sliders = _applicationDbContext.Sliders
            .AsNoTracking();

        // 
        if (predicate != null)
        {
            var slidersPredicate = _mapper.MapExpression<Expression<Func<Slider, bool>>>(predicate);
            sliders = sliders.Where(slidersPredicate);
        }

        var slidersList = await sliders
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();

        var getSlidersResponse = _mapper.Map<List<GetSlidersResponse>>(slidersList);

        return getSlidersResponse;
    }

    #endregion

    #region UpdateSlider

    public async Task<SliderResponse> UpdateSlider(UpdateSliderRequest updateSliderRequest)
    {
        var slider = await _applicationDbContext.Sliders.FirstOrDefaultAsync(
                         x => x.SliderId == updateSliderRequest.SliderId)
                     ?? throw new NotFoundException(SliderExceptionMessages.NotFound);
        
        await IsExistOrderNumberWhenUpdate(updateSliderRequest.SliderId, updateSliderRequest.OrderNumber);

        slider.Title = updateSliderRequest.Title;
        slider.TargetUrl = updateSliderRequest.TargetUrl;
        slider.OrderNumber = updateSliderRequest.OrderNumber;
        slider.Duration = updateSliderRequest.Duration;
        slider.ActiveFrom = updateSliderRequest.ActiveFrom.ToUniversalTime();
        slider.ActiveTo = updateSliderRequest.ActiveTo.ToUniversalTime();
        slider.IsActive = updateSliderRequest.IsActive;

        if (updateSliderRequest.MainImage != null)
        {
            await _imageService.DeleteImageAsync(slider.MainImageUrl);
            slider.MainImageUrl = await _imageService.SaveImageAsync(updateSliderRequest.MainImage);
        }

        if (updateSliderRequest.MobileImage != null)
        {
            await _imageService.DeleteImageAsync(slider.MobileImageUrl);
            slider.MobileImageUrl = await _imageService.SaveImageAsync(updateSliderRequest.MobileImage);
        }

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
            throw new ConflictException(SliderExceptionMessages.OrderNumberConflict);
        }
    }

    private async Task IsExistOrderNumberWhenUpdate(Guid sliderId, int orderNumber)
    {
        var isExistOrderNumber = await _applicationDbContext.Sliders
            .AnyAsync(x => x.SliderId != sliderId && x.OrderNumber == orderNumber);

        if (isExistOrderNumber)
        {
            throw new ConflictException(SliderExceptionMessages.OrderNumberConflict);
        }
    }

    private async Task<bool> IsExistGeneric(Expression<Func<Slider, bool>> filter)
    {
        var result = await _applicationDbContext.Sliders.AnyAsync(filter);

        if (result)
            throw new ConflictException(SliderExceptionMessages.Conflict);

        return result;
    }

    #endregion
}