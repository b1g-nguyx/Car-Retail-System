
using Car_Rental_System.Models;
using Car_Rental_System.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Car_Rental_System.ViewModels;
public class CarViewModel
{
    public Car Car { get; set; } = new Car();
    public List<IFormFile> files { get; set; } = new List<IFormFile>();
    public IEnumerable<Images> Images { get; set; } = new List<Images>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();

    // Chuyển RentalStatus enum thành SelectListItem
    public IEnumerable<SelectListItem> RentalStatuses => Enum.GetValues<RentalStatus>()
        .Select(rs => new SelectListItem {  Value = ((int)rs).ToString(), Text = rs.ToString() });
}
