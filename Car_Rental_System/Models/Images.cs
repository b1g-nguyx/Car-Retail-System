using System.ComponentModel.DataAnnotations;

namespace Car_Rental_System.Models;

public class Images
{
    [Key]
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string RelationId { get; set; } = default!;
    public string NameRelation { get; set; } = string.Empty;

}