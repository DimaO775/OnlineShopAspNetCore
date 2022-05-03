using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Models
{
    public class Payment
    {
        public int Id { get; set; }
        
        public int Price { get; set; }

        [ForeignKey(nameof(PaymentMethod))]
        public int PaymentMethodId { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public string Date { get; set; }

        public User User { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Order Order { get; set; }        
    }
}
