using Car_Rental_System.Models;
using Car_Rental_System.ViewModels;
using X.PagedList;

public interface IContractRepository
{
    Task<IPagedList<Contract>> GetAllAsync(string searchString, int pageSize,int pageNumber);
    Task<Contract> GetByIdAsync(int id);
    Task<Contract> GetByIdAsync(int id, string check);
    Task UpdateAsync(Contract contract);
    Task<IEnumerable<Contract>> GetContractsInPartByUserIdAsync(string userId);
    Task<IEnumerable<Contract>> GetCurrentContractByUserIdAsync(string userId);
    Task AddAsync(Contract contract);
    Task DeleteAsync(int id);
}