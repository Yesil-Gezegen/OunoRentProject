namespace Shared.DTO.Slider.Request;

public sealed record CreateSliderRequest(
    string Title,
    string MainImageUrl,
    string MobileImageUrl,
    string TargetUrl,
    int OrderNumber,
    int Duration,
    DateTime ActiveFrom,
    DateTime ActiveTo,
    bool IsActive
);
