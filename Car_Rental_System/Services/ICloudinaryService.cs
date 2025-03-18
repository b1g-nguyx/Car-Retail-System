namespace Car_Rental_System.Services;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<string> UploadPdfAsync(IFormFile file);


}
