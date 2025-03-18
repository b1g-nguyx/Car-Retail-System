using Car_Rental_System.Models;
using X.PagedList;
namespace Car_Rental_System.ViewModels;
public class FilterCarViewModel
{
    public string? SearchString { get; set; } = default!;
    public int PageSize { get; set; } = default!;
    public int PageNumber { get; set; } = default!;
    public string Sort { get; set; } = default!;
    public decimal MinPrice { get; set; } = -1m;
    public decimal MaxPrice { get; set; } = -1m;
    public List<int> BrandIds { get; set; } = default!;
    public int CategoryId { get; set; } = default!;
    public IEnumerable<Brand> Brands { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IPagedList<CarViewModel> CarViewModels { get; set; } = default!;
}