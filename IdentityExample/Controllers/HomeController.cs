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
using Microsoft.AspNetCore.Identity;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopDbContext context;
        private readonly UserManager<User> userManager;

        public HomeController(ILogger<HomeController> logger, ShopDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            if (userManager.GetUserId(User) != null)
                ViewBag.UserId = userManager.GetUserId(User).ToString();
            // IQueryable<Product> favoritesProducts = context.Products.Include(t => t.Category).Include(p=>p.Photos).Include(t=>t.Comments).Where(t=>t.Id);        
            IQueryable<FavoritesProducts> favoritesProducts = context.FavoritesProducts.Include(t => t.Product).ThenInclude(t => t.Comments).Include(t => t.Product)
                .ThenInclude(t => t.Discount).Include(t => t.Product).ThenInclude(t => t.Photos).Include(t => t.Product).Where(t => t.UserId == userManager.GetUserId(User)).OrderBy(t=>t.Date).Reverse();

            IQueryable<LastViews> lastViewsProducts = context.LastViews.Include(t => t.Product).ThenInclude(t => t.Comments).Include(t => t.Product)
                .ThenInclude(t => t.Discount).Include(t => t.Product).ThenInclude(t => t.Photos).Include(t => t.Product).Where(t => t.UserId == userManager.GetUserId(User)).OrderBy(t=>t.Date).Reverse();

            List<Photo> photos = context.Photos.Where(t => t.IsSlider).ToList();
            return View(new HomeIndexViewModel
            {
                Photos = photos,
                LastViews = lastViewsProducts,
                FavoritesProducts = favoritesProducts
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
