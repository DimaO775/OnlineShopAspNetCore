using IdentityExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class ManufacturerController : Controller
    {
        private readonly ShopDbContext _context;

        public ManufacturerController(ShopDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Manufacturer> manufacturers = await _context.Manufacturers.ToListAsync();
            return View(manufacturers);
        }
        
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Title")] Manufacturer manufacturer)
        {
            if (manufacturer.Title == "")
            {
                ModelState.AddModelError(string.Empty, "Название производителя не может быть пустым!");
                return View();
            }
            else if (_context.Manufacturers.Contains(manufacturer))
            {
                ModelState.AddModelError(string.Empty, "Такой производитель уже есть в БД!");
                return View();
            }
            else await _context.Manufacturers.AddAsync(manufacturer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
