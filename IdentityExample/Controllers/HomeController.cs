using IdentityExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PagedList.Core;
using PagedList;
using IdentityExample.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopDbContext context;
        

        public HomeController(ILogger<HomeController> logger, ShopDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public IActionResult Index(string category)
        {
            IQueryable<Product>products = context.Products.Include(t => t.Category).Include(p=>p.Photos).Include(t=>t.Comments);        
            if (!string.IsNullOrEmpty(category))
                products = products.Where(t => t.Category.Title.ToLower() == category.ToLower());
            IQueryable<Category> categories = context.Categories;

            return View(new HomeIndexViewModel
            {
                ChildCategories = categories,
                CurrentCategory = category,
                Products = products
            });
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult GetUserAgent([FromHeader(Name = "User-Agent")] string userAgent)
        {
            return Content(userAgent);
        }

        public IActionResult NewCat()
        {
            return View();
        }

        public IActionResult GetCategory([FromQuery]Category category)
        {
            return Content($"{category.Id}. {category.Title}");
        }

    }
}
