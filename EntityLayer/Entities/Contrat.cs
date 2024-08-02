namespace EntityLayer.Entities;

public class Contract : AuditTrailer
{
    public Guid ContractId { get; set; }

    public string Name { get; set; }

    public int Version { get; set; }

    public int PreviousVersion { get; set; }

    public string Body { get; set; }

    public int Type { get; set; }

    public DateTime CreateDate { get; set; }

    public string RequiresAt { get; set; }

    public Boolean IsActive { get; set; }
}