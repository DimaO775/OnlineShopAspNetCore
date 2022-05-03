using IdentityExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Components
{
    public class CategoriesWithSubs : ViewComponent
    {
        private readonly ShopDbContext context;

        public CategoriesWithSubs(ShopDbContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string currentCategory, int? parentId)
        {

            IQueryable<Category> categories = context.Categories.Include(t => t.ChildCategories).Distinct();
                //.Select(p => p.Category).Include(r => r.ChildCategories).Distinct();
                //henInclude(a => a.ChildCategories)..Distinct();
            categories = categories.Where(t => t.ParentCategoryId == parentId);
            //(List<string>, string) res = (categories, currentCategory);
            return View((Categories: await categories.ToListAsync(), CurrentCategory: currentCategory));
        }
    }
}
