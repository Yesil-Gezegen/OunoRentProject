namespace Shared.DTO.Blog.Request;

public class CreateBlogRequest
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string LargeImageUrl { get; set; }
    public string SmallImageUrl { get; set; }
    public string Tags { get; set; }
    public string Slug { get; set; }
    public int OrderNumber { get; set; }
    public Guid SubCategoryId { get; set; }
}
