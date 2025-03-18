using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Car_Rental_System.Models;
using Car_Rental_System.Repositories;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Mvc.Core;
using Car_Rental_System.ViewModels;
using Car_Rental_System.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json;

namespace Car_Rental_System.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICarRepository _icarRepository;
    private readonly IBrandRepository _ibrandRepository;
    private readonly ICategoryRepository _icategoryRepository;
    private readonly IImageRepository _iimageRepository;
    public HomeController(ILogger<HomeController> logger, ICarRepository carRepository, IBrandRepository brandRepository, ICategoryRepository categoryRepository, IImageRepository imageRepository)
    {
        _iimageRepository = imageRepository;
        _icategoryRepository = categoryRepository;
        _ibrandRepository = brandRepository;
        _icarRepository = carRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _icarRepository.GetByTopAsync();
        var brandList = await _ibrandRepository.GetAllForListAsync(true);
        var categoryList = await _icategoryRepository.GetAllForListAsync(true);

        List<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();
        foreach (var item in categoryList)
        {
            var Images = await _iimageRepository.GetAllByIdRelationId(item.Id, Constant.category);
            var category = new CategoryViewModel
            {
                Images = Images,
                Category = item
            };
            categoryViewModels.Add(category);
        }
        List<BrandViewModel> brandViewModels = new List<BrandViewModel>();
        foreach (var item in brandList)
        {
            var Images = await _iimageRepository.GetAllByIdRelationId(item.Id, Constant.brand);
            var brand = new BrandViewModel
            {
                Images = Images,
                Brand = item
            };
            brandViewModels.Add(brand);
        }
        var homeViewModel = new HomeViewModel
        {
            CategoryViewModels = categoryViewModels,
            BrandViewModels = brandViewModels,
            CarViewModels = items
        };

        var DateTimeJson = HttpContext.Session.GetString("DateFilter");
        if (DateTimeJson != null)
        {
            var DateData = JsonConvert.DeserializeObject<dynamic>(DateTimeJson);
            ViewData["StartDate"] = DateData!.StartDate.ToString("yyyy-MM-ddTHH:mm");
            ViewData["EndDate"] = DateData!.EndDate.ToString("yyyy-MM-ddTHH:mm");;
        }
        else
        {
            ViewData["StartDate"] = null;
            ViewData["EndDate"] = null;
        }

        return View(homeViewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var DateTimeJson = HttpContext.Session.GetString("DateFilter");
        if (DateTimeJson == null)
        {
            DateTime StartDate = DateTime.Now.AddMinutes(10);
            DateTime EndDate = StartDate.AddHours(1);
            HttpContext.Session.SetString("DateFilter", JsonConvert.SerializeObject(new
            {
                StartDate,
                EndDate
            }));
        }
        var car = await _icarRepository.GetByIdAsync(id);
        var relatedCars = await _icarRepository.GetByTopAsync();
        var detailCarViewModel = new DetailCarViewModel
        {
            CarViewModel = car,
            RelatedCars = relatedCars
        };
        if (car == null)
        {
            return NotFound();
        }

        return View(detailCarViewModel);
    }
    public async Task<IActionResult> Shop(FilterCarViewModel filterCarViewModel, int? page, List<int>? BrandIds, string? searchString, DateTime StartDate, DateTime EndDate)
    {

        filterCarViewModel.PageSize = 12;
        filterCarViewModel.PageNumber = page ?? 1;


        filterCarViewModel.BrandIds = BrandIds ?? new List<int>();

        var brandList = await _ibrandRepository.GetAllForListAsync(true);
        filterCarViewModel.Brands = brandList;
        filterCarViewModel.MinPrice = filterCarViewModel.MinPrice < 0 ? 0m : filterCarViewModel.MinPrice;
        filterCarViewModel.MaxPrice = filterCarViewModel.MaxPrice <= 0 ? 10000000m : filterCarViewModel.MaxPrice;

        DateTime SaveStartDate = default(DateTime);
        DateTime SaveEndDate = default(DateTime);

        var DateTimeJson = HttpContext.Session.GetString("DateFilter");

        if ((StartDate != default(DateTime) && EndDate != default(DateTime)) || (EndDate > DateTime.Now))
        {
            HttpContext.Session.SetString("DateFilter", JsonConvert.SerializeObject(new
            {
                StartDate,
                EndDate
            }));
        }
        else
        {
            if (DateTimeJson != null)
            {
                var DateData = JsonConvert.DeserializeObject<dynamic>(DateTimeJson);
                SaveStartDate = DateData!.StartDate;
                SaveEndDate = DateData.EndDate;
            }
            else
            {
                StartDate = DateTime.Now.AddMinutes(10);
                EndDate = SaveStartDate.AddHours(1);
                HttpContext.Session.SetString("DateFilter", JsonConvert.SerializeObject(new
                {
                    StartDate,
                    EndDate
                }));
                SaveStartDate = StartDate;
                SaveEndDate = EndDate;
            }

        }
        filterCarViewModel.StartDate = SaveStartDate;
        filterCarViewModel.EndDate = SaveEndDate;

        ViewData["SearchString"] = searchString;
        filterCarViewModel.SearchString = searchString ?? "";
        var carss = await _icarRepository.GetAllAsync(
            filterCarViewModel.SearchString,
            filterCarViewModel.PageNumber,
            filterCarViewModel.PageSize,
            filterCarViewModel.Sort,
            filterCarViewModel.MinPrice,
            filterCarViewModel.MaxPrice,
            filterCarViewModel.BrandIds,
            filterCarViewModel.CategoryId,
            filterCarViewModel.StartDate,
            filterCarViewModel.EndDate
        );

        filterCarViewModel.CarViewModels = carss;
        return View(filterCarViewModel);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}

