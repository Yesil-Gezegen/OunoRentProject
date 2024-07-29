using MediatR;
using Shared.DTO.Slider.Request;
using Shared.DTO.Slider.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Slider.Command;

public sealed record UpdateSliderCommand(UpdateSliderRequest UpdateSliderRequest) : IRequest<SliderResponse>
{
    internal class UpdateSliderCommandHandler : IRequestHandler<UpdateSliderCommand, SliderResponse>
    {
        private readonly ISliderRepository _sliderRepository;

        public UpdateSliderCommandHandler(ISliderRepository sliderRepository)
        {
            _sliderRepository = sliderRepository;
        }
        public async Task<SliderResponse> Handle(UpdateSliderCommand request, CancellationToken cancellationToken)
        {
            var slider = await _sliderRepository.UpdateSlider(request.UpdateSliderRequest);

            return slider;
        }
    }
}
