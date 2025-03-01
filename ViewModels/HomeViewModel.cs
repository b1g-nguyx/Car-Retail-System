using Car_Rental_System.Models;

public class HomeViewModel
{
    public IEnumerable<CarViewModel> CarViewModels { get; set; } = default!;
    public IEnumerable<Brand> Brands { get; set; } = default!;
    public IEnumerable<Category> Categories { get; set; } = default!;
}