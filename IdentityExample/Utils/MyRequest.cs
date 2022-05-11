using Hangfire.Common;
using IdentityExample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Utils
{
    public static class MyRequest
    {

        public static IQueryable<FavoritesProducts> GetFavoritesProducts(ShopDbContext _context, string userId) => _context.FavoritesProducts
            .Include(t => t.Product).ThenInclude(t => t.Comments)
            .Include(t => t.Product).ThenInclude(t => t.Discount)
            .Include(t => t.Product).ThenInclude(t => t.Photos)
            .Include(t => t.Product).Where(t => t.UserId == userId).OrderBy(t => t.Date).Reverse();

        public static IQueryable<LastViews> GetLastViewsProducts(ShopDbContext _context, string userId) => _context.LastViews
            .Include(t => t.Product).ThenInclude(t => t.Comments)
            .Include(t => t.Product).ThenInclude(t => t.Discount)
            .Include(t => t.Product).ThenInclude(t => t.Photos)
            .Include(t => t.Product).Where(t => t.UserId == userId).OrderBy(t => t.Date).Reverse();
    }
}
