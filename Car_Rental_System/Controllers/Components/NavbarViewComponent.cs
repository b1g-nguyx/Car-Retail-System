using Car_Rental_System.Repositories;
using Car_Rental_System.ViewModels;
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

        var categories = await _icategoryRepository.GetAllForListAsync(true);

        var currentUrl = HttpContext.Request.Path.Value;

        var navbarViewModel = new NavbarViewModel
        {
            Categories = categories,
            CurrentUrl = currentUrl
        };
        return View("Default", navbarViewModel);
    }
}