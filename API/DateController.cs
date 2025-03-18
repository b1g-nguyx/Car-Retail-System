using Microsoft.AspNetCore.Mvc;
using System;

namespace Car_Rental_System.Controllers
{
    [Route("api/date")]
    [ApiController]
    public class DateController : ControllerBase
    {
        [HttpPost("set-dates")]
        public IActionResult SetDates([FromBody] DateRequest model)
        {
            // Lưu giá trị startDate và endDate vào session
            HttpContext.Session.SetString("StartDate", model.StartDate);
            HttpContext.Session.SetString("EndDate", model.EndDate);

            return Ok(new { success = true });
        }
    }

    public class DateRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

}
