namespace Shared.DTO.Category.Response;

public class GetCategoryResponse
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Icon { get; set; }

    public int OrderNumber { get; set; }

    public string ImageHorizontalUrl { get; set; }

    public string ImageSquareUrl { get; set; }

    public Boolean IsActive { get; set; }

}