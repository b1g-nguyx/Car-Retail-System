using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Car_Rental_System.Controllers;
public class ConfirmProfileController : Controller
{

    private readonly IProfileRepository _profileRepository;

    public ConfirmProfileController(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string id)
    {
        var display = await _profileRepository.GetByUserIdAsync(id);

        if (display == null)
        {

            return NotFound();
        }

        return View(display);
    }
}