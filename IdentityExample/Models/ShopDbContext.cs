using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class ShopDbContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }


        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentMethod> PaymentMethod { get; set; }
        public DbSet<DeliveryStatus> DeliveryStatus { get; set; }
        public DbSet<PaymentCard> PaymentCards { get; set; }
        public DbSet<PaymentPayPal> PaymentPayPals { get; set; }
        public DbSet<Support> Support { get; set; }
        public DbSet<SupportThemes> SupportThemes { get; set; }
        public DbSet<Discounts> Discounts { get; set; }

        public ShopDbContext(DbContextOptions<ShopDbContext> options): base(options)
        {
           Database.EnsureCreated();
        }
    }
}
