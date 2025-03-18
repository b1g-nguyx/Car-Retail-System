using Car_Rental_System.Models;
using Car_Rental_System.ViewModels;
using X.PagedList;

namespace Car_Rental_System.Repositories;

public interface IBrandRepository
{
    Task<IPagedList<BrandViewModel>> GetAllAsync(string searchString, int pageSize, int pageNumber );
    Task<IEnumerable<BrandViewModel>> GetAllAsync();
    Task<IEnumerable<Brand>> GetAllForListAsync(bool IsActive);
    Task<BrandViewModel> GetByIdAsync(int id);

    Task UpdateAsync(BrandViewModel brandViewModel);

    Task AddAsync(BrandViewModel brandViewModel);

    Task DeleteAsync(int id);
}