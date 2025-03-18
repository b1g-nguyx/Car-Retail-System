using System.Linq.Expressions;
using System.Threading.Tasks;
using Car_Rental_System.Models;
using LinqKit;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using Car_Rental_System.Utils;

namespace Car_Rental_System.Repositories;
public class CarRentalRepository : ICarRentalRepository
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IImageRepository _iimageRepository;
    public CarRentalRepository(UnitOfWork unitOfWork, IImageRepository imageRepository)
    {
        _iimageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(CarRental carRental)
    {

        await _unitOfWork._carRetailRepository.AddAsync(carRental);
        await _unitOfWork.SaveChangesAsync();
    }


    public async Task DeleteAsync(int id)
    {
        var item = await _unitOfWork._carRetailRepository.GetByIdAsync(id);
        if (item != null)
        {
            _unitOfWork._carRetailRepository.Remove(item!);
            await _unitOfWork.SaveChangesAsync();
        }
        await _iimageRepository.DeleteAsync(id, Constant.carRental);
    }

    public async Task<IPagedList<CarRental>> GetAllAsync(string searchString, int pageSize, int pageNumber)
    {
        Expression<Func<CarRental, bool>> filter = PredicateBuilder.New<CarRental>(true);

        if (!String.IsNullOrEmpty(searchString))
        {
            filter = filter.And(c => c.TotalPrice.ToString()!.ToUpper().Contains(searchString.ToUpper()));
        }

        var totalCount = await _unitOfWork._carRetailRepository.GetQueryable().Where(filter).CountAsync();
        var query = _unitOfWork._carRetailRepository.GetQueryable(filter, allowTracking: true)
                         .OrderBy(c => c.Id)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

        var result = await query.ToListAsync();

        return new StaticPagedList<CarRental>(result, pageNumber, pageSize, totalCount);
    }

    public async Task<CarRental> GetByIdAsync(int id)
    {
        var item = await _unitOfWork._carRetailRepository.GetByIdAsync(id);
        return item!;
    }

    public async Task UpdateAsync(CarRental carRental)
    {
        _unitOfWork._carRetailRepository.Update(carRental);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<CarRental> GetByDateCreateAtTotalUserIdAsync(string userId, DateTime StartDate, DateTime EndDate, DateTime CreateAt, decimal TotalPrice)
    {
        Expression<Func<CarRental, bool>> filter = PredicateBuilder.New<CarRental>(true);
        filter = filter.And(c => c.CreatedAt == CreateAt
                             && c.EndDate == EndDate
                             && c.StartDate == StartDate
                             && c.TotalPrice == TotalPrice
                             && c.UserId!.Contains(userId));

        var query = await _unitOfWork._carRetailRepository.FindAsync(filter, allowTracking: true);
        return query!;
    }

    public async Task<CarRental> GetByIdContractAsync(int id)
    {
        var item = await _unitOfWork._carRetailRepository.FindAsync(c => c.ContractId == id);
        return item!;
    }
}