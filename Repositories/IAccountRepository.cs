using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAccountRepository
{

    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser> GetUserByIdAsync(string id);
    Task<bool> CreateUserAsync(ApplicationUser user, string password);
    Task<bool> UpdateUserRolesAsync(string userId, List<string> roles);
    Task<List<string>> GetUserRolesAsync(ApplicationUser user);
    Task<bool> DeleteUserAsync(string id);
}
