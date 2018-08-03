﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SammysAuto.Data;
using SammysAuto.ViewModels;

namespace SammysAuto.Controllers
{
    public class ServicesController : Controller
    {
        private ApplicationDbContext _db;

        public ServicesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET: Services/Create
        public IActionResult Create(int carId)
        {
            var model = new CarAndServicesViewModel
            {
                CarObj = _db.Cars.FirstOrDefault(c => c.Id == carId),
                ServiceTypesObj = _db.ServiceTypes.ToList(),
                PastServicesObj = _db.Services
                                    .Where(s => s.CarId == carId)
                                    .OrderByDescending(s => s.DateAdded)
                                    .Take(5)
            };

            return View(model);
        }

        //POST : Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarAndServicesViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.NewServiceObj.CarId = model.CarObj.Id;
                model.NewServiceObj.DateAdded = DateTime.Now;
                _db.Add(model.NewServiceObj);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Create), new { carId = model.CarObj.Id });
            }

            var newModel = new CarAndServicesViewModel
            {
                CarObj = _db.Cars.FirstOrDefault(c => c.Id == model.CarObj.Id),
                ServiceTypesObj = _db.ServiceTypes.ToList(),
                PastServicesObj = _db.Services
                                    .Where(s => s.CarId == model.CarObj.Id)
                                    .OrderByDescending(s => s.DateAdded)
                                    .Take(5)
            };
            return View(newModel);

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