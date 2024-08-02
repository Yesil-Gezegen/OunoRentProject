namespace Shared.DTO.Channel.Response;

public class GetChannelResponse
{
    public Guid ChannelId { get; set; }

    public string Name { get; set; }

    public string Logo { get; set; }

    public Boolean IsActive { get; set; }
}