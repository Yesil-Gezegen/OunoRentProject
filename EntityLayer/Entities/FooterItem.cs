using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities;

public class FooterItem : AuditTrailer
{
    [Key]
    public Guid FooterItemId { get; set; }

    public string Label { get; set; }

    public int Column { get; set; }

    public int OrderNumber { get; set; }

    public string TargetUrl { get; set; }

    public Boolean IsActive { get; set; }
}