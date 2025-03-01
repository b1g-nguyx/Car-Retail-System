using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Car_Rental_System.Data;
using Car_Rental_System.Models;
using Car_Rental_System.Repositories;

namespace Car_Rental_System.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarRepository _icarRepository;
        private readonly ICategoryRepository _icategoryRepository;
        private readonly IBrandRepository _ibrandRepository;
        private readonly IImageRepository _iimageRepository;

        public CarController(ICarRepository carRepository, ICategoryRepository categoryRepository, IBrandRepository brandRepository, IImageRepository imageRepository)
        {
            _iimageRepository = imageRepository;
            _icarRepository = carRepository;
            _icategoryRepository = categoryRepository;
            _ibrandRepository = brandRepository;
        }


        // GET: Car
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            int pageNumber = page ?? 1;
            string searchValue = searchString ?? "";
            var items = await _icarRepository.GetAllAsync(searchValue, pageNumber, 10);
            ViewData["CurrentFilter"] = searchString;
            return View(items);
        }

        // GET: Car/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _icarRepository.GetByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Car/Create
        public async Task<IActionResult> Create()
        {
            var carViewModel = new CarViewModel
            {
                Categories = await _icategoryRepository.GetAllAsync(),
                Brands = await _ibrandRepository.GetAllAsync()
            };

            return View(carViewModel);
        }

        // POST: Car/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarViewModel carViewModel)
        {
            carViewModel.Brand = await _ibrandRepository.GetByIdAsync(carViewModel.BrandId);
            carViewModel.Category = await _icategoryRepository.GetByIdAsync(carViewModel.CategoryId);

            ModelState.Remove("Brand");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                await _icarRepository.AddAsync(carViewModel);
               return Json(new { success = true });
            }
            return View(carViewModel);
        }

        // GET: Car/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _icarRepository.GetByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            car.Categories = await _icategoryRepository.GetAllAsync();
            car.Brands = await _ibrandRepository.GetAllAsync();
            return View(car);
        }

        // POST: Car/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CarViewModel carViewModel)
        {
            ModelState.Remove("Brand");
            ModelState.Remove("Category");
            carViewModel.Brand = await _ibrandRepository.GetByIdAsync(carViewModel.BrandId);
            carViewModel.Category = await _icategoryRepository.GetByIdAsync(carViewModel.CategoryId);
            if (ModelState.IsValid)
            {
                try
                {
                    await _icarRepository.UpdateAsync(carViewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }

            carViewModel.Images = await _iimageRepository.GetAllByIdRelationId(carViewModel.Id, Constants.Constants.car);
            carViewModel.Categories = await _icategoryRepository.GetAllAsync();
            carViewModel.Brands = await _ibrandRepository.GetAllAsync();
            return View(carViewModel);
        }

        // GET: Car/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _icarRepository.GetByIdAsync(id);
            return View(car);
        }

        // POST: Car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _icarRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteImage(int id)
        {
            await _iimageRepository.DeleteAsync(id);
            return Json(new { success = true, message = "Image deleted successfully" });
        }
    }
}
