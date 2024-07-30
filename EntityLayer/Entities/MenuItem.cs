using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities;

public class MenuItem : AuditTrailer
{
    [Key]
    public Guid MenuItemId { get; set; }
    public string Label { get; set; }
    public string TargetUrl { get; set; }
    public int OrderNumber { get; set; }
    public bool OnlyToMembers { get; set; }
    public bool IsActive { get; set; }
}
