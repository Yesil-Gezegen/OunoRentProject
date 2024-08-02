namespace EntityLayer.Entities;

public class Address : AuditTrailer
{
    public Guid AddressId { get; set; }
    public int Type { get; set; }
    public string Title { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Neighborhood { get; set; }
    public string AddressDetail { get; set; }
    public int? TaxNo { get; set; }
    public string? TaxOffice { get; set; }
    public string? CompanyName { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
}