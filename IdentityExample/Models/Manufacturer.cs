using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class Manufacturer
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [ForeignKey(nameof(Discount))]
        public int? DiscountId { get; set; }

        public List<Product> Products { get; set; }
        public Discounts Discount { get; set; }
    }
}
