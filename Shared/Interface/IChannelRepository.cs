using Shared.DTO.Channel.Request;
using Shared.DTO.Channel.Response;

namespace Shared.Interface;

public interface IChannelRepository
{
    Task<ChannelResponse> CreateChannel(CreateChannelRequest createChannelRequest);
    
    Task<List<GetChannelsResponse>> GetChannels();

    Task<GetChannelResponse> GetChannel(Guid channelId);
    
    Task<ChannelResponse> UpdateChannel(UpdateChannelRequest updateChannelRequest);
    
    Task<Guid> DeleteChannel(Guid channelId);
}