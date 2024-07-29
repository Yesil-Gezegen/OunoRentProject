namespace EntityLayer;

public abstract class AuditTrailer
{
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? ModifiedDateTime { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
}