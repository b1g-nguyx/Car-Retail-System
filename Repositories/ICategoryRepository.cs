using Car_Rental_System.Models;

namespace Car_Rental_System.Repositories;

public interface ICategoryRepository
{
  Task<(IEnumerable<Category> Items, int TotalCount)> GetAllAsync(string searchString, int pageSize, int pageNumber, int status);
  Task<IEnumerable<Category>> GetAllAsync();
  Task<IEnumerable<Category>> GetAllAsync(bool status);
  Task<Category> GetByIdAsync(int id);
  Task UpdateAsync(Category category);
  Task AddAsync(Category category);
  Task DeleteAsync(int id);
}