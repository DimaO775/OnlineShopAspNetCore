using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            async Task AddMainPhoto(IFormFile photo, bool flag)
            {
                string path = $"/Files/{photo.FileName}";
                if (!_context.Photos.Where(t => t.PhotoUrl == path).Any())
                {
                    using (var fileStream = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                    }
                    await _context.Photos.AddAsync(new() { Filename = photo.FileName, PhotoUrl = path, IsMain = flag, IsSlider = true });
                }
                else _context.Photos.Where(t => t.PhotoUrl == path).FirstOrDefault().IsSlider = true;
                await _context.SaveChangesAsync();
            }

            if (mainPhoto != null)
            {
                await AddMainPhoto(mainPhoto, true);
            }
            else if (anotherPhoto != null)
            {
                await AddMainPhoto(anotherPhoto, false);
            }
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> Edit(int? id, string returnUrl, IFormFile mainPhoto)
        {

            if (mainPhoto != null)
            {
                string path = $"/Files/{mainPhoto.FileName}";
                _context.Photos.Where(t => t.Id == id).FirstOrDefault().IsMain = false;
                using (FileStream fileStream = new FileStream(hostEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await mainPhoto.CopyToAsync(fileStream);
                }
                try
                {
                    _context.Photos.Where(t => t.PhotoUrl == path).FirstOrDefault().IsMain = true;
                }
                catch
                {
                    _context.Photos.Add(new() { Filename = mainPhoto.FileName, PhotoUrl = path, IsMain = true, IsSlider = true });
                }
                await _context.SaveChangesAsync();
            }
            return Redirect(returnUrl);

        }

        public async Task<IActionResult> Remove(int[] id, string returnUrl)
        {
            foreach (int idPhoto in id)
                _context.Photos.Remove(_context.Photos.Where(t => t.Id == idPhoto).FirstOrDefault());

            await _context.SaveChangesAsync();
            return Redirect(returnUrl);
        }
    }
}
