using Car_Rental_System.Models;
using Car_Rental_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;
 [Authorize(Roles = "Admin")]
public class UserManagementController : Controller
{
    private readonly IAccountRepository _accountRepository;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IProfileRepository _profileRepository;
    public UserManagementController(IAccountRepository accountRepository, RoleManager<IdentityRole> roleManager, IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
        _accountRepository = accountRepository;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _accountRepository.GetAllUsersAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            bool success = await _accountRepository.CreateUserAsync(user, model.Password);
            if (success)
            {
                var profile = new Profile
                {
                    UserId = user.Id
                };
                await _profileRepository.UpdateAsync(profile);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Không thể tạo tài khoản.");
        }
        return View(model);
    }
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _accountRepository.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        var allRoles = _roleManager.Roles;
        var userRoles = await _accountRepository.GetUserRolesAsync(user);
        var model = new EditUserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            Roles = allRoles,
            SelectedRoles = userRoles// Chỉ lấy danh sách vai trò nếu cần
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        bool success = await _accountRepository.UpdateUserRolesAsync(model.Id, model.SelectedRoles);
        if (success)
            return RedirectToAction("Index");

        return View(model);
    }
}
