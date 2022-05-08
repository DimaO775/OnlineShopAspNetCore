using Hangfire;
using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    [Authorize(Roles = "admin")]
    public class BackgroundJobsController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<User> _userManager;
        //create and field...
        public BackgroundJobsController(ShopDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       // [HttpPost]
        public IActionResult NumberOfViewsClear()
        {
            RecurringJob.AddOrUpdate( ()=>  NumberOfViews(), Cron.Weekly);
            return Ok("Goood");
        }


        public void  NumberOfViews()
        {
            List<Product> products = _context.Products.Where(t => t.NumberOfViews != 0).ToList();
            products.ForEach(t => t.NumberOfViews = 0);
            _context.Products.UpdateRange(products);
            _context.SaveChanges();
        }



        public IActionResult FavoriteProductsClear()
        {
            RecurringJob.AddOrUpdate(() => FavoriteProducts(), Cron.Daily);
            return Ok("Goood");
        }
        public void FavoriteProducts()
        {
            List<FavoritesProducts> favoritesProducts = _context.FavoritesProducts.AsEnumerable().Where(t => (DateTime.Now - t.Date).TotalDays == 7).ToList();
            _context.FavoritesProducts.RemoveRange(favoritesProducts);
            _context.SaveChanges();
        }


        public IActionResult LastViewsProductsClear()
        {
            RecurringJob.AddOrUpdate(() => LastViewsProducts(), Cron.Hourly);
            return Ok("Goood");
        }
        public void LastViewsProducts()
        {
            List<LastViews> lastViews = _context.LastViews.AsEnumerable().Where(t => (DateTime.Now - t.Date).TotalDays == 7).ToList();
            _context.LastViews.RemoveRange(lastViews);
            _context.SaveChanges();
        }
    }
}
