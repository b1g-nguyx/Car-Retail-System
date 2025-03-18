using Car_Rental_System.Models;
namespace Car_Rental_System.ViewModels;
public class HomeViewModel
{
    public IEnumerable<CarViewModel> CarViewModels { get; set; } = default!;
    public IEnumerable<CategoryViewModel>? CategoryViewModels { get; set; }
    public IEnumerable<BrandViewModel>? BrandViewModels { get; set; }
}