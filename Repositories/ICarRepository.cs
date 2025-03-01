using Car_Rental_System.Models;
using X.PagedList;
namespace Car_Rental_System.Repositories;
public interface ICarRepository
{
    Task<IPagedList<CarViewModel>> GetAllAsync(string searchString, int pageSize, int pageNumber);
    Task<IEnumerable<CarViewModel>> GetAllAsync();
    Task<IEnumerable<CarViewModel>> GetAllAsync(bool status);
    Task<IEnumerable<CarViewModel>> GetCarByQuantityAsync(int quantity);
    Task<CarViewModel> GetByIdAsync(int id);
    Task UpdateAsync(CarViewModel car);
    Task AddAsync(CarViewModel car);
    Task DeleteAsync(int id);
}
