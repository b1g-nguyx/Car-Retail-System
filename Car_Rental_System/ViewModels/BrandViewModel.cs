using Car_Rental_System.Models;

namespace Car_Rental_System.ViewModels
{
    public class BrandViewModel
    {
        public Brand Brand { get; set; } = default!;
        public IEnumerable<Images> Images { get; set; } = default!;

        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}