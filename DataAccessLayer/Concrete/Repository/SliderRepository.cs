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
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SliderRepository(ApplicationDbContext applicationDbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }

    #region CreateSlider

    public async Task<SliderResponse> CreateSlider(CreateSliderRequest createSliderRequest)
    {
        await IsExistGeneric(x => x.Title == createSliderRequest.Title);
        await IsExistOrderNumber(createSliderRequest.OrderNumber);

        string mainImageUrl = null;
        string mobileImageUrl = null;
        
        if (createSliderRequest.File != null)
        {
            mainImageUrl = await SaveImageAsync(createSliderRequest.File);
            mobileImageUrl = await SaveImageAsync(createSliderRequest.File);
        }

        var slider = new Slider();
        
            slider.Title = createSliderRequest.Title;
            slider.MainImageUrl = mainImageUrl ?? throw new InvalidOperationException();
            slider.MobileImageUrl = mobileImageUrl ?? throw new InvalidOperationException();
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

        string mainImageUrl = null;
        string mobileImageUrl = null;
        
        if (updateSliderRequest.File != null)
        {
            mainImageUrl = await SaveImageAsync(updateSliderRequest.File);
            mobileImageUrl = await SaveImageAsync(updateSliderRequest.File);
        }
        
        slider.Title = updateSliderRequest.Title;
        slider.MainImageUrl = mainImageUrl ?? throw new InvalidOperationException();
        slider.MobileImageUrl = mobileImageUrl ?? throw new InvalidOperationException();
        slider.TargetUrl = updateSliderRequest.TargetUrl;
        slider.OrderNumber = updateSliderRequest.OrderNumber;
        slider.Duration = updateSliderRequest.Duration;
        slider.ActiveFrom = updateSliderRequest.ActiveFrom.ToUniversalTime();
        slider.ActiveTo = updateSliderRequest.ActiveTo.ToUniversalTime();
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

    #region SaveImage

    public async Task<string> SaveImageAsync(IFormFile image)
    {
        if (image == null || image.Length == 0)
            throw new BadRequestException(ImageExceptionMessages.InvalidImage);

        if (_webHostEnvironment.WebRootPath == null)
            throw new BadRequestException(ImageExceptionMessages.InvalidPath);

        if (string.IsNullOrEmpty(image.FileName))
            throw new BadRequestException(ImageExceptionMessages.InvalidName);

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        return filePath; 
    }

    #endregion
   
}