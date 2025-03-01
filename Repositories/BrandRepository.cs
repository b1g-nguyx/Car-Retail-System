using Car_Rental_System.Models;
using X.PagedList.Extensions;

namespace Car_Rental_System.Repositories;

public class BrandRepository : IBrandRepository
{

    private readonly UnitOfWork _unitOfWork;

    public BrandRepository(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task AddAsync(Brand brand)
    {
        await _unitOfWork._brandRepository.AddAsync(brand);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _unitOfWork._brandRepository.GetByIdAsync(id);
        if (item != null)
        {
            _unitOfWork._brandRepository.Remove(item);
        }
        await _unitOfWork.SaveChangesAsync();

    }
    public async Task<IEnumerable<Brand>> GetAllAsync()
    {
        return await _unitOfWork._brandRepository.GetAllAsync();
    }
    public async Task<(IEnumerable<Brand> Items, int TotalCount)> GetAllAsync(string searchString, int pageSize, int pageNumber, int status)
    {
        var result = await _unitOfWork._brandRepository.GetAllAsync();

        if (pageSize != -1)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                if (status == -1)
                {
                    result = result.Where(c =>
                           c.Name!.ToUpper().Contains(searchString.ToUpper()));
                }
                else if (status == 0)
                {
                    result = result.Where(c =>
                    c.Name!.ToUpper().Contains(searchString.ToUpper()) &&
                    c.Status == false);
                }
                else
                {
                    result = result.Where(c =>
                    c.Name!.ToUpper().Contains(searchString.ToUpper()) &&
                    c.Status == true);
                }
                            ;

            }
        ;
        }
        ;




        if (pageSize < 0)
        {
            pageSize = 1;
            var pageResultAll = result.ToPagedList(pageNumber, pageSize);
            return (result, pageResultAll.TotalItemCount);
        }
        var pageResult = result.ToPagedList(pageNumber, pageSize);
        return (pageResult, pageResult.TotalItemCount);
    }

    public async Task<IEnumerable<Brand>> GetAllAsync(bool status)
    {
        var result = await _unitOfWork._brandRepository.GetAllAsync();
        result = result.Where(b =>
       b.Status == status);
        return result;
    }

    public async Task<Brand> GetByIdAsync(int id)
    {
        return await _unitOfWork._brandRepository.GetByIdAsync(id);
    }

    public async Task UpdateAsync(Brand brand)
    {

        _unitOfWork._brandRepository.Update(brand);
        await _unitOfWork.SaveChangesAsync();
    }
}