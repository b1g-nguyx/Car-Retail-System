// AccountController.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Car_Rental_System.Models;
using Car_Rental_System.Services;
using Car_Rental_System.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Car_Rental_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IProfileRepository _profileRepository;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IConfiguration configuration, RoleManager<IdentityRole> roleManager, IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
            _roleManager = roleManager;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Profile = new Profile
                {
                    FullName = model.FullName,
                }
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Xác nhận email", $"Vui lòng xác nhận email của bạn bằng cách nhấp vào <a href='{confirmationLink}'>đây</a>");

                return View("RegisterSuccess");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        [Authorize]
        public IActionResult RegisterSuccess()
        {
            return View();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Yêu cầu xác nhận email không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? "ConfirmEmailSuccess" : "ConfirmEmailFailure");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            var adminEmail = _configuration["AdminUser:Email"];  // Cập nhật key đúng với appsettings.json
            var adminPassword = _configuration["AdminUser:Password"];

            // Kiểm tra nếu là tài khoản Admin
            if (email == adminEmail && password == adminPassword)
            {
                // Kiểm tra và tạo role Admin nếu chưa có
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Tạo Claims để đăng nhập Admin mà không cần UserManager
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, adminEmail),
            new Claim(ClaimTypes.Email, adminEmail),
            new Claim(ClaimTypes.Role, "Admin")
        };

                var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = rememberMe };

                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "UserManagement"); // Chuyển đến Admin Dashboard
            }

            // Kiểm tra tài khoản bình thường trong database
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email và mật khẩu không được để trống.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Bạn phải xác nhận email trước khi đăng nhập.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("CheckRole");
            }

            ModelState.AddModelError(string.Empty, "Đăng nhập không thành công.");
            return View();
        }
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }
        public IActionResult LoginWithFacebook()
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return Challenge(properties, "Facebook");
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");

            // Kiểm tra xem người dùng đã tồn tại chưa
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                // Nếu chưa có, kiểm tra email
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                if (email != null)
                {
                    user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        // Tạo user mới
                        user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            Profile = new Profile
                            {
                                FullName = name ?? "",
                            }
                        };
                        await _userManager.CreateAsync(user);
                    }
                    // Liên kết tài khoản đăng nhập ngoài với tài khoản Identity
                    await _userManager.AddLoginAsync(user, info);
                }
            }

            // Đăng nhập user vào hệ thống
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("CheckRole");
            }

            return RedirectToAction("Login");
        }
        [Authorize]

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [Authorize]

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [Authorize]

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["Message"] = "Email không tồn tại!";
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { token, email }, Request.Scheme);

            await _emailSender.SendEmailAsync(email, "Đặt lại mật khẩu", $"Click vào link sau để đặt lại mật khẩu: <a href='{resetLink}'>Đặt lại mật khẩu</a>");

            TempData["Message"] = "Vui lòng kiểm tra email để đặt lại mật khẩu!";
            return View();
        }
        [Authorize]

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return BadRequest("Yêu cầu không hợp lệ!");
            }
            ViewData["Token"] = token;
            ViewData["Email"] = email;
            return View();
        }
        [Authorize]

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, string email, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Email không hợp lệ!");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                TempData["Message"] = "Mật khẩu đã được đặt lại thành công!";
                return RedirectToAction("Login");
            }

            TempData["Message"] = "Lỗi đặt lại mật khẩu!";
            return View();
        }

        [Authorize]
        public IActionResult CheckRole()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "UserManagement");
            }
            if (User.IsInRole("Manager"))
            {
                return RedirectToAction("Index", "Car");
            }
            if (User.IsInRole("Staff"))
            {
                return RedirectToAction("Index", "CarRental");
            }
            return RedirectToAction("Index", "Home");
        }
    }

}
