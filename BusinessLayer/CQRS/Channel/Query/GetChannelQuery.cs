using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Shared.DTO.Channel.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Channel.Query;

public sealed record GetChannelQuery(Guid ChannelId) : IRequest<GetChannelResponse>
{
    internal class GetChannelQueryHandler : IRequestHandler<GetChannelQuery, GetChannelResponse>
    {
        private readonly IChannelRepository _channelRepository;

        public GetChannelQueryHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<GetChannelResponse> Handle(GetChannelQuery request, CancellationToken cancellationToken)
        {
             return await _channelRepository.GetChannel(request.ChannelId);
        }
    }
}