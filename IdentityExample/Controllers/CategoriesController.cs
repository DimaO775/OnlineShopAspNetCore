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

namespace IdentityExample.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly IWebHostEnvironment hostEnvironment;
        public CategoriesController(ShopDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var shopDbContext = _context.Categories.Include(t => t.Photo).Include(t=>t.ParentCategory);
            return View(await shopDbContext.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.Where(t => t.Id == id).Include(t => t.Photo).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["ParentCategory"] = new SelectList(new[] { new {Id=0, Title="Нет" } }
            .Union(_context.Categories.Select(t=>new {t.Id, t.Title })), "Id", "Title");
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
                    string path = "/Files/" + Photo.FileName;
                    if (!_context.Photos.Where(t => t.PhotoUrl == path).Any())
                    {

                        using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await Photo.CopyToAsync(fs);
                        }                       
                        Photo photo = new Photo { Filename = Photo.FileName, PhotoUrl = path, CategoryId = category.Id, IsMain = false };
                        _context.Photos.Add(photo);
                        await _context.SaveChangesAsync();
                    }
                    else if(_context.Photos.Where(t => t.PhotoUrl == path && t.CategoryId != category.Id).Any())
                        _context.Photos.Where(t => t.PhotoUrl == path).FirstOrDefault().CategoryId = category.Id;
                    
                    await _context.SaveChangesAsync();

                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Id", category.ParentCategoryId);
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Title", category.ParentCategoryId);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ParentCategoryId")] Category category, IFormFile Photo)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                       
                    if (Photo != null)
                    {
                        string path = "/Files/" + Photo.FileName;
                        Photo photo = new Photo { Filename = Photo.FileName, PhotoUrl = path, CategoryId = category.Id, IsMain = false };
                        if (_context.Photos.Where(t => t.Id == category.Id).Any())
                        {
                            using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                            {
                                await Photo.CopyToAsync(fs);
                            }
                            _context.Photos.Where(t => t.Id == category.Id).FirstOrDefault().CategoryId = null;                           
                            _context.Photos.Add(photo);
                        }
                        else _context.Photos.Add(photo);

                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Title", category.ParentCategoryId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            await _context.Entry(category).Collection(t => t.ChildCategories).LoadAsync();
            _context.Categories.RemoveRange(category.ChildCategories);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> GetCategories(string title)
        {

            Category category = _context.Categories.Where(t => t.Title == title).FirstOrDefault();
           // List<Category> categories = _context.Categories.Where(t => t.Title == title).FirstOrDefault().ChildCategories.ToList();
            List<Product> products = _context.Products.Where(t=>t.Category.Title == title).Include(t=>t.Photos).Include(t=>t.Comments).ToList();
            
            /*List<Photo> photos = _context.Categories.Where(t => t.Title == title).FirstOrDefault().ChildCategories;*/
            await _context.Entry(category).Collection(t => t.ChildCategories).Query().Include(t=>t.Photo).Include(t=>t.ChildCategories).Include(t=>t.Photo).Include(t=>t.Products).LoadAsync();
            if (category.ChildCategories.Count() != 0)
            {
                foreach (Category category1 in category.ChildCategories)
                {
                    if (category1.ChildCategories.Count() != 0)
                    {
                        foreach (Category category2 in category1.ChildCategories)
                            products.AddRange(_context.Products.Where(t => t.Category == category2).Include(t=>t.Photos).Include(t=>t.Comments));
                        /*_context.Entry(products).Collection(t => t.Include(m => m.Category.ChildCategories)*/
                    }
                    else products.AddRange(_context.Products.Where(t => t.Category == category1).Include(t => t.Photos).Include(t => t.Comments));
                }
            }
            List<Product> topProductsView = new List<Product>();
            if (products.Count() > 10)
            {
                IEnumerable<Product> products1 = (from Products in products orderby Products.NumberOfViews select Products).Reverse();
                topProductsView = products1.ToList().GetRange(0, 10);
            }
            else topProductsView = products;
            
            return View(new CategoryProductsViewModel
            {
                Category = category,
                PopularProducts = topProductsView
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsWithCategory(CategoryProductsViewModel viewModel, int? startPrice, int? endPrice, 
            int[] manufacturersId, int[] Ids, int manufacturerId = 0)
        {
            Category category = new Category();
            IQueryable<Product> products = null;
            if (!viewModel.ManufacturerIds.Contains(manufacturerId))
            {
                viewModel.ManufacturerIds.Add(manufacturerId);
            }
            
            Manufacturer selectedManufacturer = _context.Manufacturers.Where(t => t.Id == manufacturerId).FirstOrDefault();
            

                category = _context.Categories.Where(t => t.Title == viewModel.CurrentCategory).FirstOrDefault();

                await _context.Entry(category).Collection(t => t.ChildCategories).Query().Include(t => t.Photo).LoadAsync();


                if (category.ChildCategories.Count() != 0)
                    products = _context.Products.Where(t => t.Category.ParentCategory.Title == viewModel.CurrentCategory).Include(t => t.Photos).Include(t => t.Comments).Include(t => t.Manufacturer);
                else
                    products = _context.Products.Where(t => t.Category.Title == viewModel.CurrentCategory).Include(t => t.Photos).Include(t => t.Comments).Include(t => t.Manufacturer);
                
            if (products != null)
            {

                Filter filter = new Filter();

                if (products.Count() > 0)
                {
                    filter.StartPrice = (int)products.OrderBy(t => t.Price).Select(t => t.Price).Min();
                    filter.EndPrice = (int)products.OrderBy(t => t.Price).Select(t => t.Price).Max();
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

                if (startPrice != null && endPrice != null)
                {
                    products = products.Where(t => t.Price >= startPrice && t.Price <= endPrice);
                }
                if (manufacturerId != 0)
                {
                    products = products.Where(t => viewModel.ManufacturerIds.Contains(t.ManufacturerId.Value));
                }
                return View(new CategoryProductsViewModel
                {
                    Ids = Ids,
                    Test = viewModel.Test,
                    Manufacturer = selectedManufacturer,
                    CurrentCategory = category.Title,
                    Filter = filter,
                    Manufacturers = SelManufacturers,
                    Category = category,
                    AllProductsWithCategory = products,
                    ManufacturerIds = viewModel.ManufacturerIds
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
