using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly IContractRepository _contractRepository;

    public ApiController(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    [HttpGet("DownloadContract/{contractId}")]
    public async Task<IActionResult> DownloadContract(int contractId)
    {
        string check = string.Empty;
        var contract = await _contractRepository.GetByIdAsync(contractId,check);
        if (contract == null || contract.PdfFileData == null)
        {
            return NotFound(new { message = "Không tìm thấy hợp đồng hoặc hợp đồng chưa có file PDF!" });
        }

        return File(contract.PdfFileData, "application/pdf", $"Contract_{contractId}.pdf");
    }
}
