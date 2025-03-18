
using Car_Rental_System.Utils;
using Car_Rental_System.Models;
using Car_Rental_System.Repositories;
using Car_Rental_System.ViewModels;
using X.PagedList;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Car_Rental_System.Repositories;
public class ContractRepository : IContractRepository
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IImageRepository _iimageRepository;
    public ContractRepository(UnitOfWork unitOfWork, IImageRepository imageRepository)
    {
        _iimageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task AddAsync(Contract contract)
    {
        await _unitOfWork._contractRepository.AddAsync(contract);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _unitOfWork._contractRepository.GetByIdAsync(id);
        _unitOfWork._contractRepository.Remove(item);
        await _unitOfWork.SaveChangesAsync();
        await _iimageRepository.DeleteAsync(item.Id, Constant.contract);
    }

    public async Task<Contract> GetByIdAsync(int id)
    {
        var contract = await _unitOfWork._contractRepository.GetByIdAsync(id);
       return contract!;
    }

    public async Task UpdateAsync(Contract contract)
    {
        _unitOfWork._contractRepository.Update(contract);
        await _unitOfWork.SaveChangesAsync();
    }
  

    public async Task<IEnumerable<Contract>> GetContractsInPartByUserIdAsync(string userId)
    {
        var result = await _unitOfWork._contractRepository.GetManyAsync(c => c.UserId == userId && c.Status == Contracts.Signature);

        return result;

    }

    public async Task<IEnumerable<Contract>> GetCurrentContractByUserIdAsync(string userId)
    {
        var result = await _unitOfWork._contractRepository.GetManyAsync(c => c.UserId == userId && c.Status == Contracts.Pending);
        return result;
    }

    public async Task<IPagedList<Contract>> GetAllAsync(string searchString, int pageSize, int pageNumber)
    {
        Expression<Func<Contract, bool>> filter = PredicateBuilder.New<Contract>(true);

        if (!String.IsNullOrEmpty(searchString))
        {
            filter = filter.And(c => c.Status!.ToUpper().Contains(searchString.ToUpper()));
            filter = filter.Or(c => c.Email!.ToUpper().Contains(searchString.ToUpper()));
            filter = filter.Or(c => c.Address!.ToUpper().Contains(searchString.ToUpper()));
            filter = filter.Or(c => c.FullName!.ToUpper().Contains(searchString.ToUpper()));
            filter = filter.Or(c => c.PhoneNumber!.ToUpper().Contains(searchString.ToUpper()));
        }

        var totalCount = await _unitOfWork._contractRepository.GetQueryable().Where(filter).CountAsync();
        var query = _unitOfWork._contractRepository.GetQueryable(filter, allowTracking: true)
                         .OrderBy(c => c.Id)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

        var result = await query.ToListAsync();
       
        return new StaticPagedList<Contract>(result, pageNumber, pageSize, totalCount);
    }

    public async Task<Contract> GetByIdAsync(int id, string check)
    {
       var model = await _unitOfWork._contractRepository.GetByIdAsync(id);

       return model!;
    }
}