using IdentityExample.Models;
using IdentityExample.Services;
using IdentityExample.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class OrderController : Controller
    {


        private readonly ShopDbContext _context;
        private readonly IEmailService emailService;
        private readonly UserManager<User> userManager;

        public OrderController(ShopDbContext context, IEmailService emailService, UserManager<User> userManager)
        {
            this._context = context;
            this.emailService = emailService;
            this.userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> Ordering(Cart cart, string returnUrl)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            return View("Ordering", new CartOrderingViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(CartOrderingViewModel viewModel, string street, string house, string apartment)
        {
            Payment payment = new Payment();
            Order order = new Order();
            Random random = new Random();
            List<Order> orders = new List<Order>();
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            string address = street + house + apartment;
            int numOrder = random.Next(100000000, 999999999);

            order.Name = viewModel.Order.Name;
            order.Surname = viewModel.Order.Surname;
            order.City = viewModel.Order.City;
            order.NumberOfPhone = viewModel.Order.NumberOfPhone;
            order.Address = address;
            order.Email = user.Email;
            order.Number = numOrder;
            order.Price = viewModel.Cart.GetTotalSum();
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            
            foreach (CartItem item in viewModel.Cart.CartItems)
            {
                await _context.OrderItems.AddAsync(new OrderItem { ProductId = item.Product.Id, Quantity = item.Quantity, 
                    OrderId = _context.Orders.Where(t=>t.Number == numOrder).FirstOrDefault().Id });
                await _context.SaveChangesAsync();
            }
            /*return RedirectToAction("Payment", "Order", new { number = numOrder});*/



            var url = Url.Action
                (
                    "Payment",
                    "Order",
                    new { number = order.Number },
                    protocol: HttpContext.Request.Scheme
                );

            StringBuilder builder = new StringBuilder();
            string emailTo = user.Email;
            builder.Append("<h3>Ваш заказ: </h3><ul>");
            builder.Append("<ul>");
            int i = 0;
            foreach (CartItem item in viewModel.Cart.CartItems)
            {
                builder.Append($"<li>{++i}. {item.Product.Title} - {item.Quantity} шт: {item.Product.Price} грн. </li>");
            }
            builder.Append("</ul>");
            builder.Append($"<h4>Итого:{viewModel.Cart.GetTotalSum()} грн. </h4>");
            builder.Append($"<a style='color: white; height:50px; background-color: green; font-size:30px; " +
                $"font-weight:600; padding:10px; text-decoration: none;' href='{url}'>ПОДТВЕРДИТЬ</a>");
            await emailService.SendEmailAsync(emailTo, "Подтвердите ваш заказ под № " + numOrder, builder.ToString());
            return RedirectToAction("Index", "Home");


        }
        [HttpGet]
        public async Task<IActionResult> Payment(int number)
        {
            

            Order order = await _context.Orders.Where(t=>t.Number == number).FirstOrDefaultAsync();
            PaymentOrderViewModel viewModel = new PaymentOrderViewModel {Order = order};
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> PaymentTrue(PaymentOrderViewModel viewModel)
        {
            int paymentMethodId = 1;
            string valiUntil = viewModel.ValidUntilMonth + "/" + viewModel.ValidUntilYear;
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            Order order = await _context.Orders.Where(t => t.Id == viewModel.Order.Id).Include(t=>t.OrderItems).FirstOrDefaultAsync();
            foreach(OrderItem orderItem in order.OrderItems)
            {
                _context.Products.Where(t => t.Id == orderItem.ProductId).FirstOrDefault().Quantity -= orderItem.Quantity;
            }
            await _context.SaveChangesAsync();
            if (viewModel.PaymentCard != null)
            {
                paymentMethodId = 2;
            }
            if (viewModel.PaymentPayPal != null)
            {
                paymentMethodId = 3;
            }
            Payment payment = new Payment
            {
                Date = DateTime.Now.ToString(),
                OrderId = order.Id,
                PaymentMethodId = paymentMethodId,
                UserId = user.Id,
                Price = order.Price
            };
            order.DeliveryStatusId = 1;
            _context.Orders.Update(order);
            await _context.Payment.AddAsync(payment);
            await _context.SaveChangesAsync();

            if(paymentMethodId == 2)
            {
                PaymentCard paymentCard = new PaymentCard { Number = viewModel.PaymentCard.Number, PaymentId = _context.Payment.Where(t=>t.OrderId == order.Id).FirstOrDefault().Id, ValidUntil = valiUntil };
                await _context.PaymentCards.AddAsync(paymentCard);
                await _context.SaveChangesAsync();
            }

            return View(viewModel);
        }

        public ActionResult PaymentCard()
        {
            return PartialView("_PaymentCard");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
