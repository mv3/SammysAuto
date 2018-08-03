using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SammysAuto.Data;
using SammysAuto.Models;
using SammysAuto.ViewModels;

namespace SammysAuto.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CarsController(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index(string userId = null)
        {
            if(userId == null)
            {
                // Only called when a guest user logs in
                userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var model = new CarAndCustomerViewModel
            {
                Cars = _db.Cars.Where(c => c.UserId == userId),
                UserObj = _db.Users.FirstOrDefault(u => u.Id == userId)
            };

            return View(model);
        }

        // GET Create
        public IActionResult Create(string userId)
        {
            Car carObj = new Car
            {
                Year = DateTime.Now.Year,
                UserId = userId
            };
            return View(carObj);
        }

        // POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (ModelState.IsValid)
            {
                _db.Add(car);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { userId = car.UserId });
            }
            return View(car);
        }

        // Details GET
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _db.Cars
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                NotFound();
            }

            return View(car);
        }

        // Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _db.Cars
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                NotFound();
            }

            return View(car);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(car);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { userId = car.UserId });
            }

            return View(car);
        }

            // Delete GET
            public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _db.Cars
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                NotFound();
            }

            return View(car);
        }

        // Edit POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _db.Cars.SingleOrDefaultAsync(c=>c.Id==id);

           if (car == null)
            {
                return NotFound();
            }
            _db.Cars.Remove(car);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { userId = car.UserId});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
        }
    }
}