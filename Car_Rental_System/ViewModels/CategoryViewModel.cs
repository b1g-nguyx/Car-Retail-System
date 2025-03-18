using Car_Rental_System.Models;

namespace Car_Rental_System.ViewModels;
public class CategoryViewModel
{
    public Category Category { get; set; }
    public IEnumerable<Images> Images { get; set; }
    public List<IFormFile> Files { get; set; } = new List<IFormFile>();
}