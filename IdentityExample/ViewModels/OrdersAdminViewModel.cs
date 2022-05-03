using IdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class OrdersAdminViewModel
    {
        public Order Order { get; set; }
        public string returnUrl { get; set; }
        public string paymentMethod { get; set; }

    }
}
