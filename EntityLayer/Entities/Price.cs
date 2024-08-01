namespace EntityLayer.Entities;

public class Price : AuditTrailer
{
    public Guid PriceId { get; set; }

    public string Barcode { get; set; }

    public Decimal LogoPrice { get; set; }
}