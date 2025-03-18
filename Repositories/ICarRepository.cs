using Car_Rental_System.Models;
using Car_Rental_System.ViewModels;
using X.PagedList;
namespace Car_Rental_System.Repositories;
public interface ICarRepository
{
    Task<IPagedList<CarViewModel>> GetAllAsync(string searchString, int pageNumber, int pageSize);
    Task<IPagedList<CarViewModel>> GetAllAsync(string searchString, int PageNumber, int PageSize, string Sort, decimal MinPrice, decimal MaxPrice, List<int> BrandIds, int categoryId,DateTime StartDate, DateTime EndDate);
    Task<IEnumerable<CarViewModel>> GetAllAsync();
    Task<IEnumerable<CarViewModel>> GetAllAsync(int status);
    Task<IEnumerable<CarViewModel>> GetByTopAsync();
    Task<CarViewModel> GetByIdAsync(int id);
    Task UpdateAsync(CarViewModel car);
    Task AddAsync(CarViewModel car);
    Task DeleteAsync(int id);
}
