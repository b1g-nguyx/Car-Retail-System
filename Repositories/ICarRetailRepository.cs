using Car_Rental_System.Models;
using X.PagedList;

public interface ICarRentalRepository
{
    Task DeleteAsync(int id);
    Task<IPagedList<CarRental>> GetAllAsync(string searchString, int pageSize, int pageNumber);
    Task<CarRental> GetByIdAsync(int id);
    Task<CarRental> GetByDateCreateAtTotalUserIdAsync(string userId, DateTime StartDate, DateTime EndDate, DateTime CreateAt, decimal TotalPrice);
    Task UpdateAsync(CarRental carRetail);
    Task AddAsync(CarRental carRetail);
}
