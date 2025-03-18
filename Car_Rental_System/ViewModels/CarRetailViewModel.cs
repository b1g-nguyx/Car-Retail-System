using Car_Rental_System.Models;
using Car_Rental_System.Models.Enums;
namespace Car_Rental_System.ViewModels;
public class CarRetailViewModel
{
    public CarRental CarRental { get; set; } = default!;
    public IEnumerable<Images>? Images { get; set; } = new List<Images>();
    public List<IFormFile>? Files {get; set;} = new List<IFormFile>();
    public List<RentalStatus> RentalStatuses = Enum.GetValues<RentalStatus>().ToList();
}