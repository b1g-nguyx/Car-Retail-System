
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Car_Rental_System.Services;
public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudName = configuration["CloudinarySettings:CloudName"];
        var apiKey = configuration["CloudinarySettings:ApiKey"];
        var apiSecret = configuration["CloudinarySettings:ApiSecret"];

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File không hợp lệ.");

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return uploadResult.Url.ToString();
    }


   public async Task<string> UploadPdfAsync(IFormFile file)
{
    if (file == null || file.Length == 0)
        return null;

    using var stream = file.OpenReadStream();
    var uploadParams = new RawUploadParams
    {
        File = new FileDescription(file.FileName, stream),
        PublicId = $"pdfs/{Guid.NewGuid()}"
    };

    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
    return uploadResult.SecureUrl.AbsoluteUri;
}


}
