using Car_Rental_System.Models;

namespace Car_Rental_System.Repositories;

public interface IBrandRepository
{
    Task<(IEnumerable<Brand> Items, int TotalCount)> GetAllAsync(string searchString, int pageSize, int pageNumber, int status);
    Task<IEnumerable<Brand>> GetAllAsync();
    Task<IEnumerable<Brand>> GetAllAsync(bool status);

    Task<Brand> GetByIdAsync(int id);

    Task UpdateAsync(Brand brand);

    Task AddAsync(Brand brand);

    Task DeleteAsync(int id);
}