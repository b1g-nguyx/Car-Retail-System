using Microsoft.AspNetCore.Mvc;

namespace Car_Rental_System.Controllers.Components;

public class TopbarViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync()
    {
        return Task.FromResult((IViewComponentResult)View("Default"));
    }
}