using System.Linq.Expressions;
using System.Threading.Tasks;
using Car_Rental_System.Models;
using Car_Rental_System.Repositories;
using Car_Rental_System.Utils;
using Car_Rental_System.ViewModels;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;
namespace Car_Rental_System.Repositories;

public class CarRepository : ICarRepository
{

    private readonly UnitOfWork _unitOfWork;
    private readonly IImageRepository _iimageRepository;
    private readonly ICategoryRepository _icategoryRepository;
    private readonly IBrandRepository _ibrandRepository;

    public CarRepository(UnitOfWork unitOfWork, IImageRepository imageRepository, ICategoryRepository categoryRepository, IBrandRepository brandRepository)
    {
        _icategoryRepository = categoryRepository;
        _ibrandRepository = brandRepository;
        _iimageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task AddAsync(CarViewModel carViewModel)
    {
        var car = CarConvert(carViewModel);
        await _unitOfWork._carRepository.AddAsync(car);
        await _unitOfWork.SaveChangesAsync();
        await _iimageRepository.AddAsync(carViewModel.files, car.Id, Constant.car);
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _unitOfWork._carRepository.GetByIdAsync(id);
        await _iimageRepository.DeleteAsync(item!.Id, Constant.car);
        _unitOfWork._carRepository.Remove(item);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IPagedList<CarViewModel>> GetAllAsync(string searchString, int pageNumber, int pageSize)
    {
        Expression<Func<Car, bool>> filter = PredicateBuilder.New<Car>(true);

        if (!String.IsNullOrEmpty(searchString))
        {
            filter = filter.And(c => c.Description!.Contains(searchString));
            filter = filter.Or(c => c.Model!.Contains(searchString));
            filter = filter.Or(c => c.Brand != null && c.Brand.Name.Contains(searchString));
            filter = filter.Or(c => c.Category != null && c.Category.Name.Contains(searchString));
            filter = filter.Or(c => c.FuelType!.Contains(searchString));
            filter = filter.Or(c => c.Transmission!.Contains(searchString));
        }

        if (decimal.TryParse(searchString, out decimal price))
        {
            filter = filter.Or(c => c.PricePerDay == price);
        }

        if (int.TryParse(searchString, out int year))
        {
            filter = filter.Or(c => c.Year >= year);
        }
        var totalCount = await _unitOfWork._carRepository.GetQueryable().Where(filter).CountAsync();

        var query = _unitOfWork._carRepository.GetQueryable(filter, allowTracking: true)
                  .OrderBy(c => c.Model)
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize);

        var result = await query.ToListAsync();

        List<CarViewModel> carViewModels = new List<CarViewModel>();

        foreach (var item in result)
        {
            var car = await ViewModelConvertToModel(item);
            carViewModels.Add(car);
        }
        return new StaticPagedList<CarViewModel>(carViewModels, pageNumber, pageSize, totalCount);
    }

    public async Task<IEnumerable<CarViewModel>> GetAllAsync()
    {
        IEnumerable<Car> cars = await _unitOfWork._carRepository.GetAllAsync();
        List<CarViewModel> carViewModels = new List<CarViewModel>();

        foreach (var item in cars)
        {
            var car = await ViewModelConvertToModel(item);
            carViewModels.Add(car);
        }
        return carViewModels;
    }

    public async Task<IEnumerable<CarViewModel>> GetAllAsync(int status)
    {
        IEnumerable<Car> cars = await _unitOfWork._carRepository.GetManyAsync(c => (int)c.Status == status);
        List<CarViewModel> carViewModels = new List<CarViewModel>();

        foreach (var item in cars)
        {
            var car = await ViewModelConvertToModel(item);
            carViewModels.Add(car);
        }
        return carViewModels;
    }

    public async Task<CarViewModel> GetByIdAsync(int id)
    {
        var car = await _unitOfWork._carRepository.GetByIdAsync(id);

        var carViewModel = await ViewModelConvertToModel(car!);
        return carViewModel;
    }

    public async Task UpdateAsync(CarViewModel car)
    {
        _unitOfWork._carRepository.Update(CarConvert(car));
        await _unitOfWork.SaveChangesAsync();
        await _iimageRepository.AddAsync(car.files, car.Car.Id, Constant.car);


    }

    public Car CarConvert(CarViewModel carViewModel)
    {
        var car = carViewModel.Car;
        return car;
    }
    public async Task<CarViewModel> ViewModelConvertToModel(Car car)
    {


        var carViewModel = new CarViewModel
        {
            Car = car

        };
        var Category = await _icategoryRepository.GetByIdAsync(car.CategoryId);
        var Brand = await _ibrandRepository.GetByIdAsync(car.BrandId);
        carViewModel.Car.Category = Category.Category;
        carViewModel.Car.Brand = Brand.Brand;
        carViewModel.Images = await _iimageRepository.GetAllByIdRelationId(car!.Id, Constant.car);

        return carViewModel;
    }

    public async Task<IEnumerable<CarViewModel>> GetByTopAsync()
    {
        var query = _unitOfWork._carRepository.GetQueryable(c => (int)c.Status == 0, allowTracking: true).Take(6);
        var result = await query.ToListAsync();
        List<CarViewModel> carViewModels = new List<CarViewModel>();

        foreach (var item in result)
        {
            var car = await ViewModelConvertToModel(item);
            carViewModels.Add(car);
        }
        return carViewModels;
    }
    public async Task<IPagedList<CarViewModel>> GetAllAsync(string searchString, int PageNumber, int PageSize, string Sort, decimal MinPrice, decimal MaxPrice, List<int> BrandIds, int categoryId, DateTime StartDate, DateTime EndDate)
    {
        Expression<Func<Car, bool>> filter = PredicateBuilder.New<Car>(true);

        if (!string.IsNullOrEmpty(searchString))
        {
            filter = filter.And(c => c.Description!.Contains(searchString)
                                  || c.Model!.Contains(searchString)
                                  || c.FuelType!.Contains(searchString));
        }

        if (int.TryParse(searchString, out int year))
        {
            filter = filter.Or(c => c.Year >= year);
        }

        if (MinPrice > 0 || (MaxPrice > 0 && MaxPrice >= MinPrice))
        {
            filter = filter.And(c => c.PricePerDay >= MinPrice && c.PricePerDay <= (MaxPrice > 0 ? MaxPrice : decimal.MaxValue));
        }

        if (BrandIds != null && BrandIds.Any())
        {
            filter = filter.And(c => BrandIds.Contains(c.BrandId));
        }

        if (categoryId > 0)
        {
            filter = filter.And(c => c.CategoryId == categoryId);
        }

        
        var rentedCarIds = _unitOfWork._carRetailRepository
            .GetQueryable(crd => crd.StartDate <= EndDate && crd.EndDate >= StartDate, allowTracking: false)
            .Select(crd => crd.CarId)
            .Distinct(); // Loại bỏ trùng lặp

        filter = filter.And(c => !rentedCarIds.Contains(c.Id));

        var totalCount = await _unitOfWork._carRepository.GetQueryable()
                                .Where(filter).CountAsync();

        var query = _unitOfWork._carRepository.GetQueryable(filter, allowTracking: false);

        // Thêm sắp xếp linh hoạt
        query = Sort?.ToLower() switch
        {
            "asc" => query.OrderBy(c => c.CreatedAt),
            "desc" => query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderBy(c => c.Model)
        };

        query = query.Skip((PageNumber - 1) * PageSize).Take(PageSize);

        List<CarViewModel> carViewModels = new List<CarViewModel>();
        var result = await query.ToListAsync();

        foreach (var item in result)
        {
            var car = await ViewModelConvertToModel(item);
            carViewModels.Add(car);
        }

        return new StaticPagedList<CarViewModel>(carViewModels, PageNumber, PageSize, totalCount);
    }

}