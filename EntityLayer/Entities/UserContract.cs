using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities;

public class UserContract : AuditTrailer
{
    [Key]
    public Guid UserContractId { get; set; }
    public string FileName { get; set; }
    public DateTime Date { get; set; }

    public Guid UserId { get; set; }
    public Guid ContractId { get; set; }
    
    public User User { get; set; }
    public Contract Contract { get; set; }
}

