using MediatR;
using Shared.DTO.Slider.Request;
using Shared.DTO.Slider.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Slider.Command;

public sealed record CreateSliderCommand(CreateSliderRequest CreateSliderRequest) : IRequest<SliderResponse>;
internal class CreateSliderCommandHandler : IRequestHandler<CreateSliderCommand, SliderResponse>
{
    private readonly ISliderRepository _sliderRepository;

    public CreateSliderCommandHandler(ISliderRepository sliderRepository)
    {
        _sliderRepository = sliderRepository;
    }

    public async Task<SliderResponse> Handle(CreateSliderCommand request, CancellationToken cancellationToken)
    {
        return await _sliderRepository.CreateSlider(request.CreateSliderRequest);
    }
}