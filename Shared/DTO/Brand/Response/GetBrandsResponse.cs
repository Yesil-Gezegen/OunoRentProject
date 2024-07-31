namespace Shared.DTO.Brand.Response;

public class GetBrandsResponse
{
    public Guid BrandId { get; set; }

    public string Name { get; set; }

    public string Logo { get; set; }

    public Boolean ShowOnBrands { get; set; }

    public Boolean IsActive { get; set; }
}