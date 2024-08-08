using MediatR;
using Shared.DTO.Channel.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Channel.Query;

public sealed record GetActiveChannelsQuery() : IRequest<List<GetChannelsResponse>>
{
    internal class GetActiveChannelsQueryHandler : IRequestHandler<GetActiveChannelsQuery, List<GetChannelsResponse>>
    {
        private readonly IChannelRepository _channelRepository;

        public GetActiveChannelsQueryHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<List<GetChannelsResponse>> Handle(
            GetActiveChannelsQuery request, CancellationToken cancellationToken)
        {
            return await _channelRepository.GetChannels(x => x.IsActive);
        }
    }
}