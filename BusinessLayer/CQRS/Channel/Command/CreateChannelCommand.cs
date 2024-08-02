using MediatR;
using Shared.DTO.Channel.Request;
using Shared.DTO.Channel.Response;
using Shared.Interface;

namespace BusinessLayer.CQRS.Channel.Command;

public sealed record CreateChannelCommand(CreateChannelRequest CreateChannelRequest) : IRequest<ChannelResponse>
{
     internal class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, ChannelResponse>
     {
          private readonly IChannelRepository _channelRepository;

          public CreateChannelCommandHandler(IChannelRepository channelRepository)
          {
               _channelRepository = channelRepository;
          }

          public async Task<ChannelResponse> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
          {
               return await _channelRepository.CreateChannel(request.CreateChannelRequest);
          }
     }
}