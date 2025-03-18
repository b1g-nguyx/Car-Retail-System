
using System.Security.Claims;
using System.Threading.Tasks;
using Car_Rental_System.Helpers;
using Car_Rental_System.Models;
using Car_Rental_System.Models.Enums;
using Car_Rental_System.Repositories;
using Car_Rental_System.Services;
using Car_Rental_System.Utils;
using Car_Rental_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Car_Rental_System.Controllers;

[Authorize]
public class PaymentController : Controller
{


    private readonly ICarRentalRepository _carRentalRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly ICarRepository _carRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IImageRepository _iimageRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IVNPayService _vNPayService;
    public PaymentController(IProfileRepository profileRepository, ICarRepository carRepository, ICarRentalRepository carRentalRepository, IImageRepository imageRepository, IContractRepository contractRepository, IVNPayService vNPayService, IHttpContextAccessor httpContextAccessor)
    {
        _profileRepository = profileRepository;
        _carRepository = carRepository;
        _httpContextAccessor = httpContextAccessor;
        _vNPayService = vNPayService;
        _contractRepository = contractRepository;
        _iimageRepository = imageRepository;
        _carRentalRepository = carRentalRepository;
    }
    public static Dictionary<string, string> vnp_TransactionStatus = new Dictionary<string, string>()
        {
            {"00","Giao dịch thành công" },
            {"01","Giao dịch chưa hoàn tất" },
            {"02","Giao dịch bị lỗi" },
            {"04","Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPAY)" },
            {"05","VNPAY đang xử lý giao dịch này (GD hoàn tiền)" },
            {"06","VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng (GD hoàn tiền)" },
            {"07","Giao dịch bị nghi ngờ gian lận" },
            {"09","GD Hoàn trả bị từ chối" }
        };

    [HttpGet]
    public async Task<IActionResult> Checkout(int carId)
    {
        var item = await _carRepository.GetByIdAsync(carId);
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var profile = await _profileRepository.GetByUserIdAsync(userId!);
        if (string.IsNullOrEmpty(profile.Address) || string.IsNullOrEmpty(profile.DrivingLicense) || string.IsNullOrEmpty(profile.PhoneNumber))
        {
            return RedirectToAction("Index", "Profile");
        }
        var display = new PaymentViewModel
        {
            CarViewModel = item,
            Email = email!,
            TotalPrice = 500000m,
            Profile = profile
        };
        var DateTimeJson = HttpContext.Session.GetString("DateFilter");
        if (DateTimeJson != null)
        {
            var DateData = JsonConvert.DeserializeObject<dynamic>(DateTimeJson);
            display.StartDate = DateData!.StartDate;
            display.EndDate = DateData.EndDate;
        }
        return View(display);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(PaymentViewModel payment, int carId)
    {
        ModelState.Remove("Profile.User");
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Checkout");
        }

        var car = await _carRepository.GetByIdAsync(carId);
        car!.Car.Status = RentalStatus.PendingPayment;
        await _carRepository.UpdateAsync(car);
        payment.CarViewModel = car;


        long timestamp = long.Parse(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        long uniquePart = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);

        long orderId = Math.Abs(timestamp + (uniquePart % 1000000));
        var vnPayModel = new VnPaymentRequestModel
        {
            Amount = 500000,
            CreatedDate = DateTime.UtcNow,
            Description = $"{payment.Profile.FullName} {payment.Profile.PhoneNumber}",
            FullName = payment.Profile.FullName,
            OrderId = orderId
        };
        HttpContext.Session.SetString("PaymentData", JsonConvert.SerializeObject(new
        {
            payment,
            carId
        }));

        var paymentUrl = _vNPayService.CreatePaymentUrl(HttpContext, vnPayModel);
        return Redirect(paymentUrl);
    }


    [HttpGet]
    public async Task<IActionResult> PaymentCallBack()
    {

        var response = _vNPayService.PaymentExecute(Request.Query);
        var paymentDataJson = HttpContext.Session.GetString("PaymentData");
        var paymentData = JsonConvert.DeserializeObject<dynamic>(paymentDataJson);
        int carId = (int)paymentData.carId;
        var car = await _carRepository.GetByIdAsync(carId);

        if (car == null || car.Car.Status != RentalStatus.PendingPayment)
        {
            return BadRequest("Giao dịch không hợp lệ");
        }

        if (response.VnPayResponseCode == "00")
        {

            string FullName = paymentData.payment.Profile.FullName;
            string Email = paymentData.payment.Email;
            string PhoneNumber = paymentData.payment.Profile.PhoneNumber;
            string Address = paymentData.payment.Profile.Address;
            DateTime StartDate = paymentData.payment.StartDate;
            DateTime EndDate = paymentData.payment.EndDate;
            string Deposit = "500000";
            string TaxCode = paymentData.payment.TaxCode;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var totalDays = (decimal)(EndDate - StartDate).TotalDays;
            var TotalPrice = totalDays * car.Car.PricePerDay;
            var deposit = decimal.Parse(Deposit);
            var CreateAt = DateTime.Now;

            var contract = new Contract
            {
                UserId = userId!,
                FullName = FullName,
                StartDate = StartDate,
                EndDate = EndDate,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Address = Address,
                TaxCode = TaxCode,
                TotalPrice = TotalPrice,
                Deposit = deposit,
                Status = Contracts.Pending,
                CreatedAt = CreateAt
            };

            
            await _contractRepository.AddAsync(contract);



            var CarRental = new CarRental
            {
                CarId =carId,
                UserId = userId!,
                StartDate = StartDate,
                EndDate = EndDate,
                TotalPrice = TotalPrice,
                CreatedAt = CreateAt,
                Status = RentalStatus.PendingApproval,
                ContractId = contract.Id
            };
            await _carRentalRepository.AddAsync(CarRental);

            var cars = await _carRepository.GetByIdAsync(carId);
            cars.Car.Status = RentalStatus.Available;
            await _carRepository.UpdateAsync(cars);
            return RedirectToAction(nameof(PaymentSuccess));
        }

        // Get the message corresponding to VnPayResponseCode from the dictionary
        if (vnp_TransactionStatus.TryGetValue(response.VnPayResponseCode!, out var message))
        {
            TempData["Message"] = $"Payment error: {message}";
        }
        else
        {
            TempData["Message"] = $"Unknown payment error: {response.VnPayResponseCode}";
        }

        return RedirectToAction(nameof(PaymentFail));
    }


    public IActionResult PaymentSuccess()
    {
        return View();
    }

    public IActionResult PaymentFail()
    {
        return View();
    }
}