using Car_Rental_System.Models;
using X.PagedList.Extensions;

namespace Car_Rental_System.Repositories;

public class CategoryRepository : ICategoryRepository
{

    private readonly UnitOfWork _unitOfWork;

    public CategoryRepository(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task AddAsync(Category category)
    {
        await _unitOfWork._categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _unitOfWork._categoryRepository.GetByIdAsync(id);

        if (item != null)
        {
            _unitOfWork._categoryRepository.Remove(item);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Category> Items, int TotalCount)> GetAllAsync(string searchString, int pageSize, int pageNumber, int status)
    {
        var result = await _unitOfWork._categoryRepository.GetAllAsync();

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
        if (pageSize < 0)
        {
            pageSize = 1;
            var pageResultAll = result.ToPagedList(pageNumber, pageSize);
            return (result, pageResultAll.TotalItemCount);
        }
        var pageResult = result.ToPagedList(pageNumber, pageSize);
        return (pageResult, pageResult.TotalItemCount);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _unitOfWork._categoryRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Category>> GetAllAsync(bool status)
    {
        var result = await _unitOfWork._categoryRepository.GetAllAsync();
        result = result.Where(c => c.Status == status);
        return result;
    }

    public async Task<Category> GetByIdAsync(int id)
    {
        return await _unitOfWork._categoryRepository.GetByIdAsync(id);
    }

    public async Task UpdateAsync(Category category)
    {

        _unitOfWork._categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync();
    }
}