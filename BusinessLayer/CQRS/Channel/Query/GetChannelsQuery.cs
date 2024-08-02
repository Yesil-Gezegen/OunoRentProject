using MediatR;
using Shared.DTO.Channel.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Channel.Query;

public sealed record GetChannelsQuery : IRequest<List<GetChannelsResponse>>
{
    internal class GetChannelsQueryHandler : IRequestHandler<GetChannelsQuery, List<GetChannelsResponse>>
    {
        private readonly IChannelRepository _channelRepository;

        public GetChannelsQueryHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<List<GetChannelsResponse>> Handle(GetChannelsQuery request, CancellationToken cancellationToken)
        {
            return await _channelRepository.GetChannels();
        }
    }
}