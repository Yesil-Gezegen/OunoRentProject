using MediatR;
using Shared.DTO.Slider.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Slider.Query;

public sealed record GetSlidersQuery : IRequest<List<GetSlidersResponse>>
{
    internal class GetSlidersQueryHandler : IRequestHandler<GetSlidersQuery, List<GetSlidersResponse>>
    {
        private readonly ISliderRepository _sliderRepository;

        public GetSlidersQueryHandler(ISliderRepository sliderRepository)
        {
            _sliderRepository = sliderRepository;
        }
        public async Task<List<GetSlidersResponse>> Handle(GetSlidersQuery request, CancellationToken cancellationToken)
        {
            var sliders = await _sliderRepository.GetSliders();

            return sliders;
        }
    }
}
