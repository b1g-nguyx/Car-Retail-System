using System.Linq.Expressions;
using System.Threading.Tasks;
using Car_Rental_System.Models;
using Car_Rental_System.Repositories;
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
        await _iimageRepository.AddAsync(carViewModel.files, car.Id, Constants.Constants.car);
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _unitOfWork._carRepository.GetByIdAsync(id);
        await _iimageRepository.DeleteAsync(item!.Id, Constants.Constants.car);
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

    public async Task<IEnumerable<CarViewModel>> GetAllAsync(bool status)
    {
        IEnumerable<Car> cars = await _unitOfWork._carRepository.GetManyAsync(c => c.Availability == status);
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
        var carViewModel = new CarViewModel();
        carViewModel = await ViewModelConvertToModel(car!);
        return carViewModel;
    }

    public async Task UpdateAsync(CarViewModel car)
    {
        _unitOfWork._carRepository.Update(CarConvert(car));
        await _unitOfWork.SaveChangesAsync();
        await _iimageRepository.AddAsync(car.files, car.Id, Constants.Constants.car);


    }

    public Car CarConvert(CarViewModel carViewModel)
    {
        var car = new Car
        {
            Id = carViewModel.Id,
            CategoryId = carViewModel.Category.Id,
            Category = carViewModel.Category,
            Brand = carViewModel.Brand,
            BrandId = carViewModel.Brand.Id,
            Availability = carViewModel.Availability,
            Description = carViewModel.Description,
            PricePerDay = carViewModel.PricePerDay,
            FuelType = carViewModel.FuelType,
            Kilometers = carViewModel.Kilometers,
            LicensePlates = carViewModel.LicensePlates,
            Model = carViewModel.Model,
            Seats = carViewModel.Seats,
            Transmission = carViewModel.Transmission,
            Year = carViewModel.Year
        };
        return car;
    }
    public async Task<CarViewModel> ViewModelConvertToModel(Car car)
    {


        var carViewModel = new CarViewModel
        {
            Id = car.Id,
            Availability = car.Availability,
            Description = car.Description,
            FuelType = car.FuelType,
            Kilometers = car.Kilometers,
            LicensePlates = car.LicensePlates,
            PricePerDay = car.PricePerDay,
            Model = car.Model,
            Seats = car.Seats,
            Transmission = car.Transmission,
            Year = car.Year,
            BrandId = car.BrandId,
            CategoryId = car.CategoryId

        };

        carViewModel.Category = await _icategoryRepository.GetByIdAsync(car.CategoryId);
        carViewModel.Brand = await _ibrandRepository.GetByIdAsync(car.BrandId);
        carViewModel.Images = await _iimageRepository.GetAllByIdRelationId(car!.Id, Constants.Constants.car);

        return carViewModel;
    }

    public async Task<IEnumerable<CarViewModel>> GetCarByQuantityAsync(int quantity)
    {
        var query = _unitOfWork._carRepository.GetQueryable().Take(quantity);
        var result = await query.ToListAsync();
        List<CarViewModel> carViewModels = new List<CarViewModel>();

        foreach (var item in result)
        {
            var car = await ViewModelConvertToModel(item);
            carViewModels.Add(car);
        }
        return carViewModels;
    }
}