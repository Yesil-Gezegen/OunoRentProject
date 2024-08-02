using Microsoft.AspNetCore.Http;

namespace Shared.Interface;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile image);
    Task DeleteImageAsync(string imageUrl);
}