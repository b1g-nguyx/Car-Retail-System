using Car_Rental_System.Models;

namespace Car_Rental_System.Repositories;
public interface IImageRepository
    {
        Task<IEnumerable<Images>> GetAllAsync();
        Task<Images> GetByIdAsync(int id);
        Task AddAsync(List<IFormFile> files, object relationId, string relationName);
        Task DeleteAsync(int id);
        Task DeleteAsync(object relationId, string relationName);
        Task<IEnumerable<Images>> GetAllByIdRelationId(object relationId, string relationName);

    }