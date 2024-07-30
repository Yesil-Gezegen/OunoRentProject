using MediatR;
using Shared.DTO.Slider.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Slider.Query;

public sealed record GetActiveSlidersQuery() : IRequest<List<GetSlidersResponse>>;

class GetActiveSlidersQueryHandler : IRequestHandler<GetActiveSlidersQuery, List<GetSlidersResponse>>
{
    private readonly ISliderRepository _sliderRepository;

    public GetActiveSlidersQueryHandler(ISliderRepository sliderRepository)
    {
        _sliderRepository = sliderRepository;
    }

    public async Task<List<GetSlidersResponse>> Handle(GetActiveSlidersQuery request, CancellationToken cancellationToken)
    {
        var slidersResponse = await _sliderRepository.GetSliders(s => s.IsActive);
        return slidersResponse;
    }
}
