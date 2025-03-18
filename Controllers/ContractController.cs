using System;
using System.IO;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Car_Rental_System.ViewModels;
using Car_Rental_System.Services;
using Microsoft.AspNetCore.Authorization;
namespace Car_Rental_System.Controllers
{ [Authorize(Roles = "Manager,Staff")]
    public class ContractController : Controller
    {
        private readonly ViewRenderService _viewRenderService;
        private readonly IWebHostEnvironment _env;
        private readonly PdfService _pdfService;
        private readonly IContractRepository _contractRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IEmailSender _emailSender;

        private readonly IProfileRepository _profileRepository;
        public ContractController( IProfileRepository profileRepository,IEmailSender emailSender, IContractRepository contractRepository, IWebHostEnvironment env, PdfService pdfService, ICloudinaryService cloudinaryService, ViewRenderService viewRenderService)
        {
            _profileRepository = profileRepository;
            _emailSender = emailSender;
            _viewRenderService = viewRenderService;
            _cloudinaryService = cloudinaryService;
            _pdfService = pdfService;
            _env = env;
            _contractRepository = contractRepository;
        }

        // GET: Contract
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            string stringValue = searchString ?? "";
            int pageNumber = page ?? 1;
            int pageSize = 10;
            var contracts = await _contractRepository.GetAllAsync(stringValue, pageSize, pageNumber);
            return View(contracts);
        }

        // GET: Contract/Details/5
        public async Task<IActionResult> Details(int id)
        {

            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            var profile = await _profileRepository.GetByUserIdAsync(contract.UserId);

            var ContractViewModel = new ContractViewModel{
                Contract = contract,
                imgaeDriver = profile.DrivingLicense!
            };

            return View(ContractViewModel);
        }

        // Hiển thị form hợp đồng
        public IActionResult CreateContract()
        {
            return View();
        }

        public async Task<IActionResult> ContractTemplate()
        {
            string check = string.Empty;
            var contract = await _contractRepository.GetByIdAsync(15, check);
            return View(contract);
        }


        [HttpPost]
        public async Task<IActionResult> CreateContract(int contractId)
        {
            string check = string.Empty;
            var model = await _contractRepository.GetByIdAsync(contractId, check);
            if (model == null)
            {
                return NotFound(new { message = "Hợp đồng không tồn tại!" });
            }

            // Render HTML cho hợp đồng
            var htmlContent = await _viewRenderService.RenderViewToStringAsync(this, "ContractTemplate", model);

            // Tạo file PDF từ HTML
            byte[] pdfBytes = await _pdfService.GeneratePdfFromHtml(htmlContent);

            model.PdfFileData = pdfBytes;
            await _contractRepository.UpdateAsync(model);

            return RedirectToAction("Index");
        }

    }
}