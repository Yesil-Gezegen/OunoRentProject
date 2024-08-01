namespace Shared.DTO.Price.Request;

public sealed record CreatePriceRequest(string Barcode, Decimal LogoPrice);
