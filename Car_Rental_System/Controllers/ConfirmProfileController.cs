using System.Threading.Tasks;
using Car_Rental_System.Models;
using Car_Rental_System.ViewModels;
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
    public async Task<IActionResult> Index(string searchString, int? page)
    {
        string searchValue = searchString ?? "";
        int pageNumber = page ?? 1;
        int pageSize = 10;
        var display = await _profileRepository.GetAllAsync(searchValue, pageNumber, pageSize);
        ViewData["searchString"] = searchString;
        return View(display);
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


    [HttpPost]
    public async Task<IActionResult> ConfirmProfile(string UserId)
    {
        var model = await _profileRepository.GetByUserIdAsync(UserId);
        if (!ModelState.IsValid)
        {

            return View(model);
        }
        model.IsConfirm = true;
        await _profileRepository.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }
}