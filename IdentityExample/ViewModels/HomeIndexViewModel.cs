using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityExample.Models;
using PagedList;
using PagedList.Core;
namespace IdentityExample.ViewModels
{
    public class HomeIndexViewModel
    {
        public IQueryable<FavoritesProducts> FavoritesProducts { get; set; }
        public IQueryable<LastViews> LastViews { get; set; }
        public List<Photo> Photos { get; set; }

        public string CurrentCategory { get; set; }


    }
}
