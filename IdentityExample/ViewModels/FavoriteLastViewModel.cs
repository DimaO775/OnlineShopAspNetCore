using IdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class FavoriteLastViewModel
    {
        public IQueryable<FavoritesProducts> FavoritesProducts { get; set; }
        public IQueryable<LastViews> LastViews { get; set; }
    }
}
