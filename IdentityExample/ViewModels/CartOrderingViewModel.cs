using IdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class CartOrderingViewModel
    {
        public Order Order { get; set; }

        public Cart Cart { get; set; }                 

        public string ReturnUrl{ get; set; }


    }
}
