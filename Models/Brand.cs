
using System.ComponentModel.DataAnnotations;

namespace Car_Rental_System.Models;
public class Brand
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Status { get; set; } = true;

    // Danh sách các xe thuộc thương hiệu này
    public ICollection<Car> Cars { get; set; } = new List<Car>();
}