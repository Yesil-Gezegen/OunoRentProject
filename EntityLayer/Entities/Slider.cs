namespace EntityLayer.Entities;

public class Slider : AuditTrailer
{
    public Guid SliderId { get; set; }

    public string MainImageUrl { get; set; }

    public string MobileImageUrl { get; set; }

    public string TargetUrl { get; set; }

    public string Title { get; set; }

    public int OrderNumber { get; set; }

    public int Duration { get; set; }

    public DateTime ActiveFrom { get; set; }

    public DateTime ActiveTo { get; set; }

    public bool IsActive { get; set; }
}
