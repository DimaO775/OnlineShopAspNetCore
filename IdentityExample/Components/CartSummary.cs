using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityExample.Models;
using IdentityExample.Extensions;

namespace IdentityExample.Components
{
    [ViewComponent]
    public class CartSummaryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            Cart cart = new Cart();
            var cartItems = HttpContext.Session.Get<IEnumerable<CartItem>>("cart");
            if (cartItems == null)
                cartItems = new List<CartItem>();
            cart.AddItems(cartItems);
            return View(cart);
        }
    }
}
