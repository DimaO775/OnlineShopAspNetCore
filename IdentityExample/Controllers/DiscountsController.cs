using IdentityExample.Models;
using IdentityExample.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    [Authorize(Roles ="admin,manager")]
    public class DiscountsController : Controller
    {
        private readonly ShopDbContext _context;
        public DiscountsController(ShopDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Discounts> discounts = _context.Discounts.Include(t => t.Products).Include(t => t.Categories).Include(t => t.Manufacturers).ToList();
            foreach(Discounts discount in discounts)
            {
                if (discount.Products.Count() == 0 && discount.Categories.Count() == 0 && discount.Manufacturers.Count() == 0)
                    _context.Discounts.Where(t => t.Id == discount.Id).FirstOrDefault().IsActive = false;               
            }
            await _context.SaveChangesAsync();
            return View("Index", discounts);
        }

        public async Task<IActionResult> AddDiscount(int value, string name)
        {
            await _context.Discounts.AddAsync(new Discounts { Name = name, Value = value });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ControlDiscount(int discountId)
        {
            ViewData["CategoryId"] = new SelectList(new[] { new { Id = 0, Title = "Выбрать..." } }.Union(_context.Categories.Where(t => t.ParentCategoryId != null).Select(t => new { t.Id, t.Title })), "Id", "Title");
            ViewData["ManufacturerId"] = new SelectList(new[] { new { Id = 0, Title = "Выбрать..." } }.Union(_context.Manufacturers.Select(t => new { t.Id, t.Title })), "Id", "Title");
            
            Discounts discount =await _context.Discounts.Where(t => t.Id == discountId).FirstOrDefaultAsync();

            List<Product> products = _context.Products.Where(t => t.DiscountId == discountId).ToList();

            List<Category> categories = _context.Categories.Where(t => t.DiscountId == discountId).ToList();

            List<Manufacturer> manufacturers = _context.Manufacturers.Where(t => t.DiscountId == discountId).ToList();

            return View(new DiscountsViewModel {Discount = discount, Categories = categories, Manufacturers = manufacturers, Products = products });
        }
        public async Task<IActionResult> Activate(int? discountId, int? categoryId, int? manufacturerId, int? productId, int? procent, string returnUrl)
        {
            double value;
            if (discountId != null)
            {
                if (categoryId != null && categoryId.Value != 0)
                {
                    Category mainCat = _context.Categories.Where(t => t.Id == categoryId).Include(t => t.Products).Include(t=>t.ChildCategories).FirstOrDefault();
                    await _context.Entry(mainCat).Collection(t => t.ChildCategories).Query().Include(t=>t.Products).LoadAsync();
                    if (mainCat.ChildCategories.Count() > 0)
                    {
                        foreach (Category category in mainCat.ChildCategories)
                        {
                            _context.Categories.Where(t => t.Id == category.Id).FirstOrDefault().DiscountId = discountId;
                        }
                    }
                    else _context.Categories.Where(t => t.Id == mainCat.Id).FirstOrDefault().DiscountId = discountId;
                    _context.Discounts.Where(t => t.Id == discountId).FirstOrDefault().IsActive = true;
                    await _context.SaveChangesAsync();
                    List<Category> categories = _context.Categories.Where(t => t.DiscountId == discountId).Include(t=>t.Products).ToList();
                    value = (100 - _context.Discounts.Where(t => t.Id == discountId).FirstOrDefault().Value) / 100;

                    foreach (Category cat in categories)
                        if (cat.Products.Count() > 0)
                            foreach (Product product in cat.Products)
                            {
                                if (_context.Products.Where(t => t.Id == product.Id).FirstOrDefault().PriceWithDiscount == 0)
                                {
                                    _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().PriceWithDiscount =
                                        Math.Round(_context.Products.Where(t => t.Id == product.Id).FirstOrDefault().Price * value, 0);
                                    _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().DiscountId = discountId;
                                }
                            }
                    await _context.SaveChangesAsync();
                }

                if (manufacturerId != null && manufacturerId.Value != 0)
                {
                    
                    _context.Manufacturers.Where(t => t.Id == manufacturerId).FirstOrDefault().DiscountId = discountId;
                    _context.Discounts.Where(t => t.Id == discountId).FirstOrDefault().IsActive = true;
                    await _context.SaveChangesAsync();
                    value = (100 - _context.Discounts.Where(t => t.Id == discountId).FirstOrDefault().Value) / 100;
                    Manufacturer manufacturer = _context.Manufacturers.Where(t => t.Id == manufacturerId).Include(t => t.Products).FirstOrDefault();
                    await _context.Entry(manufacturer).Collection(t => t.Products).LoadAsync();
                    foreach (Product product in manufacturer.Products)
                    {
                        if (_context.Products.Where(t => t.Id == product.Id).FirstOrDefault().PriceWithDiscount == 0)
                        {
                            _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().PriceWithDiscount =
                                Math.Round(_context.Products.Where(t => t.Id == product.Id).FirstOrDefault().Price * value, 0);
                            _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().DiscountId = discountId;
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                
            }
            if (productId != null && productId.Value != 0 && procent != null)
            {
                value = (100 - (double)procent) / 100;
                if (procent.Value != 0)
                {
                    if (_context.Products.Where(t => t.Id == productId).FirstOrDefault().DiscountId == null)
                        _context.Products.Where(t => t.Id == productId).FirstOrDefault().PriceWithDiscount = Math.Round(_context.Products.Where(t => t.Id == productId).FirstOrDefault().Price * value, 0);
                    else
                    {
                        _context.Products.Where(t => t.Id == productId).FirstOrDefault().DiscountId = null;
                        _context.Products.Where(t => t.Id == productId).FirstOrDefault().PriceWithDiscount = Math.Round(_context.Products.Where(t => t.Id == productId).FirstOrDefault().Price * value, 0);
                    }
                }

                else _context.Products.Where(t => t.Id == productId).FirstOrDefault().PriceWithDiscount = 0;
                await _context.SaveChangesAsync();
            }

            return Redirect(returnUrl);
        }
        public async Task<IActionResult> Disable(int id, int? categoryId, int? manufacturerId, int? productId, string returnUrl)
        {
            if (categoryId != null)
            {
                Category mainCat = _context.Categories.Where(t => t.Id == categoryId).Include(t => t.Products).Include(t => t.ChildCategories).FirstOrDefault();
                await _context.Entry(mainCat).Collection(t => t.Products).LoadAsync();
                foreach (Product product in mainCat.Products)
                {
                    if (_context.Products.Where(t => t.Id == product.Id).FirstOrDefault().DiscountId == id)
                    {
                        _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().PriceWithDiscount = 0;
                        _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().DiscountId = null;
                    }
                }
                await _context.SaveChangesAsync();
                _context.Categories.Where(t => t.Id == categoryId).FirstOrDefault().DiscountId = null;

            }
            if (manufacturerId != null)
            {
                await _context.SaveChangesAsync();
                Manufacturer manufacturer = _context.Manufacturers.Where(t => t.Id == manufacturerId).Include(t => t.Products).FirstOrDefault();
                await _context.Entry(manufacturer).Collection(t => t.Products).LoadAsync();
                foreach (Product product in manufacturer.Products)
                {
                    if (_context.Products.Where(t => t.Id == product.Id).FirstOrDefault().DiscountId == id)
                    {
                        _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().PriceWithDiscount = 0;
                        _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().DiscountId = null;
                    }
                }
                _context.Manufacturers.Where(t => t.Id == manufacturerId).FirstOrDefault().DiscountId = null;
            }
            if (productId != null)
            {
                _context.Products.Where(t => t.Id == productId).FirstOrDefault().PriceWithDiscount = 0;
                _context.Products.Where(t => t.Id == productId).FirstOrDefault().DiscountId = null;
            }
            await _context.SaveChangesAsync();
            return Redirect(returnUrl);
        }
        public async Task<IActionResult> DisableAll(string returnUrl, int id)
        {
            List<Product> products = _context.Products.Where(t => t.DiscountId == id || t.PriceWithDiscount != 0).ToList();
            List<Category> categories = _context.Categories.Where(t => t.DiscountId == id).Include(t=>t.Products).ToList();
            List<Manufacturer> manufacturers = _context.Manufacturers.Where(t => t.DiscountId == id).Include(t => t.Products).ToList();
            if(products.Count() != 0)
                foreach(Product product in products)
                    _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().DiscountId = null;

            if (categories.Count() != 0)
                foreach (Category category in categories)
                {
                    _context.Categories.Where(t => t.Id == category.Id).FirstOrDefault().DiscountId = null;
                    foreach (Product product in category.Products)
                        _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().PriceWithDiscount = 0;
                }
            if (manufacturers.Count() != 0)
                foreach (Manufacturer manufacturer in manufacturers)
                {
                    _context.Manufacturers.Where(t => t.Id == manufacturer.Id).FirstOrDefault().DiscountId = null;
                    foreach (Product product in manufacturer.Products)
                        _context.Products.Where(t => t.Id == product.Id).FirstOrDefault().PriceWithDiscount = 0;
                }
            await _context.SaveChangesAsync();
            return Redirect(returnUrl);
        }
        public async Task<IActionResult> Remove(int discountId)
        {
            Discounts discount = await _context.Discounts.Where(t => t.Id == discountId).FirstOrDefaultAsync();
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
