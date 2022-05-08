using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityExample.Models
{
    public class User : IdentityUser
    {
        public int YearOfBirth { get; set; }


        public List<Comment> Comments { get; set; }
        public List<Support> Support { get; set; }
        public List<Order> Orders { get; set; }
        public List<FavoritesProducts> FavoritesProducts { get; set; }
        public List<LastViews> LastViews { get; set; }
    }
}
