namespace Shared.DTO.Category.Response;

public class GetCategoriesResponse 
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; }

    public int OrderNumber { get; set; }

    public List<SubCategory> SubCategories { get; set; }
    
    public class SubCategory
    {
        public Guid SubCategoryId { get; set; }
    
        public string Name { get; set; }
    
        public string Description { get; set; }
    
        public string Icon { get; set; }
    
        public int OrderNumber { get; set; }
    
        public Boolean IsActive { get; set; }
    }
}