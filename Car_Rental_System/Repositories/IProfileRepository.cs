using Car_Rental_System.Models;
using X.PagedList;

public interface IProfileRepository
{
    Task<Profile> GetByUserIdAsync(string userId);
    Task UpdateAsync(Profile profile);

    Task AddSysnc(Profile profile);

    Task<IPagedList<Profile>> GetAllAsync(string searchString, int pageNumber, int pageSize);
}