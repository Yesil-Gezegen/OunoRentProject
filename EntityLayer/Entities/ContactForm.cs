using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities;

public class ContactForm : AuditTrailer
{
    [Key]
    public Guid ContactFormId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string SubjectCategory { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }

    public DateTime FormDate { get; set; }
}