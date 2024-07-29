using MediatR;
using Shared.DTO.Slider.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Slider.Query;

public sealed record GetSliderQuery(Guid SliderId) : IRequest<GetSliderResponse>
{
    internal class GetSliderQueryHandler : IRequestHandler<GetSliderQuery, GetSliderResponse>
    {
        private readonly ISliderRepository _sliderRepository;

        public GetSliderQueryHandler(ISliderRepository sliderRepository)
        {
            _sliderRepository = sliderRepository;
        }
        public async Task<GetSliderResponse> Handle(GetSliderQuery request, CancellationToken cancellationToken)
        {
            var slider = await _sliderRepository.GetSlider(request.SliderId);

            return slider;
        }
    }
}
