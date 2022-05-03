using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityExample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using PagedList.Core;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
//using AspNetCore;

namespace IdentityExample.Controllers
{

    public class ProductsController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly IWebHostEnvironment hostEnvironment;
        //create and field...
        public ProductsController(ShopDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }

        // GET: Products
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.Include(t => t.Photos).Include(p => p.Category).ToListAsync());

        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(d=>d.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Where(t => t.Id == id).FirstOrDefault().NumberOfViews = _context.Products.Where(t => t.Id == id).FirstOrDefault().NumberOfViews + 1;
            await _context.SaveChangesAsync();
            return View(product);
        }

        [HttpGet]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(new[] { new { Id = 0, Title = "Выбрать..." } }.Union(_context.Categories.Where(t=>t.ParentCategoryId == null).Select(t=>new { t.Id, t.Title})), "Id", "Title");
            ViewData["ManufacturerId"] = new SelectList(new[] { new { Id = 0, Title = "Выбрать..." } }.Union(_context.Manufacturers.Select(t => new { t.Id, t.Title })), "Id", "Title");

            return View();          
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Price,ShortDescription,LongDescription,ManufacturerId,Quantity")] Product product, 
            string[] CategoryId,IFormFileCollection Photos, IFormFile MainPhoto)
        {
            if (ModelState.IsValid)
            {
                product.CategoryId = int.Parse( CategoryId[^1]);
                _context.Add(product);

                await _context.SaveChangesAsync();

                if (MainPhoto != null)
                {
                    string path1 = "/Files/" + MainPhoto.FileName;
                    if (!_context.Photos.Where(t => t.PhotoUrl == path1).Any())
                    {
                        
                        using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path1, FileMode.Create))
                        {
                            await MainPhoto.CopyToAsync(fs);
                        }
                        Photo photo1 = new Photo { Filename = MainPhoto.FileName, PhotoUrl = path1, ProductId = product.Id, IsMain = true };
                        _context.Photos.Add(photo1);
                    }
                    await _context.SaveChangesAsync();
                }


                if (Photos != null)
                {
                    foreach(IFormFile file in Photos)
                    {
                        string path = "/Files/" + file.FileName;
                        if (!_context.Photos.Where(t => t.PhotoUrl == path).Any())
                        {

                            using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                            {
                                await file.CopyToAsync(fs);
                            }
                            Photo photo = new Photo { Filename = file.FileName, PhotoUrl = path, ProductId = product.Id, IsMain = false };
                            _context.Photos.Add(photo);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
          
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        [HttpGet]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           // List<Photo> photo = new List<Photo>(_context.Photos.Where(c=>c.ProductId == id));
            var product = await _context.Products.FindAsync(id);
           // product.Photos = new List<Photo>(_context.Photos.Where(c => c.ProductId == id));
            if (product == null)
            {
                return NotFound();
            }
            await _context.Entry(product).Collection(c => c.Photos).LoadAsync();
            ViewData["CategoryId"] = new SelectList(new[] { new { Id = 0, Title = "Выбрать..."} }
                .Union(_context.Categories.Where(t => t.ParentCategoryId == null)
                .Select(t => new { t.Id, t.Title })), "Id", "Title");
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> RemovePhotos(int id, Product product, int[] selectedPhotos, string returnUrl, IFormFileCollection Photos, IFormFile MainPhoto)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (selectedPhotos.Count() != 0)
                    {
                        foreach (int idPhoto in selectedPhotos)
                        {
                            _context.Photos.Remove(_context.Photos.Where(t => t.Id == idPhoto).FirstOrDefault());
                        }
                        
                    }

                    if (Photos.Count() != 0)
                    {
                        _context.Photos.RemoveRange(_context.Photos.Where(c => c.ProductId == product.Id && c.IsMain == false));
                        await _context.SaveChangesAsync();
                        foreach (IFormFile file in Photos)
                        {
                            string path = "/Files/" + file.FileName;
                            using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                            {
                                await file.CopyToAsync(fs);
                            }
                            Photo photo = new Photo { Filename = file.FileName, PhotoUrl = path, ProductId = product.Id };
                            _context.Photos.Add(photo);
                        }


                        await _context.SaveChangesAsync();
                    }

                    if (MainPhoto != null)
                    {
                        string path = "/Files/" + MainPhoto.FileName;
                        if (!_context.Photos.Where(c => c.ProductId == product.Id && c.PhotoUrl == path).Any())
                        {
                            Photo photo = new Photo
                            {
                                Filename = MainPhoto.FileName,
                                PhotoUrl = path,
                                ProductId = product.Id,
                                IsMain = true
                            };

                            _context.Photos.Add(photo);
                        }
                        else
                        {

                            _context.Photos.Where(c => c.PhotoUrl == path && c.ProductId == product.Id)
                                    .FirstOrDefault().IsMain = true;
                            _context.Photos.Where(c => c.IsMain && c.ProductId == product.Id)
                                    .FirstOrDefault().IsMain = false;
                        }
                        await _context.SaveChangesAsync();
                    }


                    _context.SaveChanges();
                    return Redirect(returnUrl);



                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return RedirectToAction(nameof(Index));
                        // return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(product);
        }


        [HttpPost]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> AddPhotos(int id, [Bind("Id,Title,Price,ShortDescription,LongDescription,Quantity")] Product product, string returnUrl, IFormFileCollection Photos, IFormFile MainPhoto)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(product);
                    //await _context.Entry(product).Collection(t => t.Photos).LoadAsync();
                    if (Photos.Count() != 0)
                    {
                        foreach (IFormFile file in Photos)
                        {
                            
                            string path = "/Files/" + file.FileName;
                            if (!_context.Photos.Where(t => t.PhotoUrl == path && t.ProductId == product.Id).Any())
                            {
                                using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                                {
                                    await file.CopyToAsync(fs);
                                }
                                Photo photo = new Photo { Filename = file.FileName, PhotoUrl = path, ProductId = product.Id };
                                _context.Photos.Add(photo);
                            }

                        }
                        await _context.SaveChangesAsync();
                    }

                    if (MainPhoto != null)
                    {
                        string path = "/Files/" + MainPhoto.FileName;
                        if (!_context.Photos.Where(c => c.ProductId == product.Id && c.PhotoUrl == path).Any())
                        {
                            Photo photo = new Photo
                            {
                                Filename = MainPhoto.FileName,
                                PhotoUrl = path,
                                ProductId = product.Id,
                                IsMain = true
                            };
                            if (_context.Photos.Where(c => c.ProductId == product.Id && c.IsMain).Any())
                            {
                                _context.Photos.Where(c => c.ProductId == product.Id && c.IsMain).FirstOrDefault().IsMain = false;
                            }
                            _context.Photos.Add(photo);
                            
                            
                            await _context.SaveChangesAsync();
                        }

                        else
                        {
                            if(_context.Photos.Where(c => c.IsMain && c.ProductId == product.Id).Any())
                                _context.Photos.Where(c => c.IsMain && c.ProductId == product.Id).FirstOrDefault().IsMain = false;
                            _context.Photos.Where(c => c.ProductId == product.Id && c.PhotoUrl == path).FirstOrDefault().IsMain = true;                           
                            await _context.SaveChangesAsync();
                        }
                        
                    }
                    
                    
                    return Redirect(returnUrl);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                        return RedirectToAction(nameof(Index));
                    else
                        throw;
                }
            }
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Price,ShortDescription,LongDescription,Quantity,PriceWithDiscount, NumberOfViews")] Product product, string[] CategoryId)
        {
            
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.CategoryId = int.Parse(CategoryId[^1]);
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return RedirectToAction(nameof(Index));
                       // return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        [Authorize(Roles = "manager,admin")]
        public IActionResult getSubCategories(int id)
        {
            //Category parentCategory = await _context.Categories.Wh(t => t.Id == id);
            var childCategories = _context.Categories.Where(t => t.ParentCategoryId == id);
            if (childCategories.Count() == 0)
                return NotFound();
            //await _context.Entry(parentCategory).Collection(t => t.ChildCategories).LoadAsync();
            ViewData["CategoryId"] = new SelectList(new[] { new { Id = 0, Title="Выбрать..." } }
            .Union(  childCategories.Select(t=>new { t.Id, t.Title })), "Id", "Title");
            return PartialView();

        }

        public async Task<IActionResult> GetComments(int? id)
        {
            Product product = await _context.Products.Include(t=>t.Photos).Where(c=>c.Id == id).FirstOrDefaultAsync();
            if (product == null)
                return NotFound();
            //_context.Entry(product).Collection(t => t.Photos);
            await _context.Entry(product).Collection(t => t.Comments).Query().Include(t => t.User).Include(t=>t.ChildComments).LoadAsync();
            return View(product);
        }

        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> ChangeQuantity(int? id, int quantity)
        {
            Product product = await _context.Products.Where(c => c.Id == id).FirstOrDefaultAsync();
            product.Quantity = quantity;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        /*        public async Task<IActionResult> GetAll(int id)
                {
                    Product product = await _context.Products.FindAsync(id);
                    //Comment comment = await _context.Comments.FindAsync(id);
                    if (product == null)
                        return NotFound();
                    await _context.Entry(product).Collection(t => t.Comments).LoadAsync();
                    await _context.Entry(product).Collection(t => t.Photos).LoadAsync();
                    return PartialView("GetAll", product);
                }*/

        /*        public async Task<IActionResult> GetInfo(int id)
                {
                    Product product = await _context.Products.FindAsync(id);
                    if (product == null)
                        return NotFound();
                    await _context.Entry(product).Collection(t => t.Photos).LoadAsync();
                    return PartialView("GetInfo", product);
                }*/
        public async Task<IActionResult> GetPhoto(int? id)
        {
            Product product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            await _context.Entry(product).Collection(t => t.Photos).LoadAsync();
            return View(product);
        }
    }
}
