using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityExample.Models;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
namespace IdentityExample.Components
{
    public class CategoriesMenu : ViewComponent
    {
        private readonly ShopDbContext context;

        public CategoriesMenu(ShopDbContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string currentCategory,  int? parentId)
        {
            List<string> categories = await context.Products.Include(t => t.Category)
                .Select(prod => prod.Category.Title)
                .Distinct().ToListAsync();
            //(List<string>, string) res = (categories, currentCategory);
            return View((Categories: categories, CurrentCategory: currentCategory));
        }
        //public  ViewViewComponentResult Invoke(string currentCategory)
        //{
        //    List<string> categories = context.Products.Include(t => t.Category)
        //        .Select(prod => prod.Category.Title)
        //        .Distinct().ToList();
        //    return View(new Tuple<List<string>, string>(categories, currentCategory));
        //}

    }
}
