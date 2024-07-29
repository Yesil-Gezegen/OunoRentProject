using MediatR;
using Shared.Interface;

namespace BusinessLayer.CQRS.Slider.Command;

public sealed record DeleteSliderCommand(Guid SliderId) : IRequest<Guid>
{
    internal class DeleteSliderCommandHandler : IRequestHandler<DeleteSliderCommand, Guid>
    {
        private readonly ISliderRepository _sliderRepository;

        public DeleteSliderCommandHandler(ISliderRepository sliderRepository)
        {
            _sliderRepository = sliderRepository;
        }
        public async Task<Guid> Handle(DeleteSliderCommand request, CancellationToken cancellationToken)
        {
            var slider = await _sliderRepository.DeleteSlider(request.SliderId);

            return slider;
        }
    }
}
