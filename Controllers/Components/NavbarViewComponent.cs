using Car_Rental_System.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Car_Rental_System.Controllers.Components;

public class NavbarViewComponent : ViewComponent
{

    private readonly ICategoryRepository _icategoryRepository;
    public NavbarViewComponent(ICategoryRepository categoryRepository)
    {
        _icategoryRepository = categoryRepository;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {

        var categories = await _icategoryRepository.GetAllAsync();
        return View("Default", categories);
    }
}