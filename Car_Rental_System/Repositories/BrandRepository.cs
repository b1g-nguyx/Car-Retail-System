using System.Linq;
using System.Linq.Expressions;
using Car_Rental_System.Models;
using LinqKit;
using X.PagedList.Extensions;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using Car_Rental_System.ViewModels;
using System.Threading.Tasks;
using Car_Rental_System.Utils;
namespace Car_Rental_System.Repositories;

public class BrandRepository : IBrandRepository
{

    private readonly UnitOfWork _unitOfWork;
    private readonly IImageRepository _iimageRepository;
    public BrandRepository(UnitOfWork unitOfWork, IImageRepository imageRepository)
    {
        _iimageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(BrandViewModel brandViewModel)
    {
        Brand brand = ConvertToModel(brandViewModel);
        await _unitOfWork._brandRepository.AddAsync(brand);
        await _unitOfWork.SaveChangesAsync();
        await _iimageRepository.AddAsync(brandViewModel.Files, brand.Id, Constant.brand);
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _unitOfWork._brandRepository.GetByIdAsync(id);
        if (item != null)
        {
            _unitOfWork._brandRepository.Remove(item);
        }
        await _unitOfWork.SaveChangesAsync();
        await _iimageRepository.DeleteAsync(id, Constant.brand);

    }
    public async Task<IEnumerable<BrandViewModel>> GetAllAsync()
    {
        var result = await _unitOfWork._brandRepository.GetAllAsync();
        var brandViewModel = new List<BrandViewModel>();
        foreach (var item in result)
        {
            var brand = await ConvertToViewModel(item);
            brandViewModel.Add(brand);
        }
        return brandViewModel;
    }
    public async Task<IPagedList<BrandViewModel>> GetAllAsync(string searchString, int pageSize, int pageNumber)
    {
        Expression<Func<Brand, bool>> filter = PredicateBuilder.New<Brand>(true);

        if (!String.IsNullOrEmpty(searchString))
        {
            filter = filter.And(c => c.Name!.ToUpper().Contains(searchString.ToUpper()));
        }

        var totalCount = await _unitOfWork._brandRepository.GetQueryable().Where(filter).CountAsync();
        var query = _unitOfWork._brandRepository.GetQueryable(filter, allowTracking: true)
                         .OrderBy(c => c.Id)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

        var result = await query.ToListAsync();
        List<BrandViewModel> brandViewModel = new List<BrandViewModel>();

        foreach (var item in result)
        {
            var brand = await ConvertToViewModel(item);
            brandViewModel.Add(brand);
        }
        return new StaticPagedList<BrandViewModel>(brandViewModel, pageNumber, pageSize, totalCount);
    }

    public async Task<BrandViewModel> GetByIdAsync(int id)
    {
        var brand = await _unitOfWork._brandRepository.GetByIdAsync(id);
        var brandViewModel = await ConvertToViewModel(brand!);
        return brandViewModel;
    }

    public async Task UpdateAsync(BrandViewModel brandViewModel)
    {

        Brand brand = ConvertToModel(brandViewModel);
        _unitOfWork._brandRepository.Update(brand);
        await _unitOfWork.SaveChangesAsync();
        await _iimageRepository.AddAsync(brandViewModel.Files, brand.Id, Constant.brand);
    }

    public async Task<BrandViewModel> ConvertToViewModel(Brand brand)
    {
        var brandViewModel = new BrandViewModel
        {
            Brand = brand,
            Images = await _iimageRepository.GetAllByIdRelationId(brand.Id, Constant.brand)
        };

        return brandViewModel;
    }
    public Brand ConvertToModel(BrandViewModel brandViewModel)
    {
        var brand = brandViewModel.Brand;
        return brand;
    }

    public Task<IEnumerable<Brand>> GetAllForListAsync(bool IsActive)
    {
        return _unitOfWork._brandRepository.GetManyAsync(c => c.IsActive == IsActive);
    }
}