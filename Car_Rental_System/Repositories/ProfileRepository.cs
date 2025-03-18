
using System.Linq.Expressions;
using Car_Rental_System.Data;
using Car_Rental_System.Models;
using Car_Rental_System.Repositories;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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

    public async Task<IPagedList<Profile>> GetAllAsync(string searchString, int pageNumber, int pageSize)
    {
         Expression<Func<Profile, bool>> filter = PredicateBuilder.New<Profile>(true);

        if (!String.IsNullOrEmpty(searchString))
        {
            filter = filter.And(c => c.FullName!.Contains(searchString));
            filter = filter.Or(c => c.Address!.ToUpper().Contains(searchString.ToUpper()));            
            filter = filter.Or(c => c.PhoneNumber!.ToUpper().Contains(searchString.ToUpper()));            
        }

        var totalCount = await _unitOfWork._profileRepository.GetQueryable().Where(filter).CountAsync();

        var query = _unitOfWork._profileRepository.GetQueryable(filter, allowTracking: true)
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize);

        var result = await query.ToListAsync();

       
        return new StaticPagedList<Profile>(result, pageNumber, pageSize, totalCount);
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