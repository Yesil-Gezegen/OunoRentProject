using Microsoft.AspNetCore.Http;

namespace Shared.DTO.Slider.Request;

public sealed record UpdateSliderRequest(
    Guid SliderId,
    string Title,
    string MainImageUrl,
    string MobileImageUrl,
    string TargetUrl,
    int OrderNumber,
    int Duration,
    DateTime ActiveFrom,
    DateTime ActiveTo,
    bool IsActive,
    IFormFile MainImage,
    IFormFile MobileImage
);

