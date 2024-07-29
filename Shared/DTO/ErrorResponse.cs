
namespace Shared.DTO;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
}
