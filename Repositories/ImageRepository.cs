using Car_Rental_System.Models;
using Car_Rental_System.Services;

namespace Car_Rental_System.Repositories;
public class ImageRepository : IImageRepository
{

    private readonly UnitOfWork _unitOfWork;
    private readonly ICloudinaryService _icloudinaryService;
    public ImageRepository(UnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
    {
        _icloudinaryService = cloudinaryService;
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(List<IFormFile> files, object relationID, string relationName)
    {

        List<Images> newImages = new List<Images>();


        foreach (var file in files)
        {
            var image = new Images
            {
                RelationId = relationID.ToString()!,
                NameRelation = relationName,
                ImageUrl = await _icloudinaryService.UploadImageAsync(file),
            };
            newImages.Add(image);
        }
        _unitOfWork._imageRepository.AddRange(newImages);
        await _unitOfWork.SaveChangesAsync();

    }
    public async Task DeleteAsync(int id)
    {
        var item = await _unitOfWork._imageRepository.GetByIdAsync(id);

        _unitOfWork._imageRepository.Remove(item!);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(object relationId, string relationName)
    {
        var items = await _unitOfWork._imageRepository.GetManyAsync(c => c.RelationId.Equals(relationId.ToString()) && c.NameRelation.Equals(relationName));
        _unitOfWork._imageRepository.RemoveRange(items);
        await _unitOfWork.SaveChangesAsync();
    }



    public async Task<IEnumerable<Images>> GetAllAsync()
    {
        return await _unitOfWork._imageRepository.GetAllAsync();
    }
    public async Task<IEnumerable<Images>> GetAllByIdRelationId(object relationId, string relationName)
    {
        var result = await _unitOfWork._imageRepository.GetManyAsync(c => c.RelationId == relationId.ToString() && c.NameRelation == relationName);
        return result;
    }
    public async Task<Images> GetByIdAsync(int id)
    {
        var item = await _unitOfWork._imageRepository.GetByIdAsync(id);
        return item!;
    }
}