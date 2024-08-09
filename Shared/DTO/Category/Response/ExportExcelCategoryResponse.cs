namespace Shared.DTO.Category.Response;

public class ExportExcelCategoryResponse
{
        public string Name { get; set; }

        public int OrderNumber { get; set; }

        public List<SubCategory> SubCategories { get; set; }
        
        public class SubCategory
        {
            public string Name { get; set; }
    
            public string Description { get; set; }
            
            public int OrderNumber { get; set; }
    
        }
    }