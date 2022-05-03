using IdentityExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class SupportThemesController : Controller
    {
        private readonly ShopDbContext _context;

        public SupportThemesController(ShopDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<SupportThemes> supportThemes = await _context.SupportThemes.ToListAsync();
            return View(supportThemes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Topic")] SupportThemes supportThemes)
        {
            if (supportThemes.Topic == "")
            {
                ModelState.AddModelError(string.Empty, "Название темы не может быть пустым!");
                return View();
            }

            else if (_context.SupportThemes.Contains(supportThemes))
            {
                ModelState.AddModelError(string.Empty, "Такая тема уже есть в БД!");
                return View();
            }
            else await _context.SupportThemes.AddAsync(supportThemes);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
