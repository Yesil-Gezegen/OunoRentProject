using MediatR;
using Shared.DTO.Category.Request;
using Shared.DTO.Channel.Request;
using Shared.DTO.Channel.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Channel.Command;

public sealed record UpdateChannelCommand(UpdateChannelRequest UpdateChannelRequest) :IRequest<ChannelResponse>
{
    internal class UpdateChannelCommandHandler : IRequestHandler<UpdateChannelCommand, ChannelResponse>
    {
        private readonly IChannelRepository _channelRepository;

        public UpdateChannelCommandHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<ChannelResponse> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
        {
            return await _channelRepository.UpdateChannel(request.UpdateChannelRequest);
        }
    }
}