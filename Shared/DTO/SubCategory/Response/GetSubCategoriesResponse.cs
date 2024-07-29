using System.Security.AccessControl;

namespace Shared.DTO.SubCategory.Response;

public class GetSubCategoriesResponse 
{
    public Guid CategoryId { get; set; }    

    public Guid SubCategoryId { get; set; } 

    public string Name { get; set; }    
    
    public int OrderNumber { get; set; }
}
