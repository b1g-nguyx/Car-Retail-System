using Microsoft.AspNetCore.Mvc;
using Car_Rental_System.Models;
using System.Linq;
using System.Threading.Tasks;
using Car_Rental_System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Car_Rental_System.Utils;
using Microsoft.AspNetCore.Authorization;
using Car_Rental_System.Repositories;
using Car_Rental_System.Models.Enums;

namespace Car_Rental_System.Controllers
{
    [Authorize(Roles = "Manager")]
    public class CarRentalController : Controller
    {
        private readonly ICarRentalRepository _carRentalRepository;

        public CarRentalController(ICarRentalRepository carRentalRepository)
        {
            _carRentalRepository = carRentalRepository;
        }

        public async Task<IActionResult> Index(string searchString, int? page)
        {

            int pageNumber = page ?? 1;
            var rentals = await _carRentalRepository.GetAllAsync(searchString, 10, pageNumber);
            return View(rentals);
        }


        public async Task<IActionResult> Details(int id)
        {
            var carRental = await _carRentalRepository.GetByIdAsync(id);
            if (carRental == null)
            {
                return NotFound();
            }
            return View(carRental);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnCar(int id)
        {
            var carRental = await _carRentalRepository.GetByIdAsync(id);

            if (carRental == null)
            {
                return NotFound();
            }

            carRental.Status = RentalStatus.Completed;
            await _carRentalRepository.UpdateAsync(carRental);
            return RedirectToAction("Index");
        }
    }
}