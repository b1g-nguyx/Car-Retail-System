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
using Car_Rental_System.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Car_Rental_System.Controllers
{
    [Authorize(Roles = "Manager")]
    public class BrandController : Controller
    {
        private readonly IBrandRepository _ibrandRepository;
        private readonly IImageRepository _iimageRepository;
        public BrandController(IBrandRepository brandRepository, IImageRepository imageRepository)
        {
            _iimageRepository = imageRepository;
            _ibrandRepository = brandRepository;
        }

        // GET: Brand
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            var items = await _ibrandRepository.GetAllAsync("", pageSize, pageNumber);
            return View(items);
        }

        // GET: Brand/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _ibrandRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandViewModel brandViewModel)
        {

            ModelState.Remove("Images");


            if (ModelState.IsValid)
            {
                await _ibrandRepository.AddAsync(brandViewModel);
                return Json(new { success = true });
            }
            return View(brandViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandViewModel = await _ibrandRepository.GetByIdAsync(id);
            if (brandViewModel == null)
            {
                return NotFound();
            }
            return View(brandViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandViewModel brandViewModel)
        {
            ModelState.Remove("Images");
            if (ModelState.IsValid)
            {
                try
                {
                    await _ibrandRepository.UpdateAsync(brandViewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
              return Json(new { success = true });
            }
            return View(brandViewModel);
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandViewModel = await _ibrandRepository.GetByIdAsync(id);
            if (brandViewModel == null)
            {
                return NotFound();
            }

            return View(brandViewModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brandViewModel = await _ibrandRepository.GetByIdAsync(id);
            if (brandViewModel != null)
            {
                await _ibrandRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(int id)
        {

            await _iimageRepository.DeleteAsync(id);
            return Ok(new { message = "Ảnh đã được xóa thành công!" });
        }

    }
}
