using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public double Price { get; set; }
        public double? PriceWithDiscount { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public string ShortDescription { get; set; }
        
        public int Quantity { get; set; }

        public int NumberOfViews { get; set; }
        
        public string LongDescription { get; set; }

        [ForeignKey(nameof(Manufacturer))]
        public int? ManufacturerId { get; set; }

        [ForeignKey(nameof(Discount))]
        public int? DiscountId { get; set; }

        public Category Category { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public Discounts Discount { get; set; }

        public List<Photo> Photos { get; set; }
        
        public List<Comment> Comments { get; set; }
        public List<OrderItem> OrderItems { get; set; }


    }
}
