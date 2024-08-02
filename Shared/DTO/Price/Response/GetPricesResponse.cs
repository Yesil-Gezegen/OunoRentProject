namespace Shared.DTO.Price.Response;

public class GetPricesResponse
{
    public Guid PriceId { get; set; }

    public string Barcode { get; set; }

    public Decimal LogoPrice { get; set; }
}