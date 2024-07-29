using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EntityLayer.Entities;

public class SubCategory : AuditTrailer
{
    [Key]
    public Guid SubCategoryId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Icon { get; set; }

    public int OrderNumber { get; set; }

    public Boolean IsActive { get; set; }

    // Relationships

    public Guid CategoryId { get; set; }

    public Category Category { get; set; }
}
