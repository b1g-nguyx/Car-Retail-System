using Microsoft.AspNetCore.Mvc;

namespace Car_Rental_System.Controllers.Components;

public class FooterViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync()
    {
        return Task.FromResult((IViewComponentResult)View("Default"));
    }
}