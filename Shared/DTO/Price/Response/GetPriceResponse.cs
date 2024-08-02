namespace Shared.DTO.Price.Response;

public class GetPriceResponse
{
    public Guid PriceId { get; set; }

    public string Barcode { get; set; }

    public Decimal LogoPrice { get; set; }
}