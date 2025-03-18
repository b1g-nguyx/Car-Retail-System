using Car_Rental_System.Models;
using Microsoft.AspNetCore.Identity;
using System;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }

    public virtual ICollection<CarRental>? CarRentals { get; set; }
    public virtual Profile Profile { get; set; }
}
