using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Car_Rental_System.ViewModels;
public class EditUserViewModel
{
    public string Id { get; set; }
    
    [Required]
    public string Email { get; set; }

    public IEnumerable<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
    public List<string> SelectedRoles { get; set; } = new List<string>();
}
