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
{ [Authorize(Roles = "Manager")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _icategoryRepository;
        private readonly IImageRepository _iimageRepository;

        public CategoryController(ICategoryRepository categoryRepository, IImageRepository imageRepository)
        {
            _iimageRepository = imageRepository;
            _icategoryRepository = categoryRepository;
        }

        // GET: Category
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            var item = await _icategoryRepository.GetAllAsync("", pageSize, pageNumber);
            return View(item);
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _icategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
        {
            ModelState.Remove("Images");
            if (ModelState.IsValid)
            {
                await _icategoryRepository.AddAsync(categoryViewModel);
                return Json(new { success = true });
            }
            return View(categoryViewModel);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryViewModel = await _icategoryRepository.GetByIdAsync(id);
            if (categoryViewModel == null)
            {
                return NotFound();
            }
            return View(categoryViewModel);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel categoryViewModel)
        {

                ModelState.Remove("Images");
            if (ModelState.IsValid)
            {
                try
                {
                    await _icategoryRepository.UpdateAsync(categoryViewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                  return Json(new { success = true });
            }
            return View(categoryViewModel);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryViewModel = await _icategoryRepository.GetByIdAsync(id);
            if (categoryViewModel == null)
            {
                return NotFound();
            }

            return View(categoryViewModel);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryViewModel = await _icategoryRepository.GetByIdAsync(id);
            if (categoryViewModel != null)
            {
                await _icategoryRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}