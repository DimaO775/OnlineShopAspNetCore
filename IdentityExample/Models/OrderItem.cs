using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }


        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
