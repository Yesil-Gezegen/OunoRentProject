using System.Linq.Expressions;
using Shared.DTO.Slider.Request;
using Shared.DTO.Slider.Response;

namespace Shared.Interface;

public interface ISliderRepository
{
    Task<SliderResponse> CreateSlider(CreateSliderRequest createSliderRequest);

    Task<List<GetSlidersResponse>> GetSliders(Expression<Func<GetSliderResponse, bool>>? predicate = null);

    Task<GetSliderResponse> GetSlider(Guid sliderId);

    Task<SliderResponse> UpdateSlider(UpdateSliderRequest updateSliderRequest);

    Task<Guid> DeleteSlider(Guid sliderId);
}
