namespace Shared.DTO.Price.Request;

public sealed record UpdatePriceRequest(Guid PriceId, Decimal LogoPrice);
