using IdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModels
{
    public class PaymentOrderViewModel
    {
        public Payment Payment { get; set; }

        public Order Order { get; set; }

        public PaymentCard PaymentCard { get; set; }
        public PaymentPayPal PaymentPayPal { get; set; }

        public int ValidUntilMonth { get; set; }
        public int ValidUntilYear { get; set; }
        public int CVV { get; set; }
    }
}
