using Car_Rental_System.Models;
using Car_Rental_System.ViewModels;
using X.PagedList;

namespace Car_Rental_System.Repositories;

public interface ICategoryRepository
{
  Task<IPagedList<CategoryViewModel>> GetAllAsync(string searchString, int pageSize, int pageNumber);
  Task<IEnumerable<CategoryViewModel>> GetAllAsync();
  Task<IEnumerable<Category>> GetAllForListAsync(bool IsActive);
  Task<CategoryViewModel> GetByIdAsync(int id);
  Task UpdateAsync(CategoryViewModel categoryViewModel);
  Task AddAsync(CategoryViewModel categoryViewModel);
  Task DeleteAsync(int id);
}