using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityExample.Models;
using Microsoft.AspNetCore.Http;
using IdentityExample.Extensions;
using IdentityExample.ViewModels;
using IdentityExample.Services;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Html;

namespace IdentityExample.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopDbContext context;
        private readonly IEmailService emailService;
        private readonly UserManager<User> userManager;

        public CartController(ShopDbContext context, IEmailService emailService, UserManager<User> userManager)
        {
            this.context = context;
            this.emailService = emailService;
            this.userManager = userManager;
        }
        public IActionResult Index(Cart cart, string returnUrl)
         {
            //Cart cart = GetCart();
            return View(new CartIndexViewModel { Cart = cart, ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, Cart cart, string returnUrl)
        {
            Product product = await context.Products.FindAsync(id);
            await context.Entry(product).Collection(t => t.Photos).LoadAsync();
            if (product != null)
            {
                if (cart.CartItems.Where(t => t.Product.Id == product.Id).Any())
                {
                    if (product.Quantity >= cart.CartItems.Where(t => t.Product.Id == product.Id).FirstOrDefault().Quantity + 1)
                        cart.AddItem(product, 1);
                }
                else cart.AddItem(product, 1);
                UpdateCart(cart);
                /*context.Products.Where(t => t.Id == id).FirstOrDefault().Quantity -= 1;
                await context.SaveChangesAsync();*/
            }

            return Redirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id, Cart cart, string returnUrl)
        {
            Product product = await context.Products.FindAsync(id);
            if(product!=null)
            {
                cart.RemoveItem(product);
                UpdateCart(cart);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public async Task<IActionResult> ChangeQuantity(int id, Cart cart, int quantity, string returnUrl)
        {
            int quantityChange = quantity - cart.CartItems.Where(t => t.Product.Id == id).FirstOrDefault().Quantity;
            Product product = context.Products.Where(t => t.Id == id).FirstOrDefault();
            if (quantity > 0 && quantityChange <= product.Quantity)
            {                
                cart.CartItems.Where(t => t.Product.Id == id).FirstOrDefault().Quantity = quantity;
                context.Products.Where(t => t.Id == id).FirstOrDefault().Quantity -= quantityChange;
                await context.SaveChangesAsync();
            }
            else if(quantity > 0 && quantityChange >= product.Quantity)
            {
                cart.CartItems.Where(t => t.Product.Id == id).FirstOrDefault().Quantity += context.Products.Where(t => t.Id == id).FirstOrDefault().Quantity;
                context.Products.Where(t => t.Id == id).FirstOrDefault().Quantity = 0;
                await context.SaveChangesAsync();
            }           
            else cart.RemoveItem(product);
            UpdateCart(cart);
            return RedirectToAction("Index", new CartIndexViewModel { ReturnUrl = returnUrl, Cart = cart});
        }



        public IActionResult SetCart()
        {
            Cart cart = new Cart();
            Product product1 = context.Products.Find(1);
            Product product2 = context.Products.Find(2);
            cart.AddItem(product1, 2);
            cart.AddItem(product2, 1);
            HttpContext.Session.Set<IEnumerable<CartItem>>("cart", cart.CartItems);
            return RedirectToAction("Index");
        }


        private Cart GetCart()
        {
            IEnumerable<CartItem> items = HttpContext.Session.Get<IEnumerable<CartItem>>("cart");
            if(items==null)
            {
                items = new List<CartItem>();
                HttpContext.Session.Set<IEnumerable<CartItem>>("cart", items);
            }
            Cart cart = new Cart();
            cart.AddItems(items);
            return cart;
        }
        

        private void UpdateCart(Cart cart)
        {
            HttpContext.Session.Set<IEnumerable<CartItem>>("cart", cart.CartItems);
        }


        
        

        /*[HttpPost]
        public async Task<IActionResult> ConfirmOrder(Cart cart, string returnUrl)
        {
            StringBuilder builder = new StringBuilder();
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            string emailTo = user.Email;
            builder.Append("<h3>Ваш заказ: </h3><ul>");
            builder.Append("<ul>");
            int i = 0;
            foreach(CartItem item in cart.CartItems)
            {
                builder.Append($"<li>{++i}. {item.Product.Title} - {item.Quantity} шт: {item.Product.Price} грн. </li>");
            }
            builder.Append("</ul>");
            builder.Append($"<h4>Итого:{cart.GetTotalSum()} грн. </h4>") ;
            await emailService.SendAsync("serhii.ruban81@gmail.com", emailTo, "Ваш заказ №237815 подтвержден", builder.ToString());
            cart.Clear();
            return Redirect(returnUrl);
        }*/
    }
}
