using Car_Rental_System.Models;
namespace Car_Rental_System.ViewModels;
public class NavbarViewModel
{
    public IEnumerable<Category> Categories { get; set; } = default!;
    public string CurrentUrl { get; set; } = default!;
}