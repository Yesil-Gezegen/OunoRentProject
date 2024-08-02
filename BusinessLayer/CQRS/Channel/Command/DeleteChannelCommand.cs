using MediatR;
using Shared.Interface;

namespace BusinessLayer.CQRS.Channel.Command;

public sealed record DeleteChannelCommand(Guid ChannelId) : IRequest<Guid>
{
    internal class DeleteCommandHandler : IRequestHandler<DeleteChannelCommand, Guid>
    {
        private readonly IChannelRepository _channelRepository;

        public DeleteCommandHandler(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<Guid> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
        {
            return await _channelRepository.DeleteChannel(request.ChannelId);
        }
    }
}