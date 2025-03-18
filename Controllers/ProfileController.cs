using Car_Rental_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using Car_Rental_System.Services;
using Car_Rental_System.Utils;
using Microsoft.AspNetCore.Authorization;
using Car_Rental_System.ViewModels;
using Car_Rental_System.Repositories;
using Car_Rental_System.Models.Enums;

[Authorize]
public class ProfileController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IProfileRepository _profileRepository;
    private readonly IContractRepository _contractRepository;
    private readonly ICarRentalRepository _carRentalRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ViewRenderService _viewRenderService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly PdfService _pdfService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public ProfileController(SignInManager<ApplicationUser> signInManager,
        ICloudinaryService cloudinaryService,
        PdfService pdfService,
        ViewRenderService viewRenderService,
        IWebHostEnvironment env,
        IProfileRepository profileRepo,
        IContractRepository contractRepo,
        ICarRentalRepository carRentalRepo,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _cloudinaryService = cloudinaryService;
        _pdfService = pdfService;
        _viewRenderService = viewRenderService;
        _env = env;
        _profileRepository = profileRepo;
        _contractRepository = contractRepo;
        _carRentalRepository = carRentalRepo;
        _userManager = userManager;
    }

    // Hiển thị hồ sơ người dùng
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();
        var profile = await _profileRepository.GetByUserIdAsync(user.Id);
        return View(profile);
    }

    // Cập nhật thông tin hồ sơ

    [HttpGet]
    public async Task<IActionResult> Edit(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return View(nameof(Index));
        }

        var display = await _profileRepository.GetByUserIdAsync(userId);
        if (display == null)
        {
            return NotFound(); // Tránh truyền model null vào view
        }

        var profile = new profileViewModel
        {
            UserId = display.UserId!,
            FullName = display.FullName,
            Address = display.Address,
            PhoneNumber = display.PhoneNumber,
            AvatarUrl = display.AvatarUrl,
            DrivingLicense = display.DrivingLicense
        };
        return View(profile);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(profileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        string AvatarURL = string.Empty;
        string DriverURL = string.Empty;
        if (model.DrivingLicenseFiles != null)
        {
            DriverURL = await _cloudinaryService.UploadImageAsync(model.DrivingLicenseFiles) ?? string.Empty;
        }
        if (model.AvatarFiles != null)
        {
            AvatarURL = await _cloudinaryService.UploadImageAsync(model.AvatarFiles) ?? string.Empty;

        }
        var profile = new Profile
        {
            UserId = model.UserId!,
            FullName = model.FullName!,
            Address = model.Address,
            PhoneNumber = model.PhoneNumber,
            AvatarUrl = AvatarURL,
            DrivingLicense = DriverURL
        };

        await _profileRepository.UpdateAsync(profile);
        TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
        return RedirectToAction("Index");
    }


    [HttpGet]
    public async Task<IActionResult> ViewDetailContract(int id)
    {
        string check = string.Empty;
        var item = await _contractRepository.GetByIdAsync(id, check);

        return View(item);
    }


    [HttpPost]
    public async Task<IActionResult> SaveSignature([FromBody] SignatureRequest request)
    {
        if (request == null || request.Id <= 0 || string.IsNullOrEmpty(request.SignatureData))
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }
        string check = string.Empty;
        var contract = await _contractRepository.GetByIdAsync(request.Id, check);
        if (contract == null)
        {
            return NotFound(new { message = "Hợp đồng không tồn tại!" });
        }
        try
        {
            byte[] signatureBytes = Convert.FromBase64String(request.SignatureData.Split(',')[1]);
            string filePath = Path.Combine("wwwroot/signatures", $"{request.Id}.png");

            contract.SignatureImage = request.SignatureData;
            contract.Status = Contracts.Signature;
            await _contractRepository.UpdateAsync(contract);


            var htmlContent = await _viewRenderService.RenderViewToStringAsync(this, "ContractTemplate", contract);

            byte[] pdfBytes = await _pdfService.GeneratePdfFromHtml(htmlContent);

            contract.PdfFileData = pdfBytes;
            await _contractRepository.UpdateAsync(contract);

            var item = await _carRentalRepository.GetByIdContractAsync(contract.Id);
            item.Status = RentalStatus.Rented;
            await _carRentalRepository.UpdateAsync(item);
            return Ok(new { message = "Lưu chữ ký thành công!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Lỗi khi lưu chữ ký!", error = ex.Message });
        }


    }


    // Xem lịch sử hợp đồng
    public async Task<IActionResult> ContractHistory()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var contracts = await _contractRepository.GetContractsInPartByUserIdAsync(user.Id);
        return View(contracts);
    }


    public async Task<IActionResult> ContractTemplate()
    {
        string check = string.Empty;
        var contract = await _contractRepository.GetByIdAsync(15, check);
        return View(contract);
    }


    // Cập nhật hợp đồng hiện tại
    public async Task<IActionResult> CurrentContract()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var contract = await _contractRepository.GetCurrentContractByUserIdAsync(user.Id);
        return View(contract);
    }

    // Model nhận chữ ký
    public class SignatureRequest
    {
        public int Id { get; set; }
        public string SignatureData { get; set; }
    }

    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        var user = await _userManager.GetUserAsync(User);
        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            ViewData["NotTrue"] = "1";
        }
        var ChangePassword = new ChangePasswordViewModel{
            UserId = user.Id
        };
        return View(ChangePassword);
    }

    // Xử lý đổi mật khẩu
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            ModelState.Remove("OldPassword");
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!hasPassword)
        {
            var password = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (password.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Mật khẩu đã được cập nhật thành công!";
                return RedirectToAction("Index");
            }
        }
        else
        {
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Mật khẩu đã được cập nhật thành công!";
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

}
