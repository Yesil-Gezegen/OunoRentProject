using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities;

public class FAQ : AuditTrailer
{
    [Key]
    public Guid FAQId { get; set; }
    public string Label { get; set; }
    public string Text { get; set; }
    public int OrderNumber { get; set; }
    public bool IsActive { get; set; }
}