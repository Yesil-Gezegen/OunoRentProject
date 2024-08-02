namespace EntityLayer.Entities;

public class Channel : AuditTrailer
{
    public Guid ChannelId { get; set; }

    public string Name { get; set; }

    public string Logo { get; set; }

    public Boolean IsActive { get; set; }
}