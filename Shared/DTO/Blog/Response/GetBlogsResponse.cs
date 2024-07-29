namespace Shared.DTO.Blog.Response;

public class GetBlogsResponse 
{
    public Guid BlogId { get; set; }
    public Guid SubCategoryId { get; set; }
    public string SubCategoryName { get; set; }
    public string Title { get; set; }
    public string Tags { get; set; }
    public string Slug { get; set; }
    public int OrderNumber { get; set; }
    public DateTime Date { get; set; }
    public string LargeImageUrl { get; set; }
    public string SmallImageUrl { get; set; }
    public bool IsActive { get; set; }
}
