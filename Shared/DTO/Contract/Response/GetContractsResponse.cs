namespace Shared.DTO.Contract.Response;

public class GetContractsResponse
{
    public Guid ContractId { get; set; }

    public string Name { get; set; }

    public double Version { get; set; }

    public double PreviousVersion { get; set; }

    public string Body { get; set; }

    public int Type { get; set; }

    public DateTime CreateDate { get; set; }

    public string RequiresAt { get; set; }

    public Boolean IsActive { get; set; }
}