using Microsoft.AspNetCore.Http;

namespace Shared.DTO.Channel.Request;

public sealed record CreateChannelRequest(string Name, IFormFile Logo , Boolean IsActive );