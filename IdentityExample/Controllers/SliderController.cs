using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    [Authorize(Roles = "admin")]
    public class SliderController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public SliderController(ShopDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            List<Photo> photos = _context.Photos.Where(t => t.IsSlider).ToList();
            return View(photos);
        }

        public async Task<IActionResult> Add(string returnUrl, IFormFile mainPhoto, IFormFile anotherPhoto)
        {
            if (mainPhoto != null)
            {
                string path = "/Files/" + mainPhoto.FileName;
                if (!_context.Photos.Where(t => t.PhotoUrl == path).Any())
                {

                    using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await mainPhoto.CopyToAsync(fs);
                    }
                    Photo photo = new Photo { Filename = mainPhoto.FileName, PhotoUrl = path, IsMain = true, IsSlider = true };
                    _context.Photos.Add(photo);
                    await _context.SaveChangesAsync();
                }
                else _context.Photos.Where(t => t.PhotoUrl == path).FirstOrDefault().IsSlider = true;
                await _context.SaveChangesAsync();
            }

            else if (anotherPhoto != null)
            {
                string path = "/Files/" + anotherPhoto.FileName;
                if (!_context.Photos.Where(t => t.PhotoUrl == path).Any())
                {

                    using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await anotherPhoto.CopyToAsync(fs);
                    }
                    Photo photo = new Photo { Filename = anotherPhoto.FileName, PhotoUrl = path, IsMain = false, IsSlider = true };
                    _context.Photos.Add(photo);
                }
                else _context.Photos.Where(t => t.PhotoUrl == path).FirstOrDefault().IsSlider = true;
                await _context.SaveChangesAsync();
            }
                return Redirect(returnUrl);
        }

        public async Task<IActionResult> Edit(int? id, string returnUrl, IFormFile mainPhoto)
        {
            if (mainPhoto != null)
            {
                string path = "/Files/" + mainPhoto.FileName;
                _context.Photos.Where(t => t.Id == id).FirstOrDefault().IsMain = false;
                await _context.SaveChangesAsync();
                using (FileStream fs = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await mainPhoto.CopyToAsync(fs);
                }
                Photo photo = new Photo { Filename = mainPhoto.FileName, PhotoUrl = path, IsMain = true, IsSlider = true };
                if (_context.Photos.Where(t => t.PhotoUrl == path).Any())
                    _context.Photos.Where(t => t.PhotoUrl == path).FirstOrDefault().IsMain = true;
                else 
                    _context.Photos.Add(photo);
                await _context.SaveChangesAsync();
            }
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> Remove(int[] id, string returnUrl)
        {
            foreach(int idPhoto in id)
                _context.Photos.Remove(_context.Photos.Where(t => t.Id == idPhoto).FirstOrDefault());
            await _context.SaveChangesAsync();
            return Redirect(returnUrl);
        }
    }
}
