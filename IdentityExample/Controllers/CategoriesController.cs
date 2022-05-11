using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityExample.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using IdentityExample.ViewModels;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Identity;
using IdentityExample.Utils;

namespace IdentityExample.Controllers
{
    public class CategoriesController : Controller
    {
        public CategoriesController(ShopDbContext context, IWebHostEnvironment hostEnvironment, UserManager<User> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }
        private readonly ShopDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<User> _userManager;


        // GET: Categories
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _context.Categories.Include(t => t.Photo).Include(t => t.ParentCategory).ToListAsync());
            }
            catch
            {
                return NotFound();
            }
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                return View(await _context.Categories.Where(t => t.Id == id).Include(t => t.Photo).FirstOrDefaultAsync());
            }
            catch
            {
                return NotFound();
            }
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["ParentCategory"] = new SelectList(new[] { new { Id = 0, Title = "Нет" } }
            .Union(_context.Categories.Select(t => new { t.Id, t.Title })), "Id", "Title");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ParentCategoryId")] Category category, IFormFile Photo)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentCategoryId == 0)
                    category.ParentCategoryId = null;

                _context.Add(category);
                await _context.SaveChangesAsync();
                if (Photo != null)
                {
                    string path = $"/Files/{Photo.FileName}";

                    try
                    {
                        if (!_context.Photos.Where(t => t.PhotoUrl == path).Any())
                        {
                            using (FileStream fileStream = new(_hostEnvironment.WebRootPath + path, FileMode.Create))
                            {
                                await Photo.CopyToAsync(fileStream);
                            }
                            _context.Photos.Add(new() { Filename = Photo.FileName, PhotoUrl = path, CategoryId = category.Id, IsMain = false });
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            _context.Photos.Where(t => t.PhotoUrl == path && t.CategoryId != category.Id).FirstOrDefault().CategoryId = category.Id;
                            await _context.SaveChangesAsync();
                        }
                    }
                    catch
                    {

                    }

                    //if (!_context.Photos.Where(t => t.PhotoUrl == path).Any())
                    //{
                    //    using (FileStream fileStream = new(_hostEnvironment.WebRootPath + path, FileMode.Create))
                    //    {
                    //        await Photo.CopyToAsync(fileStream);
                    //    }
                    //    _context.Photos.Add(new() { Filename = Photo.FileName, PhotoUrl = path, CategoryId = category.Id, IsMain = false });
                    //}
                    //else if (_context.Photos.Where(t => t.PhotoUrl == path && t.CategoryId != category.Id).Any())
                    //    _context.Photos.Where(t => t.PhotoUrl == path).FirstOrDefault().CategoryId = category.Id;

                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id", category.ParentCategoryId);
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Title", category.ParentCategoryId);
                return View(category);
            }
            catch
            {
                return NotFound();
            }

            /*if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Title", category.ParentCategoryId);
            return View(category);*/
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ParentCategoryId")] Category category, IFormFile Photo)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && Photo != null)
            {
                string path = $"/Files/{Photo.FileName}";
                try
                {
                    _context.Photos.Where(t => t.CategoryId == category.Id).FirstOrDefault().CategoryId = null;

                    using (FileStream fileStream = new(_hostEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }

                    if (!_context.Photos.Where(t => t.PhotoUrl == path).Any())
                    {
                        _context.Photos.Add(new() { Filename = Photo.FileName, PhotoUrl = path, CategoryId = category.Id, IsMain = false });
                    }
                    else
                    {
                        _context.Photos.Where(t => t.PhotoUrl == path).FirstOrDefault().CategoryId = category.Id;
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Title", category.ParentCategoryId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                return View(await _context.Categories.Include(c => c.ParentCategory).FirstOrDefaultAsync(m => m.Id == id));
            }
            catch
            {
                return NotFound();
            }
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                await _context.Entry(category).Collection(t => t.ChildCategories).LoadAsync();
                _context.Categories.RemoveRange(category.ChildCategories);
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return NotFound();
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetCategories(string title)
        {
            ViewBag.UserId = _context.Users.Where(t => t.UserName == User.Identity.Name).FirstOrDefault().Id.ToString();
            Category category = _context.Categories.Where(t => t.Title == title).FirstOrDefault();

            List<Product> products = _context.Products.Where(t => t.Category.Title == title).Include(t => t.Photos).Include(t => t.Comments).Include(t => t.FavoritesProducts).ToList();

            await _context.Entry(category).Collection(t => t.ChildCategories).Query().Include(t => t.Photo).Include(t => t.ChildCategories).Include(t => t.Photo).Include(t => t.Products).LoadAsync();
            if (category.ChildCategories.Count() > 0)
            {
                foreach (Category category1 in category.ChildCategories)
                {
                    if (category1.ChildCategories.Count() > 0)
                    {
                        foreach (Category category2 in category1.ChildCategories)
                        {
                            products.AddRange(_context.Products.Where(t => t.Category == category2).Include(t => t.Photos).Include(t => t.Comments).Include(t => t.FavoritesProducts));
                        }
                    }
                    else
                    {
                        products.AddRange(_context.Products.Where(t => t.Category == category1).Include(t => t.Photos).Include(t => t.Comments).Include(t => t.FavoritesProducts));
                    }
                }
            }
            List<Product> topProductsView = new();
            if (products.Count() > 10)
            {
                IEnumerable<Product> products1 = (from Products in products orderby Products.NumberOfViews select Products).Reverse();
                topProductsView = products1.ToList().GetRange(0, 10);
            }
            else
            {
                topProductsView = products;
            }

            if (_userManager.GetUserId(User) != null)
                ViewBag.UserId = _userManager.GetUserId(User).ToString();
            return View(new CategoryProductsViewModel
            {
                FavoritesProducts = MyRequest.GetFavoritesProducts(_context, _userManager.GetUserId(User)),
                LastViews = MyRequest.GetLastViewsProducts(_context, _userManager.GetUserId(User)),
                Category = category,
                PopularProducts = topProductsView
            });
        }

        /*[HttpGet]
        public async Task<IActionResult> GetCategories(string title)
        {
            ViewBag.UserId = _context.Users.Where(t => t.UserName == User.Identity.Name).FirstOrDefault().Id.ToString();
            Category category = _context.Categories.Where(t => t.Title == title).FirstOrDefault();
            List<Product> products = _context.Products.Where(t => t.Category.Title == title).Include(t => t.Photos).Include(t => t.Comments).Include(t => t.FavoritesProducts).ToList();

            await _context.Entry(category).Collection(t => t.ChildCategories).Query().Include(t => t.Photo).Include(t => t.ChildCategories).Include(t => t.Photo).Include(t => t.Products).LoadAsync();
            if (category.ChildCategories.Count() != 0)
            {
                foreach (Category category1 in category.ChildCategories)
                {
                    if (category1.ChildCategories.Count() != 0)
                    {
                        foreach (Category category2 in category1.ChildCategories)
                            products.AddRange(_context.Products.Where(t => t.Category == category2).Include(t => t.Photos).Include(t => t.Comments).Include(t => t.FavoritesProducts));
                    }
                    else products.AddRange(_context.Products.Where(t => t.Category == category1).Include(t => t.Photos).Include(t => t.Comments).Include(t => t.FavoritesProducts));
                }
            }
            List<Product> topProductsView = new List<Product>();
            if (products.Count() > 10)
            {
                IEnumerable<Product> products1 = (from Products in products orderby Products.NumberOfViews select Products).Reverse();
                topProductsView = products1.ToList().GetRange(0, 10);
            }
            else topProductsView = products;

            if (_userManager.GetUserId(User) != null)
                ViewBag.UserId = _userManager.GetUserId(User).ToString();
            IQueryable<FavoritesProducts> favoritesProducts = _context.FavoritesProducts.Include(t => t.Product).ThenInclude(t => t.Comments).Include(t => t.Product)
                .ThenInclude(t => t.Discount).Include(t => t.Product).ThenInclude(t => t.Photos).Include(t => t.Product).Where(t => t.UserId == _userManager
                .GetUserId(User)).OrderBy(t => t.Date).Reverse();

            IQueryable<LastViews> lastViewsProducts = _context.LastViews.Include(t => t.Product).ThenInclude(t => t.Comments).Include(t => t.Product)
                .ThenInclude(t => t.Discount).Include(t => t.Product).ThenInclude(t => t.Photos).Include(t => t.Product).Where(t => t.UserId == _userManager
                .GetUserId(User)).OrderBy(t => t.Date).Reverse();
            return View(new CategoryProductsViewModel
            {
                FavoritesProducts = favoritesProducts,
                LastViews = lastViewsProducts,
                Category = category,
                PopularProducts = topProductsView
            });
        }*/

        [HttpGet]
        public async Task<IActionResult> GetProductsWithCategory(CategoryProductsViewModel viewModel, int? startPrice, int? endPrice, int manufacturerId = 0)
        {
            int startPriceWithDiscount = 0;
            int endPriceWithDiscount = 0;
            ViewBag.UserId = _context.Users.Where(t => t.UserName == User.Identity.Name).FirstOrDefault().Id.ToString();
            Category category = new Category();
            List<Product> products = null;
            if (!viewModel.ManufacturerIds.Contains(manufacturerId))
            {
                viewModel.ManufacturerIds.Add(manufacturerId);
            }

            Manufacturer selectedManufacturer = _context.Manufacturers.Where(t => t.Id == manufacturerId).FirstOrDefault();


            category = _context.Categories.Where(t => t.Title == viewModel.CurrentCategory).FirstOrDefault();

            await _context.Entry(category).Collection(t => t.ChildCategories).Query().Include(t => t.Photo).LoadAsync();


            if (category.ChildCategories.Count() != 0)
                products = _context.Products.Where(t => t.Category.ParentCategory.Title == viewModel.CurrentCategory).Include(t => t.Photos).Include(t => t.Comments).Include(t => t.Manufacturer).Include(t => t.FavoritesProducts).ToList();
            else
                products = _context.Products.Where(t => t.Category.Title == viewModel.CurrentCategory).Include(t => t.Photos).Include(t => t.Comments).Include(t => t.Manufacturer).Include(t => t.FavoritesProducts).ToList();

            if (products != null)
            {

                Filter filter = new Filter();

                if (products.Count() > 0)
                {
                    filter.StartPrice = (int)products.OrderBy(t => t.Price).Select(t => t.Price).Min();
                    filter.EndPrice = (int)products.OrderBy(t => t.Price).Select(t => t.Price).Max();
                    if (products.Where(t => t.PriceWithDiscount != 0).Any())
                    {
                        startPriceWithDiscount = (int)products.Where(t => t.PriceWithDiscount != 0).OrderBy(t => t.PriceWithDiscount).Select(t => t.PriceWithDiscount).Min();
                        if (filter.StartPrice > startPriceWithDiscount)
                            filter.StartPrice = startPriceWithDiscount;
                        endPriceWithDiscount = (int)products.Where(t => t.PriceWithDiscount != 0).OrderBy(t => t.PriceWithDiscount).Select(t => t.PriceWithDiscount).Max();
                        if (filter.EndPrice < startPriceWithDiscount)
                            filter.EndPrice = startPriceWithDiscount;
                    }



                }
                List<Manufacturer> SelManufacturers = new List<Manufacturer>();
                foreach (Product product in products)
                {
                    if (!SelManufacturers.Contains(product.Manufacturer))
                    {
                        SelManufacturers.Add(product.Manufacturer);
                    }
                }

                filter.Manufacturers = new List<Manufacturer>();

                if (SelManufacturers.Count() != 0)
                {
                    filter.Manufacturers.AddRange(SelManufacturers);
                }
                if (manufacturerId != 0)
                {
                    products = products.Where(t => viewModel.ManufacturerIds.Contains(t.ManufacturerId.Value)).ToList();
                }
                if (viewModel.SelectedManufacturers.Count > 0)
                    products = products.Where(t => viewModel.SelectedManufacturers.Contains(t.ManufacturerId.Value)).ToList();

                List<Product> productsWithDisc;
                if (startPrice != null && endPrice != null)
                {
                    productsWithDisc = products.Where(t => t.PriceWithDiscount != 0 && t.PriceWithDiscount >= startPrice && t.PriceWithDiscount <= endPrice).ToList();
                    products = products.Where(t => t.PriceWithDiscount == 0 && t.Price >= startPrice && t.Price <= endPrice).ToList();
                    products.AddRange(productsWithDisc);
                }

                if (_userManager.GetUserId(User) != null)
                    ViewBag.UserId = _userManager.GetUserId(User).ToString();

                return View(new CategoryProductsViewModel
                {
                    FavoritesProducts = MyRequest.GetFavoritesProducts(_context, _userManager.GetUserId(User)),
                    LastViews = MyRequest.GetLastViewsProducts(_context, _userManager.GetUserId(User)),
                    Manufacturer = selectedManufacturer,
                    CurrentCategory = category.Title,
                    Filter = filter,
                    Manufacturers = SelManufacturers,
                    Category = category,
                    AllProductsWithCategory = products,
                    ManufacturerIds = viewModel.ManufacturerIds,
                    SelectedManufacturers = viewModel.SelectedManufacturers
                });
            }
            else return NotFound();
        }






        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }



    }
}
