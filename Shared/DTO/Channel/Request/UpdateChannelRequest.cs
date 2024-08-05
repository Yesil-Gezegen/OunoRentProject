using Microsoft.AspNetCore.Http;

namespace Shared.DTO.Channel.Request;

public sealed record UpdateChannelRequest(Guid ChannelId, string Name, IFormFile? Logo, Boolean IsActive);
