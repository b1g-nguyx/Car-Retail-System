using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Car_Rental_System.Models;
using Car_Rental_System.Repositories;
using System.Threading.Tasks;

namespace Car_Rental_System.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICarRepository _icarRepository;
    private readonly IBrandRepository _ibrandRepository;
    private readonly ICategoryRepository _icategoryRepository;
    public HomeController(ILogger<HomeController> logger, ICarRepository carRepository, IBrandRepository brandRepository, ICategoryRepository categoryRepository)
    {
        _icategoryRepository = categoryRepository;
        _ibrandRepository = brandRepository;
        _icarRepository = carRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string searchString)
    {
        searchString = searchString ?? "";
        var items = await _icarRepository.GetCarByQuantityAsync(9);
        var brandList = await _ibrandRepository.GetAllAsync();
        var categoryList = await _icategoryRepository.GetAllAsync();

        var homeViewModel = new HomeViewModel
        {
            Categories = categoryList,
            Brands = brandList,
            CarViewModels = items
        };

        ViewData["searchString"] = searchString;
        return View(homeViewModel);
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
