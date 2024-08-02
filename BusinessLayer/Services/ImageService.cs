using BusinessLayer.Middlewares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Shared.Interface;

namespace BusinessLayer.Services;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ImageService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    #region SaveImage

    public async Task<string> SaveImageAsync(IFormFile image)
    {
        if (image == null || image.Length == 0)
            throw new BadRequestException(ImageExceptionMessages.InvalidImage);

        if (_webHostEnvironment.WebRootPath == null)
            throw new BadRequestException(ImageExceptionMessages.InvalidPath);

        if (string.IsNullOrEmpty(image.FileName))
            throw new BadRequestException(ImageExceptionMessages.InvalidName);

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
    
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        // Relative path'i döndür (sadece "uploads" klasöründen itibaren)
        var relativePath = Path.Combine("uploads", uniqueFileName);
        
        return relativePath;
    }

    #endregion

    #region DeleteImage

    public async Task DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return;
        
        var fileName = Path.GetFileName(imageUrl);
        
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

        // Dosya yolunu kontrol ve loglama
        if (!File.Exists(filePath))
        {
            throw new NotFoundException(ImageExceptionMessages.NotFound);
        }

        try
        {
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            throw new InternalServerError(ImageExceptionMessages.Error);
        }
    }

    #endregion
}