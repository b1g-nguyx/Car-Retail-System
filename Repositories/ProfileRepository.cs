
using Car_Rental_System.Data;
using Car_Rental_System.Models;
using Car_Rental_System.Repositories;
using Microsoft.EntityFrameworkCore;

public class ProfileRepository : IProfileRepository
{
    private readonly UnitOfWork _unitOfWork;

    public ProfileRepository(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddSysnc(Profile profile)
    {
       await _unitOfWork._profileRepository.AddAsync(profile);
    }

    public async Task<Profile> GetByUserIdAsync(string userId)
    {
        var profile = await _unitOfWork._profileRepository.GetByIdAsync(userId);
        return profile!;
    }

    public async Task UpdateAsync(Profile profile)
    {
        _unitOfWork._profileRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync();
    }
}