using Car_Rental_System.Models;

public interface IProfileRepository
{
    Task<Profile> GetByUserIdAsync(string userId);
    Task UpdateAsync(Profile profile);

    Task AddSysnc(Profile profile);
}